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
	Take,
	Drop
}

public class MoveController : MonoBehaviour, CommandReceiver {
	private HexGridController _gridController;
    private Vector3 _playerPosition;
    private IMovePlayerController _movePlayerController;
	private int direction;
	private Queue<Command> commandQueue;
	private Camera _mainCamera;

	public MoveController(HexGridController gridController, Vector3 playerPosition, IMovePlayerController movePlayerController) {
    }

	public void Init(HexGridController gridController, Vector3 playerPosition, IMovePlayerController movePlayerController){
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
		foreach(Command c in commands) {
			commandQueue.Enqueue (c);
		}

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
		Debug.Log ("Excecute:"+command.type.ToString());
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
		case CommandType.Take: {
				takeItem ();
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
		if (_methodController == null) {
			//_methodController = GameObject.Find("CodingBox/CodingBox").GetComponent<CodingBoxMethodController>();
			_methodController.SetOnCompleteAction(AllowRunndingCode);
		}


		_methodController.AddMethod(method);
	}

	protected void AllowRunndingCode(){
		//LevelController.AllowRunningCode();
	}

	private void takeItem() {
		Debug.Log ("TakeItem");
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
		GameObject tile = ((HexGridController)_gridController).getTileAtPosition (currentPosition, (GridDirection)direction);

		if(tile.transform.childCount>0) {
			if(tile.transform.GetChild(0).name == "envStone") {
				objectInRobotsHand = tile.transform.GetChild(0).gameObject;
				objectInRobotsHand.transform.parent = this.transform;
				objectInRobotsHand.transform.localPosition = new Vector3 (0, 1.5f, 0);
				((HexGridController)_gridController).setBlockStateOfTile (currentPosition, (GridDirection)direction,true);
				((MovePlayerController)_movePlayerController).TakeObject();
			}
		}
	}

	GameObject objectInRobotsHand;

	private void dropItem() {
		if(objectInRobotsHand)
		Debug.Log ("DropItem");
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
		GameObject tile = ((HexGridController)_gridController).getTileAtPosition (currentPosition, (GridDirection)direction);
		Debug.Log (tile.name);
		Debug.Log (tile.transform.childCount);
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