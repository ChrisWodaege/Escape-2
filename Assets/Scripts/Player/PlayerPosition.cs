using UnityEngine;
using System.Collections;

public class PlayerPosition : IPlayerPosition
{
    private Vector3 _currentPosition;

    public PlayerPosition(Vector3 startPosition)
    {
        this._currentPosition = startPosition;
    }

    public Vector3 GetCurrentPosition()
    {
        //Debug.Log("current position: " + _currentPosition.ToString());
        return _currentPosition;
    }

    public void ChangePosition(Vector3 newPosition)
    {
        this._currentPosition = newPosition;
    }
}