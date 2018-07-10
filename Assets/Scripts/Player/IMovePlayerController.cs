using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public interface IMovePlayerController {
	void AddWalkingFinishedListener(UnityAction listener);
    void MovePlayer(Vector3 from, Vector3 to);
    void RotatePlayer(GridDirection to);
	IEnumerator BootCharacter();
}