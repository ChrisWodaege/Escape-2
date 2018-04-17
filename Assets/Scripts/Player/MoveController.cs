using System;
using UnityEngine;


public class MoveController : IMoveController
{
    private IGridController _gridController;
    private IPlayerPosition _playerPosition;
    private IMovePlayerController _movePlayerController;

    public MoveController(IGridController gridController, IPlayerPosition playerPosition, IMovePlayerController movePlayerController)
    {
        this._gridController = gridController;
        this._playerPosition = playerPosition;
        this._movePlayerController = movePlayerController;
    }

    public void MoveUp()
    {
        MoveTo(GridDirection.Up);
    }

    public void MoveUpRight()
    {
        MoveTo(GridDirection.UpRight);
    }

    public void MoveDownRight()
    {
        MoveTo(GridDirection.DownRight);
    }

    public void MoveDown()
    {
        MoveTo(GridDirection.Down);
    }

    public void MoveDownLeft()
    {
        MoveTo(GridDirection.DownLeft);
    }

    public void MoveUpLeft()
    {
        MoveTo(GridDirection.UpLeft);
    }

    private void MoveTo(GridDirection direction)
    {
        Vector3 currentPosition = _playerPosition.GetCurrentPosition();
        Vector3 newPosition = currentPosition;
        try
        {
            newPosition = _gridController.GetNeighborTileVector(currentPosition, direction);
        }
        catch (Exception e)
        {
            //Debug.Log("Was not a valid move, so let it be :D");
        }
        _playerPosition.ChangePosition(newPosition);
        _movePlayerController.MovePlayer(currentPosition, newPosition);
        _movePlayerController.RotatePlayer(direction);
    }
}