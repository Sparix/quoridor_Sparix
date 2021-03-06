using System;
using System.Collections.Generic;
using System.Linq;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class PawnPlacer : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private List<GameObject> playersPawns;
        [SerializeField] private PlaceForPawn[] places;
        private GameObject _selectedWallGO;
        private Wall _selectedWall;
        private Renderer _selectedWallRenderer;
        private Wall.Type _type;
        private PlaceForWall _closestPlace;
        private List<Pawn> _pawns;

        private void Start() {
            CheckInitialization();
            _pawns = gameManager.Game.Players.Select(p => p.Pawn).ToList();
            var updatePositions = new Action[] {
                () => UpdatePosition(0),
                () => UpdatePosition(1),
                () => UpdatePosition(2),
                () => UpdatePosition(3)
            };
            for (var i = 0; i < _pawns.Count; i++) {
                updatePositions[i].Invoke();
                _pawns[i].PosChanged += updatePositions[i];
            }

            Highlight();
            gameManager.Game.OnNextTurn += Highlight;
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            places ??= FindObjectsOfType<PlaceForPawn>();
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