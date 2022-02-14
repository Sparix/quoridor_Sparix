using System;
using System.Collections.Generic;
using System.Linq;
using Project.Classes;
using Project.Classes.Player;
using UnityEngine;

namespace Project.Scripts.Pawn {
    public class PawnPlacer : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private List<GameObject> playersPawns;
        [SerializeField] private PlaceForPawn[] places;
        private List<Classes.Field.Pawn> _pawns = new List<Classes.Field.Pawn>();

        private Action[] _updatePositions;

        private void Start() {
            CheckInitialization();
            FillUpdatePositions();
            gameManager.OnGameModeChosen += OnGameMoveChosen; // todo try replace with Game.GameStart
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            places ??= FindObjectsOfType<PlaceForPawn>();
        }

        private void FillUpdatePositions() {
            _updatePositions = new Action[] {
                () => UpdatePosition(0),
                () => UpdatePosition(1),
                () => UpdatePosition(2),
                () => UpdatePosition(3)
            };
        }

        private void OnGameMoveChosen() {
            Unsubscribe();
            _pawns = gameManager.Game.Players.Select(p => p.Pawn).ToList();
            Highlight();
            Subscribe();
        }

        private void Subscribe() {
            for (var i = 0; i < _pawns.Count; i++) {
                _updatePositions[i].Invoke();
                _pawns[i].PosChanged += _updatePositions[i];
            }

            gameManager.Game.OnNextTurn += Highlight;
        }

        private void Unsubscribe() {
            for (var i = 0; i < _pawns.Count; i++) {
                _updatePositions[i].Invoke();
                _pawns[i].PosChanged -= _updatePositions[i];
            }

            gameManager.Game.OnNextTurn -= Highlight;
        }


        private void UpdatePosition(int i) {
            var pos = _pawns[i].Pos;
            playersPawns[i].transform.position =
                places.First(place => place.GetX == pos.X && place.GetY == pos.Y).transform.position;
        }

        private void Highlight() {
            if (!gameManager.Game.GameRunning) {
                return;
            }

            var curPlayer = gameManager.CurrentPlayer;
            if (!(curPlayer is LocalPlayer)) {
                return;
            }

            var dirs = curPlayer.Pawn.GetPossibleDirections();
            foreach (var place in places) {
                foreach (var dir in dirs) {
                    if (dir.Y == place.GetY && dir.X == place.GetX) {
                        place.Highlight();
                    }
                }
            }
        }
    }
}