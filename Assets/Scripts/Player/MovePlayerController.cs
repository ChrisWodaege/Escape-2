using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovePlayerController : MonoBehaviour, IMovePlayerController, IMoveController
{
    IMoveController moveController;
    IPlayerPosition playerPosition;

    [SerializeField]
    private Vector3 _start;
    [SerializeField]
    private Vector3 _end;
    [SerializeField]
    private float _timeToMove = 1f;
    private float _moveTimeRemaining = 0f;
    private UnityEvent _walkingFinished;

    [SerializeField]
    private float _timeToRotate = 0.1f;
    private float _rotateTimeRemaining = 0f;
    private Quaternion _fromRotation;
    private Quaternion _toRotation;

    [SerializeField]
    private HexGridController _gridController;
    [SerializeField]
    private Animator _playerAnimator;

    public bool KeyMovement { get; set; }

    private bool moving = false;
    private bool _allowGeneralMoving = false;

    private bool rotating = false;

    // Use this for initialization
    void Start()
    {
        Init(Vector3.zero); //usually overwriten by HexGridController
        KeyMovement = true;
    }

    public void Init(Vector3 startPosition)
    {
        _start = startPosition;
        playerPosition = new PlayerPosition(_start);
        moveController = new MoveController(_gridController, playerPosition, this);
        transform.position = _start;
        _walkingFinished = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (_allowGeneralMoving && KeyMovement)
        {
            if (Input.GetKeyDown("w"))
            {
                MoveUp();
            }
            if (Input.GetKeyDown("e"))
            {
                MoveUpRight();
            }
            if (Input.GetKeyDown("d"))
            {
                MoveDownRight();
            }
            if (Input.GetKeyDown("s"))
            {
                MoveDown();
            }
            if (Input.GetKeyDown("a"))
            {
                MoveDownLeft();
            }
            if (Input.GetKeyDown("q"))
            {
                MoveUpLeft();
            }
        }

        if (moving)
        {
            MovePlayer();
        }
        if (rotating)
        {
            RotatePlayer();
        }
    }

    private void MovePlayer()
    {
        _moveTimeRemaining -= Time.deltaTime;

        float percentage = 1 - (_moveTimeRemaining / _timeToMove);
        transform.position = Vector3.Lerp(_start, _end, percentage);

        if (_moveTimeRemaining < 0f)
        {
            moving = false;
            _walkingFinished.Invoke();
        }
    }

    private void RotatePlayer()
    {
        _rotateTimeRemaining -= Time.deltaTime;

        float percentage = 1 - (_rotateTimeRemaining / _timeToRotate);
        _playerAnimator.transform.rotation = Quaternion.Lerp(_fromRotation, _toRotation, percentage);

        if (_rotateTimeRemaining < 0f)
        {
            rotating = false;
        }
    }

    public void MovePlayer(Vector3 from, Vector3 to)
    {
        _moveTimeRemaining = _timeToMove;
        _start = from;
        _end = to;
        moving = true;
    }

    public void RotatePlayer(GridDirection to)
    {
        _rotateTimeRemaining = _timeToRotate;
        _fromRotation = _playerAnimator.transform.rotation;
        _toRotation = Quaternion.LookRotation(_gridController.GetDirectionVector(to));
        rotating = true;
    }

    public void AddWalkingFinishedListener(UnityAction listener)
    {
        _walkingFinished.AddListener(listener);
    }

    public void MoveUp()
    {
        moveController.MoveUp();
    }

    public void MoveUpRight()
    {
        moveController.MoveUpRight();
    }

    public void MoveDownRight()
    {
        moveController.MoveDownRight();
    }

    public void MoveDown()
    {
        moveController.MoveDown();
    }

    public void MoveDownLeft()
    {
        moveController.MoveDownLeft();
    }

    public void MoveUpLeft()
    {
        moveController.MoveUpLeft();
    }

    public IEnumerator BootCharacter()
    {
        _playerAnimator.SetBool("StartUp", true);

        yield return new WaitForSeconds(5);
    }

    public void ChangeHandUp(bool up)
    {
        _playerAnimator.SetBool("HandUp", up);
    }
}