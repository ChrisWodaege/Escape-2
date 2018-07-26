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
		Debug.Log (commandQueue.Count + ":active:" + active);
		if(commandQueue.Count > 0 || active){
			Debug.Log("Running");
			return;
		}

		foreach(Command c in commands) {
			if (!androidBooted) {
				if (c.type == CommandType.Boot) {
					androidBooted = true;
				} else {
					continue;
				}
			} else if (c.type == CommandType.Boot) {
				continue;
			}
			commandQueue.Enqueue (c);
		}

		if (active == false) {
			active = true;
			executeCommand ();
		}
	}
	private bool androidBooted = false;
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
		case CommandType.Move: {
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
	}

	private IEnumerator BootCoroutine() {
		//TODO Hier müsste dann die GameStateMachine aktuallisiert werden
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
		


		_methodController.AddMethod(method);
	}

//	protected void AllowRunndingCode(){
//
//	}

	private void takeItem() {
		try {
			Debug.Log ("TakeItem");
			if (objectInRobotsHand == null) { //Only take if robot has no object taken
				Vector3 currentPosition = _playerPosition;
				Vector3 newPosition = currentPosition;
				GameObject stone = _gridController.getStoneFromTile (_playerPosition, (GridDirection)direction);
				if (stone != null) {
					objectInRobotsHand = stone;
					objectInRobotsHand.transform.parent = this.transform;
					objectInRobotsHand.transform.localPosition = new Vector3 (0, 1.5f, 0);
				}
			}

			((MovePlayerController)_movePlayerController).TakeObject();
		}
		catch (Exception e) {
			this.executeCommand ();
			Debug.Log(e.Message);
		}
	}

	GameObject objectInRobotsHand;

	private void dropItem() {
		try {
			if (objectInRobotsHand) {
				Debug.Log ("DropItem");
				if (_gridController.putStoneAtTile (objectInRobotsHand, _playerPosition, (GridDirection)direction)) {
					objectInRobotsHand = null;
				}
			}
			((MovePlayerController)_movePlayerController).DropObject();
		}
		catch (Exception e) {
			this.executeCommand ();
			Debug.Log(e.Message);
		}
	}

	private void rotateLeft(){

		direction--;
		if (direction < 0)
			direction = 5;
		GridTile tile = _gridController.GetGridTile (_playerPosition,(GridDirection)direction);
		Debug.Log ("TileID:"+tile.tileID);
		_movePlayerController.RotatePlayer((GridDirection)direction);
	}

	private void rotateRight(){		
		direction++;
		if (direction > 5)
			direction = 0;
		GridTile tile = _gridController.GetGridTile (_playerPosition,(GridDirection)direction);
		Debug.Log ("TileID:"+tile.tileID);
		_movePlayerController.RotatePlayer((GridDirection)direction);
	}

	private void MoveTo(GridDirection direction) {
		Vector3 currentPosition = _playerPosition;
		Vector3 newPosition = currentPosition;
	    try {
	        newPosition = _gridController.GetNeighborTileVector(currentPosition, direction);
			_playerPosition = newPosition;
			GridTile tile = _gridController.GetGridTile (_playerPosition,(GridDirection)direction);
			_movePlayerController.MovePlayer(currentPosition, newPosition);
	    }
	    catch (Exception e) {
			this.executeCommand ();
			Debug.Log(e.Message);
	    }
    }
}