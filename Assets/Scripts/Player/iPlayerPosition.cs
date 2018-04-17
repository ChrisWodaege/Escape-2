using UnityEngine;


public interface IPlayerPosition
{
    Vector3 GetCurrentPosition();

    void ChangePosition(Vector3 position);
}