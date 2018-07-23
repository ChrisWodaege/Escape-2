using System;
using UnityEngine;

public class HexGridController : MonoBehaviour
{
    [Header("Grid properties")]
    [SerializeField]
    private float _tileDistance = 1f;
    private Vector3[] _directionVectors; //calculated in Start()

    private float _xDistance;
    private float _yDistance; // !based on z-coordinate

    private int _width = 100;
    private int _height = 50;

    [SerializeField]
    private int _borderThickness = 5;
    
    [Header("Tiles")]
    [SerializeField]
    private TileManager _tileManager;
    [Space(10)]
    [SerializeField]
    private int[] _borderTileIDs;

    [Header("World")]
    [SerializeField]
    private HexWorld _hexWorld;
    private GameObject[,] _tiles;

    [Space(10)]
    [Header("Player")]
    private MovePlayerController _player;
    private GameObject _tileParent;

    private void Awake()
    {
        GenerateDirectionVectors();
    }

    // Use this for initialization
    void Start()
    {
        _player = FindObjectOfType<MovePlayerController>();

        if (_borderThickness < 0)
        {
            _borderThickness = 0;
        }
    }

    private void GenerateDirectionVectors()
    {
        _directionVectors = new Vector3[6];

        Vector3 currentVector = Vector3.forward * _tileDistance;
        for (int i = 0; i < 6; i++)
        {
            _directionVectors[i] = currentVector;
            currentVector = Quaternion.AngleAxis(60, Vector3.up) * currentVector;
        }

        _xDistance = _directionVectors[1].x;
        _yDistance = _directionVectors[0].z;
    }

    public Vector3 GetDirectionVector(GridDirection direction)
    {
        return _directionVectors[(int)direction];
    }
    
    public Vector3 GetTileVector(GridPosition position)
    {
        float x = GetDirectionVector(GridDirection.DownRight).x * position.X;
        float z = GetDirectionVector(GridDirection.Down).z * position.Y;
        if (position.X % 2 != 0)
        {
            z += GetDirectionVector(GridDirection.DownRight).z;
        }
        return new Vector3(x, 0, z);
    }

    //from IGridController - used by movecontroller

    public Vector3 GetNeighborTileVector(Vector3 fromTile, GridDirection direction)
    {
        GridPosition nearestGridPosition = GetNearestGridPosition(fromTile);
        return GetGridPositionFrom(nearestGridPosition, direction);
    }

    private Vector3 GetGridPositionFrom(GridPosition fromPosition, GridDirection direction) {
        if (!IsInsideWorld(fromPosition))
        {
            //invalid fromTile
            throw new ArgumentOutOfRangeException("FromTile is not within range");
        }


        GridPosition neighborPosition = fromPosition.GetNeighborGridPosition(direction);
        if (!IsInsideWorld(neighborPosition))
        {
            throw new IndexOutOfRangeException("The tile you want to access is not inside the world bounds.");
        }
        if (!IsTileWalkable(neighborPosition))
        {
            throw new ArgumentException("The tile you want to access is blocked somehow.");
        }
        return GetTileVector(neighborPosition);
    }

    public GridPosition GetNearestGridPosition(Vector3 position)
    {        
        //being a little generous with the boundaries
        int x1 = (int)(position.x / _xDistance);
        int y1 = (int)(-position.z / _yDistance);

        //get distance to 4/6/_9_ nearest neighbors

        float minSqrDistance = float.MaxValue;
        GridPosition closestGridPosition = new GridPosition(-1, -1);

        for (int xi = -1; xi <= 1; xi++)
        {
            for (int yi = -1; yi <= 1; yi++)
            {
                GridPosition currentGridPosition = new GridPosition(x1 + xi, y1 + yi);
                float currentSqrDistance = GetSqrDistance(position, currentGridPosition);

                if (currentSqrDistance < minSqrDistance)
                {
                    minSqrDistance = currentSqrDistance;
                    closestGridPosition = currentGridPosition;
                }
            }
        }
        
        return closestGridPosition;
    }

    private float GetSqrDistance(Vector3 position, GridPosition currentGridPosition)
    {        
        //check for out of bounds
        if (!IsInsideWorld(currentGridPosition))
        {
            return float.MaxValue;
        }
        Vector3 tilePosition = _tiles[currentGridPosition.X, currentGridPosition.Y].transform.position;
        float dx = position.x - tilePosition.x;
        float dy = position.z - tilePosition.z;
        float sqrDistance = dx * dx + dy * dy;
        //Debug.Log("Position: " + position + " | grid: [" + currentGridPosition.X + "|" + currentGridPosition.Y + " | sqrDistance: " + sqrDistance);
        
        return sqrDistance;
    }

	public void setBlockStateOfTile(Vector3 fromTile, GridDirection direction, bool state) {
//		GridPosition nearestGridPosition = GetNearestGridPosition(fromTile);
//		GridPosition gridPosition = nearestGridPosition.GetNeighborGridPosition(direction);
//		GridTile tile = _hexWorld.GetTile(gridPosition);
		//SetWalkable
		_tileManager.blockTile (GetGridTile(fromTile,direction), state);
	}

	public GridTile GetGridTile(Vector3 fromTile, GridDirection direction) {
		GridPosition nearestGridPosition = GetNearestGridPosition(fromTile);
		GridPosition gridPosition = nearestGridPosition.GetNeighborGridPosition(direction);
		return _hexWorld.GetTile(gridPosition);
	}

