using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HexWorld : MonoBehaviour {

    //properties can not be serialized (at least according to 2 google result pages)

    [Header("Save JSON - F1")]
    [SerializeField]
    private string _worldName;
    [Header("Load JSON - F2")]
    [SerializeField]
    private TextAsset _worldJSON;

    [Header("New World")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    [SerializeField]
    [HideInInspector]
    private long _timeStamp = -1L; // not sure what use this has, but it feels professional ;)

    [SerializeField]
    private int _startX;
    [SerializeField]
    private int _startY;

    [SerializeField]
    [Tooltip("Only edit in Unity Editor. This field is ignored when loading file.")]
    private GridTile defaultGridTile; // could not find a way to serialize but prevent json inclusion

    [HideInInspector]
    [SerializeField]
    private GridTile[] _tiles;

    private HexGridController _controller;

    void Start()
    {
        _controller = FindObjectOfType<HexGridController>();

        UnityEngine.Assertions.Assert.IsNotNull(_controller, "no hex grid controller found");

        if (_worldJSON == null)
        {
            CreateNewWorld();
            //if no world -> create new
            _controller.RegisterWorld(_width, _tiles, _startX, _startY);
        }
        else
        {
            //if world -> load world
            _worldName = _worldJSON.name;
            LoadFromJSON();
            _controller.RegisterWorld(_width, _tiles, _startX, _startY);
        }
    }

    private void CreateNewWorld()
    {
        _tiles = new GridTile[_width * _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                int index = y * _width + x; //TODO: _height or _width? (Jonas)
                _tiles[index] = new GridTile()
                {
                    tileID = defaultGridTile.tileID,
                    tileRotation = defaultGridTile.tileRotation,
                    itemID = defaultGridTile.itemID,
                    itemRotation = defaultGridTile.itemRotation,
                    x = x,
                    y = y
                };
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            Debug.Log("Saving...");
            SaveToJSON();
            Debug.Log("Done...");
        }

        if (Input.GetKeyUp(KeyCode.F2))
        {
            Debug.Log("Loading...");
            LoadFromJSON();
            Debug.Log("Done... ");
        }
    }

    private void SaveToJSON()
    {
        _timeStamp = DateTime.Now.Ticks;
        string path = "Assets/Worlds/" + _worldName + ".json";

        StreamWriter writer = new StreamWriter(path, false);

        writer.Write(JsonUtility.ToJson(this, true));
        writer.Close();

        AssetDatabase.ImportAsset(path); //reload in editor
    }

    private void LoadFromJSON()
    {
        //TODO: delete old world if exists
        string path = "Assets/Worlds/" + _worldName + ".json";

        StreamReader reader = new StreamReader(path);

        string json = reader.ReadToEnd();
        reader.Close();

        JsonUtility.FromJsonOverwrite(json, this);

        _controller.RegisterWorld(_width, _tiles, _startX, _startY);
    }

    public GridTile GetTile(GridPosition gridPosition)
    {
        return _tiles[gridPosition.Y * _width + gridPosition.X];
    }

    public void ReplaceTile(GridPosition gridPosition, GridTile gridTile)
    {
        int index = gridPosition.Y * _width + gridPosition.X;
        _tiles[index] = gridTile;
    }
}
