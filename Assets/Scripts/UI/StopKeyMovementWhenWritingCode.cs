using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopKeyMovementWhenWritingCode : MonoBehaviour
{
    private MovePlayerController _movePlayerController;

    public void Start()
    {
        _movePlayerController = GameObject.Find("Player").GetComponent<MovePlayerController>();
    }

    public void OnSelectCodingBox()
    {
        _movePlayerController.KeyMovement = false;
    }

    public void OnDeselectCodingBox()
    {
        _movePlayerController.KeyMovement = true;
    }

    public void ToggleCodingBoxSelected()
    {
        _movePlayerController.KeyMovement = !_movePlayerController.KeyMovement;
    }
}