    public bool IsTileWalkable(GridPosition gridPosition)
    {
        if (!IsInsideWorld(gridPosition)) {
			Debug.Log ("NotInsideWorld");
            return false;
        }

        GridTile tile = _hexWorld.GetTile(gridPosition);
		if (tile.tileID == 3) { //Water is blocked
			return false;
		}


        return _tileManager.GetIsWalkable(tile.tileID, tile.itemID);
    }


	public GameObject getTileAtPosition(Vector3 fromTile, GridDirection direction){
		GridPosition nearestGridPosition = GetNearestGridPosition(fromTile);
		GridPosition gridPosition = nearestGridPosition.GetNeighborGridPosition(direction);

		if (!IsInsideWorld(gridPosition))
		{
			return null; //TODO hotfix better use an default GridTile
		}
		return _tiles [gridPosition.X, gridPosition.Y];
	}

    private bool IsInsideWorld(GridPosition gridPosition)
    {
        return (gridPosition.X >= 0 && gridPosition.X < _width && gridPosition.Y >= 0 && gridPosition.Y < _height);
    }

    public void ReplaceTile(GridPosition gridPosition, GridTile gridTile)
    {
        _hexWorld.ReplaceTile(gridPosition, gridTile);

        Destroy(_tiles[gridPosition.X, gridPosition.Y]);

        _tiles[gridPosition.X, gridPosition.Y] = CreateTile(gridPosition, gridTile);
    }

    public void RegisterWorld(int width, GridTile[] tiles, int startX, int startY)
    {
        if(_tileParent != null)
        {
            Destroy(_tileParent);
        }

        _width = width;
        _height = tiles.Length / width;
        _tiles = new GameObject[_width, _height];
        _tileParent = new GameObject("tileParent");
        _tileParent.transform.parent = transform;

        for (int y = -_borderThickness; y < _height + _borderThickness; y++)
        {
            for (int x = -_borderThickness; x < _width + _borderThickness; x++)
            {
                GridPosition currentGridPosition = new GridPosition(x, y);
                //border
                if (!IsInsideWorld(currentGridPosition))
                {
                    //TODO: use CreateTile();? (Jonas)
                    Vector3 borderTilePosition = GetTileVector(currentGridPosition);
                    int randomRotation = UnityEngine.Random.Range(0, 6);
                    Quaternion borderTileRotation = Quaternion.LookRotation(GetDirectionVector((GridDirection)randomRotation));
                    int borderTileIndex = UnityEngine.Random.Range(0, _borderTileIDs.Length);
                    Instantiate(_tileManager.GetTilePrefab(_borderTileIDs[borderTileIndex]), borderTilePosition, borderTileRotation, _tileParent.transform);
                    continue;
                }

                int currentIndex = y * _width + x;
                GridTile currentTile = tiles[currentIndex];
                
                _tiles[x, y] = CreateTile(currentGridPosition, currentTile);
            }
        }

        _player.Init(GetTileVector(new GridPosition(startX, startY)));
    }

    private GameObject CreateTile(GridPosition gridPosition, GridTile gridTile)
    {
        //save position (only for better readability of the generated json file)
        gridTile.x = gridPosition.X;
        gridTile.y = gridPosition.Y;

        Vector3 position = GetTileVector(gridPosition);
        Quaternion tileRotation = Quaternion.LookRotation(GetDirectionVector(gridTile.tileRotation));
        GameObject tile = Instantiate(_tileManager.GetTilePrefab(gridTile.tileID), position, tileRotation, _tileParent.transform);
        _tiles[gridPosition.X, gridPosition.Y] = tile;

        if (gridTile.itemID != -1)
        {
            Quaternion objectRotation = Quaternion.LookRotation(GetDirectionVector(gridTile.itemRotation));
            Instantiate(_tileManager.GetItemPrefab(gridTile.itemID), position, objectRotation, tile.transform);
        }

        return tile;
    }

	public bool TileIsOfType(Vector3 fromTile, GridDirection direction,int type){
		return GetGridTile(fromTile,direction).tileID == type;
	}

	public bool TileContainsItem(){
		return false;
	}



	public bool TileHasSpaceForObject(){
		//TODO sagt aus ob ein Tile frei ist
		//Nicht frei wenn ein Stein drauf liegt
		return false;
	}

	public GameObject getItemFromTile(){
		return null;
	}

	public bool putStoneAtTile(GameObject stone,Vector3 currentPosition,GridDirection direction) {

		if (!TileContainsStone (currentPosition, direction)) {
			if(!TileIsOfType(currentPosition,direction,4)) { //Test if contains obstacle
				GameObject tile = getTileAtPosition (currentPosition, direction);
				stone.transform.parent = tile.transform;
				stone.transform.localPosition = new Vector3 (0, 0, 0);

				setBlockStateOfTile (currentPosition, direction,false);

				return tile.transform.GetChild(0).gameObject;
			}
		}

		return false;
	}

	public GameObject getStoneFromTile(Vector3 currentPosition,GridDirection direction) {
		if (TileContainsStone (currentPosition, direction)) {
			GameObject tile = getTileAtPosition (currentPosition, direction);
			setBlockStateOfTile (currentPosition, direction,true);
			return tile.transform.GetChild(0).gameObject;
		}
		return null;
	}

	private bool TileContainsStone(Vector3 currentPosition,GridDirection direction) {
		GameObject tile = getTileAtPosition (currentPosition, direction);
		if (tile.transform.childCount > 0) {
			if (tile.transform.GetChild (0).name == "envStone") {
				return true;
			}
		}
		return false;
	}
}