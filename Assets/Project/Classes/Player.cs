using System.Threading.Tasks;
using Project.Interfaces;

namespace Project.Classes {
    public abstract class Player {
        public bool myTurn;
        private bool _moveDone;
        // private IPlayerController _playerController;
        public Pawn Pawn { get; set; }
        public int NumOfWalls { get; private set; }

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