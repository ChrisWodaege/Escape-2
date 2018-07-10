using System.Collections;
using UnityEngine;

public class BaseLevelScript : MonoBehaviour
{
    private CodingBoxMethodController _methodController = null;


    private LevelScriptController LevelController
    {
        get
        {
            return GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelScriptController>();
        }
    }

    protected void AddMethod(IEnumerator method)
    {
        if (_methodController == null)
        {
            _methodController = GetComponent<CodingBoxMethodController>();

            _methodController.SetOnCompleteAction(AllowRunndingCode);
        }

        _methodController.AddMethod(method);
    }

    protected virtual void StartLoop()
    {
        if (_methodController != null)
        {
            _methodController.StartMethodLoop();
        }
        else
        {
            AllowRunndingCode();
        }
    }

    protected virtual void LoadNextLevel() {
        AllowRunndingCode();
        LevelController.LoadNextLevel();
    }

    protected void AllowRunndingCode()
    {
        LevelController.AllowRunningCode();
    }

    // EasterEgg
    protected void Escape()
    {
        AddMethod(EscapeCoroutine());
    }

    private IEnumerator EscapeCoroutine()
    {
        var playerController = FindObjectOfType<MovePlayerController>();

        if (playerController != null)
        {
            var playerAnimator = playerController.GetComponentInChildren<Animator>();

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Escape");
            }
        }

        yield return new WaitForSeconds(2);
    }
}
