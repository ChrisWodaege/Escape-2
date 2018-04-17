using UnityEngine;


public interface IMovePlayerController
{
    void MovePlayer(Vector3 from, Vector3 to);
    void RotatePlayer(GridDirection to);
}