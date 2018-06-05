using System;
using System.Collections.Generic;
using UnityEngine;

public interface CommandReceiver{
	void execute(Command command);
}

public class Command {
	public CommandType type;
	public Command(CommandType type){
		this.type = type;
	}
}

public enum CommandType {
	Move,
	TurnRight,
	TurnLeft
}

public class MoveController : CommandReceiver {
    private IGridController _gridController;
    private IPlayerPosition _playerPosition;
    private IMovePlayerController _movePlayerController;
	private int direction;
	private Queue<Command> commandQueue;

    public MoveController(IGridController gridController, IPlayerPosition playerPosition, IMovePlayerController movePlayerController) {
        this._gridController = gridController;
        this._playerPosition = playerPosition;
        this._movePlayerController = movePlayerController;
		_movePlayerController.AddWalkingFinishedListener(commandFinished);
		commandQueue = new Queue<Command> ();
    }

	public void commandFinished(){
		executeCommand();
	}

	public void execute(Command command) {
		commandQueue.Enqueue (command);
		if (active == false) {
			active = true;
			executeCommand ();
		}
	}

	public bool active = false;
	public void executeCommand(){
		if (commandQueue.Count == 0) {
			active = false;
			return;
		}

		Command command = commandQueue.Dequeue ();
		Debug.Log ("Command Received:"+command.type.ToString());
		switch (command.type) {
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
		}
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
		Debug.Log ("Move");
		Vector3 currentPosition = _playerPosition.GetCurrentPosition();
		Vector3 newPosition = currentPosition;
	    try {
			Debug.Log ("Move1");
	        newPosition = _gridController.GetNeighborTileVector(currentPosition, direction);
			Debug.Log ("Move2");
			_playerPosition.ChangePosition(newPosition);
			Debug.Log ("Move3");
			_movePlayerController.MovePlayer(currentPosition, newPosition);
			Debug.Log ("Move4");
	    }
	    catch (Exception e) {
			Debug.Log(e.Message);
	    }
		Debug.Log ("Move5");
    }
}