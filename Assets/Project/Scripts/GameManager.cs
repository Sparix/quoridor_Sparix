using System;
using System.Collections.Generic;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class GameManager : MonoBehaviour {
        private bool _initialized;
        public Game Game { get; private set; }

        public bool Initialized {
            get => _initialized;
            private set {
                if (_initialized == value) {
                    return;
                }

                _initialized = value;
                if (_initialized) {
                    OnGameModeChosen?.Invoke();
                }
            }
        }

        public event Action OnGameModeChosen;

        public Field Field => Game.Field;
        public Player CurrentPlayer => Game.CurrentPlayer;

        private void Awake() {
            // CreatePlayerVsPlayer();
            // CreatePlayerVsBot();
        }

        public void CreatePlayerVsPlayer() {
            var players = new List<Player> {new LocalPlayer(), new LocalPlayer()};
            Game = new Game(Consts.DEFAULT_FIELD_SIZE_Y, Consts.DEFAULT_FIELD_SIZE_X, players);
            Game.StartGame();
            Initialized = true;
        }

        public void CreatePlayerVsBot(bool playerMoveFirst = true) {
            var players = playerMoveFirst
                ? new List<Player> {new LocalPlayer(), new StupidBot()}
                : new List<Player> {new StupidBot(), new LocalPlayer()};
            Game = new Game(Consts.DEFAULT_FIELD_SIZE_Y, Consts.DEFAULT_FIELD_SIZE_X, players);
            Initialized = true;
            Game.StartGame();
        }

        public void Update() {
            if (!Initialized) {
                return;
            }

            Game.Tick();
        }
    }
}