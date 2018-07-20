using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovePlayerController : MonoBehaviour, IMovePlayerController {
    //IMoveController moveController;
	CommandReceiver moveController;
	Vector3 playerPosition;

    [SerializeField]
    private Vector3 _start;
    [SerializeField]
    private Vector3 _end;
    [SerializeField]
    private float _timeToMove = 2f;
    private float _moveTimeRemaining = 0f;
    private UnityEvent _walkingFinished;

    [SerializeField]
    private float _timeToRotate = 2f;
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
    void Start() {
        Init(Vector3.zero); //usually overwriten by HexGridController
        KeyMovement = true;
    }

    public void Init(Vector3 startPosition) {
        _start = startPosition;
		playerPosition = new Vector3(startPosition.x,startPosition.y,startPosition.z);
		//moveController = GameObject.Instantiate ();
//		moveController = gameObject.AddComponent(new MoveController(_gridController, playerPosition, this));
		moveController = gameObject.AddComponent<MoveController>() as MoveController;
		((MoveController)moveController).Init(_gridController, playerPosition, this);
//        moveController = new MoveController(_gridController, playerPosition, this);
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

		Debug.Log (_moveTimeRemaining);

        if (moving && _moveTimeRemaining < 0f) {
            moving = false;
            _walkingFinished.Invoke();
        }
    }

    private void RotatePlayer() {
        _rotateTimeRemaining -= Time.deltaTime;
        float percentage = 1 - (_rotateTimeRemaining / _timeToRotate);
        _playerAnimator.transform.rotation = Quaternion.Lerp(_fromRotation, _toRotation, percentage);

        if (rotating && _rotateTimeRemaining < 0f) {
			//_walkingFinished.Invoke();
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
		if (_walkingFinished == null) {
			//This function gets called multiple times, we need only one listener
			_walkingFinished = new UnityEvent ();
			_walkingFinished.AddListener (listener);
		}
    }

	public void sendCommand(Command c) {
		Debug.Log ("SendCommand MovePlayController");
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