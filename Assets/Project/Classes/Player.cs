﻿using System;
using System.Threading.Tasks;

namespace Project.Classes {
    public abstract class Player {
        public bool myTurn;
        private bool _moveDone;

        private int _numOfWalls;

        // private IPlayerController _playerController;
        public Pawn Pawn { get; set; }

        public int NumOfWalls {
            get => _numOfWalls;
            private set {
                if (_numOfWalls == value) {
                    return;
                }

                _numOfWalls = value;
                NumOfWallsChanged?.Invoke();
            }
        }

        public event Action NumOfWallsChanged;

        public Player(Pawn pawn = null, int numOfWalls = Consts.DEFAULT_NUM_OF_WALLS) {
            Pawn = pawn;
            NumOfWalls = numOfWalls;
        }

        public virtual async Task MakeMove() {
            // await Task.Run(() => _playerController.GetNextMove()?.Invoke());
            while (!_moveDone) {
                await Task.Yield();
            }

            _moveDone = false;
        }

        public bool TrySetWall(Wall wall) {
            if (Pawn == null || !myTurn || _moveDone || NumOfWalls <= 0 || !Pawn.Field.TrySetWall(wall)) {
                return false;
            }

            NumOfWalls--;
            _moveDone = true;
            return true;
        }

        public bool TryMovePawn(Point newPos) {
            if (Pawn == null || !myTurn || _moveDone || !Pawn.TryMove(newPos)) {
                return false;
            }

            _moveDone = true;
            return true;
        }
    }
}