using UnityEngine;

public interface IGridController
{
    Vector3 GetNeighborTileVector(Vector3 fromTile, GridDirection direction);
}