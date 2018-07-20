using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public interface CommandReceiver{
	void execute(List<Command> command);
}

public class Command {
	public CommandType type;
	public Command(CommandType type){
		this.type = type;
	}
}

public enum CommandType {
	Boot,
	Move,
	TurnRight,
	TurnLeft,
	Put,
	Drop
}

public class MoveController : MonoBehaviour, CommandReceiver {
    private IGridController _gridController;
    private Vector3 _playerPosition;
    private IMovePlayerController _movePlayerController;
	private int direction;
	private Queue<Command> commandQueue;
	private Camera _mainCamera;

    public MoveController(IGridController gridController, Vector3 playerPosition, IMovePlayerController movePlayerController) {
    }

	public void Init(IGridController gridController, Vector3 playerPosition, IMovePlayerController movePlayerController){
		this._gridController = gridController;
		this._playerPosition = playerPosition;
		this._movePlayerController = movePlayerController;
		_movePlayerController.AddWalkingFinishedListener(commandFinished);
		commandQueue = new Queue<Command> ();
		direction = 3;
	}

	private void Awake(){
		_mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		_cameraAnimator = _mainCamera.GetComponent<Animator>();
		_methodController = gameObject.AddComponent<CodingBoxMethodController>() as CodingBoxMethodController;
	}

	public void commandFinished(){
		executeCommand();
	}

	public void execute(List<Command> commands) {
		foreach(Command c in commands){
			Debug.Log ("Excecute:"+c.type.ToString());
			commandQueue.Enqueue (c);

		}
		Debug.Log ("commandQueue.Count:"+commandQueue.Count);
		if (active == false) {
			active = true;
			executeCommand ();
		}
	}

	public bool active = false;
	private void executeCommand(){
		if (commandQueue.Count == 0) {
			active = false;
			return;
		}

		Command command = commandQueue.Dequeue ();	
		switch (command.type) {
		case CommandType.Boot:
			{
				AddMethod(_movePlayerController.BootCharacter());
				AddMethod(ControllCamera());
				AddMethod(BootCoroutine());
				_methodController.StartMethodLoop();
				_movePlayerController.BootCharacter ();
				break;			
			}
		case CommandType.Move:
			{
				MoveTo ((GridDirection)direction);
				break;			
			}
		case CommandType.TurnLeft: {
				rotateLeft();
				break;
			}
		case CommandType.TurnRight: {
				rotateRight();
				break;			
			}
		case CommandType.Put: {
				putItem ();
				break;			
			}
		case CommandType.Drop: {
				dropItem ();
				break;			
			}
		}
		if (commandQueue.Count == 0) {
			active = false;
			return;
		}
	}

	private IEnumerator BootCoroutine() {
		//TODO Hier müsste dann die GameStateMachine aktuallisiert werden
		//LoadNextLevel();
		yield return null;
	}
	private Animator _cameraAnimator;
	private CodingBoxMethodController _methodController = null;

	private IEnumerator ControllCamera() {
		_cameraAnimator.SetBool("Zoom", false);
		yield return new WaitForSeconds(1);
	}
		
	protected void AddMethod(IEnumerator method)
	{
		if (_methodController == null)
		{
			

			Debug.Log ("Line 107"+			gameObject.name);
			//_methodController = GameObject.Find("CodingBox/CodingBox").GetComponent<CodingBoxMethodController>();
			Debug.Log ("Line 109");
			_methodController.SetOnCompleteAction(AllowRunndingCode);
		}


		_methodController.AddMethod(method);
	}

	protected void AllowRunndingCode(){
		//LevelController.AllowRunningCode();
	}

	private void putItem() {
//		GameObject stone = GameObject.("envStone");
//		Debug.Log (stone.transform.position.ToString());
//		stone.transform.position = new Vector3 (0, 0, 0);
		//transform.localPosition = new VectorAA3 (1, 0, 0);
//		stone.transform.Translate(new Vector3 (1, 0, 0));
		//stone.transform.position = new Vector3 (1, 0, 0);
//		stone.transform.localPosition = new Vector3 (1, 0, 0);
	

		Debug.Log ("PutItem");
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
		//newPosition = _gridController.GetNeighborTileVector(currentPosition, (GridDirection)direction);
		GameObject tile = ((HexGridController)_gridController).getTileAtPosition (currentPosition, (GridDirection)direction);
		Debug.Log (tile.name);
		Debug.Log (tile.transform.childCount);

		//tile.SetActive (false);
		Debug.Log (tile.transform.GetChild(0).name);
		objectInRobotsHand = tile.transform.GetChild(0).gameObject;
		objectInRobotsHand.transform.parent = this.transform;
		objectInRobotsHand.transform.localPosition = new Vector3 (0, 1.5f, 0);
		((HexGridController)_gridController).setBlockStateOfTile (currentPosition, (GridDirection)direction,true);
//		stone.transform.position = new Vector3 (0, 0, 0);
		((MovePlayerController)_movePlayerController).TakeObject();
	}

	GameObject objectInRobotsHand;

	private void dropItem() {
		Debug.Log ("DropItem");
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
		//newPosition = _gridController.GetNeighborTileVector(currentPosition, (GridDirection)direction);
		GameObject tile = ((HexGridController)_gridController).getTileAtPosition (currentPosition, (GridDirection)direction);
		Debug.Log (tile.name);
		Debug.Log (tile.transform.childCount);

		//tile.SetActive (false);


		objectInRobotsHand.transform.parent = tile.transform;
		objectInRobotsHand.transform.localPosition = new Vector3 (0, 0, 0);
		((HexGridController)_gridController).setBlockStateOfTile (currentPosition, (GridDirection)direction,true);
		Debug.Log (tile.transform.childCount);
		((MovePlayerController)_movePlayerController).DropObject();
	}

	private void rotateLeft(){
		direction--;
		if (direction < 0)
			direction = 5;
		_movePlayerController.RotatePlayer((GridDirection)direction);
	}

	private void rotateRight(){
		direction++;
		if (direction > 5)
			direction = 0;
		_movePlayerController.RotatePlayer((GridDirection)direction);
	}

	private void MoveTo(GridDirection direction) {
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
	    try {
	        newPosition = _gridController.GetNeighborTileVector(currentPosition, direction);
			_playerPosition = newPosition;
			Debug.Log(currentPosition.ToString()+":::::"+newPosition.ToString());
			_movePlayerController.MovePlayer(currentPosition, newPosition);
	    }
	    catch (Exception e) {
			Debug.Log(e.Message);
	    }
    }
}