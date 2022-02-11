using System;
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

        private int p1W;
        private int p2W;

        public void Update() {
            Game.Tick();
            // Debug.Log($"Cur ind = {Game.Players.IndexOf(Game.CurrentPlayer)}");
            // if (p1W != Game.Players[0].NumOfWalls) {
            //     Debug.Log($"player 1: {Game.Players[0].NumOfWalls}");
            //     p1W = Game.Players[0].NumOfWalls;
            // }
            //
            // if (p2W != Game.Players[1].NumOfWalls) {
            //     Debug.Log($"player 2: {Game.Players[1].NumOfWalls}");
            //     p2W = Game.Players[1].NumOfWalls;
            // }
            //
            // Debug.Log($"player 2: {Game.Players[1].Pawn.Pos}");
        }
    }
}