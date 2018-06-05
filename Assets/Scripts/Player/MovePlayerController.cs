using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovePlayerController : MonoBehaviour, IMovePlayerController, IMoveController
{
    //IMoveController moveController;
	CommandReceiver moveController;
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
    private float _timeToRotate = 1f;
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

    }

    // Update is called once per frame
    void Update() {
		if (moving)
			MovePlayer ();
		if (rotating)
			RotatePlayer ();
    }

    private void MovePlayer() {
		_moveTimeRemaining -= Time.deltaTime;
        float percentage = 1 - (_moveTimeRemaining / _timeToMove);
        transform.position = Vector3.Lerp(_start, _end, percentage);

        if (_moveTimeRemaining < 0f) {
            moving = false;
            _walkingFinished.Invoke();
        }
    }

    private void RotatePlayer() {
        _rotateTimeRemaining -= Time.deltaTime;
        float percentage = 1 - (_rotateTimeRemaining / _timeToRotate);
        _playerAnimator.transform.rotation = Quaternion.Lerp(_fromRotation, _toRotation, percentage);

        if (_rotateTimeRemaining < 0f) {
			_walkingFinished.Invoke();
            rotating = false;
        }
    }

    public void MovePlayer(Vector3 from, Vector3 to) {
        _moveTimeRemaining = _timeToMove;
        _start = from;
        _end = to;
        moving = true;
    }

    public void RotatePlayer(GridDirection to) {
        _rotateTimeRemaining = _timeToRotate;
        _fromRotation = _playerAnimator.transform.rotation;
        _toRotation = Quaternion.LookRotation(_gridController.GetDirectionVector(to));
        rotating = true;
    }

    public void AddWalkingFinishedListener(UnityAction listener) {
		if(_walkingFinished==null)_walkingFinished = new UnityEvent();
		_walkingFinished.AddListener(listener);
    }

	public void sendCommand(Command c){
		moveController.execute (c);
	}

//	public void Move() {
//		Command c = new Command (CommandType.Move);
//		moveController.execute (c);
//	}
//
//	public void TurnLeft() {
//		Command c = new Command (CommandType.TurnLeft);
//		moveController.execute (c);
//	}
//
//	public void TurnRight() {
//		Command c = new Command (CommandType.TurnRight);
//		moveController.execute (c);
//	}

    public void MoveUp() {
		Command c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public void MoveUpRight() {
		Command c = new Command (CommandType.TurnRight);
		moveController.execute (c);
		c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public void MoveDownRight() {
		Command c = new Command (CommandType.TurnLeft);
		moveController.execute (c);
		c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public void MoveDown() {
		Debug.Log("Move South Step 2");
		Command c = new Command (CommandType.TurnLeft);
		moveController.execute (c);
		c = new Command (CommandType.TurnLeft);
		moveController.execute (c);
		c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public void MoveDownLeft() {
		Command c = new Command (CommandType.TurnRight);
		moveController.execute (c);
		c = new Command (CommandType.TurnRight);
		moveController.execute (c);
		c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public void MoveUpLeft() {
		Command c = new Command (CommandType.TurnRight);
		moveController.execute (c);
		c = new Command (CommandType.TurnRight);
		moveController.execute (c);
		c = new Command (CommandType.Move);
		moveController.execute (c);
    }

    public IEnumerator BootCharacter() {
        _playerAnimator.SetBool("StartUp", true);
        yield return new WaitForSeconds(5);
    }

    public void ChangeHandUp(bool up) {
        _playerAnimator.SetBool("HandUp", up);
    }
}