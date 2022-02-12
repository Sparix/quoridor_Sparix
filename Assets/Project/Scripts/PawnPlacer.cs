using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Project.Classes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts {
    public class PawnPlacer : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private List<GameObject> playersPawns;
        private PlaceForPawn[] _places;
        private GameObject _selectedWallGO;
        private Wall _selectedWall;
        private Renderer _selectedWallRenderer;
        private Wall.Type _type;
        private PlaceForWall _closestPlace;
        private List<Pawn> _pawns;

        private void Start() {
            if (gameManager == null) {
                gameManager = GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            }

            _places = FindObjectsOfType<PlaceForPawn>();
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

        private void UpdatePosition(int i) {
            var pos = _pawns[i].Pos;
            playersPawns[i].transform.position =
                _places.First(place => place.GetX == pos.X && place.GetY == pos.Y).transform.position;
        }

        private void Highlight() {
            var curPlayer = gameManager.CurrentPlayer;
            if (!(curPlayer is LocalPlayer)) {
                return;
            }
            var dirs = curPlayer.Pawn.GetPossibleDirections();
            foreach (var place in _places) {
                foreach (var dir in dirs) {
                    if (dir.Y == place.GetY && dir.X == place.GetX) {
                        place.Highlight();
                    }
                }
            }
        }
        
        // private void Update() {
            // Highlight();
        // }
    }
}