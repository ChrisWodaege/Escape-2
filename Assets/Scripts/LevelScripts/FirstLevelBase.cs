using System.Collections;
using UnityEngine;

public class FirstLevelBase : BaseLevelScript
{
    private MovePlayerController _movePlayerController;

    private void Awake() {
        _movePlayerController = GameObject.Find("Player").GetComponent<MovePlayerController>();
    }

	public void Move() {
		Command c = new Command(CommandType.Move);
		_movePlayerController.sendCommand (c);
	}

	public void TurnLeft() {
		Command c = new Command(CommandType.TurnLeft);
		_movePlayerController.sendCommand(c);
	}

	public void TurnRight() {
		Command c = new Command(CommandType.TurnRight);
		_movePlayerController.sendCommand(c);
	}
}
