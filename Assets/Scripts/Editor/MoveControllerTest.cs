using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MoveControllerTest 
{
    private MoveController _moveController;
    private Vector3 _playerPosition;
    private IGridController _gridController;
    private IMovePlayerController _playerMoveController;

    [SetUp]
    public void setup()
    {
        _gridController = new MockGridController();
		_playerPosition = new Vector3(0,0,0);
        _playerMoveController = new MovePlayerController();
        _moveController = new MoveController(_gridController, _playerPosition, _playerMoveController);
    }

    //[Test]
	//public void IntegrationTestForUpdatingPlayerPosition()
	//{
	//    _moveController.MoveUp();
	//    Assert.AreEqual(new Vector3(1,1,1), _playerPosition.GetCurrentPosition());
	//}

    public class MockGridController : IGridController
    {
        public Vector3 GetNeighborTileVector(Vector3 fromTile, GridDirection direction)
        {
            if (fromTile.x == 1.0f) return new Vector3(0, 1, 0);
            if (fromTile.x == 2.0f) return new Vector3(0, 1, 1);
            if (fromTile.x == 3.0f) return new Vector3(1, 1, 0);
            if (fromTile.x == 0.0f) return new Vector3(1, 1, 1);

            return new Vector3(2, 2, 2);
        }
    }
}