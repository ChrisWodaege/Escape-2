using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SecondLevelBase : BaseLevelScript
{
    private MovePlayerController _movePlayerController;
    private HexGridController _gridController;
    private bool _movementFinished;
    private GridPosition targetPosition = new GridPosition(4,3);

    private void Awake()
    {
        _movePlayerController = FindObjectOfType<MovePlayerController>();
        _movePlayerController.AddWalkingFinishedListener(MovementFinishedListener);
        _gridController = FindObjectOfType<HexGridController>();
    }

    public void MoveSouth()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveDown));
    }

    private void MovementFinishedListener()
    {
        _movementFinished = true;
    }

    private IEnumerator MoveCoroutine(System.Action moveAction)
    {
        moveAction();

        yield return new WaitUntil(() => _movementFinished);
        _movementFinished = false;

        if (_gridController.GetNearestGridPosition(_movePlayerController.transform.position).Equals(targetPosition)) {
            _movePlayerController.RotatePlayer(GridDirection.DownRight);
            LoadNextLevel();
        }
    }

    public void MoveSouthWest()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveDownLeft));
    }

    public void MoveSouthEast()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveDownRight));
    }

    public void MoveNorth()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveUp));
    }

    public void MoveNorthWest()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveUpLeft));
    }

    public void MoveNorthEast()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveUpRight));
    }
}
