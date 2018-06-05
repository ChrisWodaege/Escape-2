using System.Collections;
using UnityEngine;

public class ZeroLevelBase : BaseLevelScript {
    private MovePlayerController _movePlayerController;
    private Camera _mainCamera;
    private Animator _cameraAnimator;

    private void Awake() {
        _movePlayerController = FindObjectOfType<MovePlayerController>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _cameraAnimator = _mainCamera.GetComponent<Animator>();
    }

    public void Boot() {
        AddMethod(_movePlayerController.BootCharacter());
        AddMethod(ControllCamera());
        AddMethod(BootCoroutine());
    }

    private IEnumerator BootCoroutine() {
        LoadNextLevel();
        yield return null;
    }

    private IEnumerator ControllCamera() {
        _cameraAnimator.SetBool("Zoom", false);
        yield return new WaitForSeconds(1);
    }
}
