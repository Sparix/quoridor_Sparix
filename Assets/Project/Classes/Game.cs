using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Classes {
    public class Game {
        private bool _gameRunning;
        private Task _waitTask;
        private IEnumerator<Player> _playersEnumerator;
        public Field Field { get; private set; }
        public List<Player> Players { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public bool GameRunning {
            get => _gameRunning;
            private set {
                if (_gameRunning == value) {
                    return;
                }

                _gameRunning = value;
                if (_gameRunning) {
                    GameStarted?.Invoke();
                }
                else {
                    GameFinished?.Invoke();
                }
            }
        }

        public event Action GameStarted;
        public event Action GameFinished;
        public event Action<Player> GameFinishedWithWinner;
        public event Action OnNextTurn;

        public void AddHandlerForOnNextTurn(int index, Action newHandler) {
            OnNextTurn = OnNextTurn.AddHandlerOnIndex(index, newHandler);
        }

        public Game(int ySize, int xSize, List<Player> players) {
            if (players.Count < 2) {
                throw new ArgumentException("There are must be at least 2 players");
            }

            if (players.Count > 4) {
                throw new ArgumentException("There are must be no more than 4 players");
            }

            if (players.Count != 2) {
                throw new ArgumentException("Not 2 players variation not implemented yet");
            }

            if (xSize.IsEven() || ySize.IsEven()) {
                throw new ArgumentException("Field sizes must be odd");
            }

            Players = players;
            _playersEnumerator = Players.GetEnumerator();
            // todo add reset methods to event
            GameStarted += () => { };
            Field = new Field(xSize, ySize);
            var yLen = Field.FieldSpaces.GetLength(0);
            var xLen = Field.FieldSpaces.GetLength(1);
            var positions = new Point[4] {
                new Point(0, xLen / 2),
                new Point(yLen - 1, xLen / 2),
                new Point(yLen / 2, 0),
                new Point(yLen / 2, xLen - 1)
            };
            var winConditions = new Predicate<Point>[] {
                p => p.Y == yLen - 1,
                p => p.Y == 0,
                p => p.X == xLen - 1,
                p => p.X == 0,
            };

            for (var i = 0; i < Players.Count; i++) {
                Players[i].Pawn = new Pawn(positions[i], Players[i], Field, winConditions[i]);
                Field.TryAddPawn(Players[i].Pawn);
            }
        }


        public void Tick() {
            if (!GameRunning) return;

            if (_waitTask.IsCompleted && !IsThereWinner(out var winner)) {
                OnNextTurn?.Invoke();
                _waitTask = WaitForMove();
            }
        }

        private bool IsThereWinner(out Player winner) {
            winner = null;
            foreach (var player in Players) {
                if (player.Pawn.IsWinner) {
                    winner = player;
                    FinishGame(winner);
                    return true;
                }
            }

            return false;
        }

        private async Task WaitForMove() {
            CurrentPlayer.myTurn = true;
            await CurrentPlayer.MakeMove();
            CurrentPlayer.myTurn = false;
            CurrentPlayer = _playersEnumerator.GetNextCycled();
        }

        public void StartGame() {
            if (GameRunning) {
                throw new Exception("Game already going");
            }

            GameRunning = true;
            _playersEnumerator.Reset();
            CurrentPlayer = _playersEnumerator.GetNextCycled();
            OnNextTurn?.Invoke();
            _waitTask = WaitForMove();
        }

        private void FinishGame(Player player) {
            CancelGame();
            GameFinishedWithWinner?.Invoke(player);
        }

        public void CancelGame() {
            if (!GameRunning) {
                throw new Exception("Game isn't going");
            }

            GameRunning = false;
            _waitTask = null;
        }
    }
}