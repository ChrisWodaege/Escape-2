using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MoveControllerTest {
    private Vector3 _playerPosition;
    private IMovePlayerController _playerMoveController;

    [SetUp]
    public void setup() {
		_playerPosition = new Vector3(0,0,0);
        _playerMoveController = new MovePlayerController();     
    }

    public class MockGridController {
        public Vector3 GetNeighborTileVector(Vector3 fromTile, GridDirection direction) {
            if (fromTile.x == 1.0f) return new Vector3(0, 1, 0);
            if (fromTile.x == 2.0f) return new Vector3(0, 1, 1);
            if (fromTile.x == 3.0f) return new Vector3(1, 1, 0);
            if (fromTile.x == 0.0f) return new Vector3(1, 1, 1);

            return new Vector3(2, 2, 2);
        }
    }
}