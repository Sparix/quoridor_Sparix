using System.Collections.Generic;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class GameManager : MonoBehaviour {
        public Game Game { get; private set; }

        public Field Field => Game.Field;
        public Player CurrentPlayer => Game.CurrentPlayer;

        private void Awake() {
            // CreatePlayerVsPlayer();
            CreatePlayerVsBot();
        }

        public void CreatePlayerVsPlayer() {
            var players = new List<Player> {new LocalPlayer(), new LocalPlayer()};
            Game = new Game(Consts.DEFAULT_FIELD_SIZE_Y, Consts.DEFAULT_FIELD_SIZE_X, players);
            Game.StartGame();
        }

        public void CreatePlayerVsBot() {
            var players = new List<Player> {new LocalPlayer(), new StupidBot()};
            Game = new Game(Consts.DEFAULT_FIELD_SIZE_Y, Consts.DEFAULT_FIELD_SIZE_X, players);
            Game.StartGame();
        }

        public void Update() {
            Game.Tick();
        }
    }
}