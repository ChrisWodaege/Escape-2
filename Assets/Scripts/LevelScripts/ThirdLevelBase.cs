using System.Collections;
using UnityEngine;

public class ThirdLevelBase : BaseLevelScript
{
    private Animator _doorAnimator;
    private MovePlayerController _playerController;
    private Animator _crankAnimator;

    protected bool doorIsClosed = true;

    protected Coroutine _doWhileLoopCoroutine = null;
    private Coroutine _rotateWinderCoroutine = null;

    private Animator _cameraAnimator;

    private bool _runningRotationAnimation = false;

    private int _whileLoopCounts = 0;

    private void Awake()
    {
        _doorAnimator = GameObject.FindGameObjectWithTag("Gate1").GetComponent<Animator>();
        _playerController = FindObjectOfType<MovePlayerController>();

        _crankAnimator = GameObject.FindGameObjectWithTag("Crank1").GetComponent<Animator>();
        _cameraAnimator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }

    public void RotateWheel()
    {
        if (_whileLoopCounts > 100)
        {
            doorIsClosed = false;

            if (_rotateWinderCoroutine == null)
            {
                _rotateWinderCoroutine = StartCoroutine(RotateWinderCoroutine());
            }
        }

        _whileLoopCounts++;
    }

    protected IEnumerator RotateWinderCoroutine()
    {
        // TODO start kurbel animation
        _playerController.ChangeHandUp(true);
        _crankAnimator.SetTrigger("Rotate");
        yield return new WaitForSeconds(3); // Animationtime

        _playerController.ChangeHandUp(false);
        _doorAnimator.SetTrigger("OpenGate");

        LoadNextLevel();

        MoveCameraToSpaceShip();
    }

    protected void MoveCameraToSpaceShip()
    {
        _cameraAnimator.SetTrigger("MoveToSpaceShip");
    }
}
