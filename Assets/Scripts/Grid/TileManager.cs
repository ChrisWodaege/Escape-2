using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    [Header("Tiles")]
    private Tile[] _tiles;
    [SerializeField]
    [Space(10)]
    [Tooltip("Use this tile if some kind of error occurs.")]
    private Tile _fallbackTile;

    [Header("Items")]
    [SerializeField]
    private Item[] _items;
    [SerializeField]
    private Item _fallbackItem;

    public GameObject GetTilePrefab (int tileID)
    {
        if (tileID < 0 || tileID >= _tiles.Length || _tiles[tileID] == null)
        {
            return _fallbackTile.tilePrefab;
        }

        return _tiles[tileID].tilePrefab;
    }

    public GameObject GetItemPrefab (int itemID)
    {
        if (itemID < 0 || itemID >= _items.Length || _items[itemID] == null)
        {
            return _fallbackItem.itemPrefab;
        }

        return _items[itemID].itemPrefab;
    }

    public bool GetIsWalkable(int tileID, int itemID)
    {
        if (!_tiles[tileID].walkable)
        {
            return false;
        }
        if (itemID != -1 && _items[itemID].blockingTile)
        {
            return false;
        }
        return true;
    }
}

[System.Serializable]
public class Tile
{
    public string name;
    public GameObject tilePrefab;
    public bool walkable;
    //TODO: can be random? or part of some random tile configuration
}

[System.Serializable]
public class Item
{
    public string name;
    public GameObject itemPrefab;
    public bool blockingTile;
}