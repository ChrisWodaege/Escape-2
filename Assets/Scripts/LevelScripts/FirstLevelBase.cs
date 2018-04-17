using System.Collections;
using UnityEngine;

public class FirstLevelBase : BaseLevelScript
{
    private MovePlayerController _movePlayerController;

    private void Awake()
    {
        _movePlayerController = GameObject.Find("Player").GetComponent<MovePlayerController>();
    }

    public void TestMove()
    {
        AddMethod(MoveCoroutine(_movePlayerController.MoveDown));
    }

    private IEnumerator MoveCoroutine(System.Action moveAction)
    {
        moveAction();

        yield return new WaitForSeconds(1);

        LoadNextLevel();
    }
}
