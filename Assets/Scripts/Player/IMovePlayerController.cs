using UnityEngine;
using UnityEngine.Events;

public interface IMovePlayerController {
	void AddWalkingFinishedListener(UnityAction listener);
    void MovePlayer(Vector3 from, Vector3 to);
    void RotatePlayer(GridDirection to);
}