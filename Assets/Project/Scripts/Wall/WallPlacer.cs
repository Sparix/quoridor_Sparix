using System;
using System.Linq;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts.Wall {
    public class WallPlacer : MonoBehaviour {
        [SerializeField] private Camera camera;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private WallPlacerController controller;
        [SerializeField] private GameObject horizontalWall;
        [SerializeField] private GameObject verticalWall;
        [SerializeField] private float maximumMagnetRange = 0.3f;
        [SerializeField] private PlaceForWall[] places;
        private Plane _boardPlane;
        private GameObject _selectedWallGO;
        private Classes.Field.Wall _selectedWall;
        private Renderer _selectedWallRenderer;
        private Classes.Field.Wall.Type _type;
        private PlaceForWall _closestPlace;

        public bool PlacingWall { get; private set; }

        private void Awake() {
            CheckInitialization();
            Subscribe();
            InitBoardPlane();
            gameManager.OnGameModeChosen += OnGameMoveChosen;
        }

        private void OnGameMoveChosen() {
            gameManager.Game.Field.OnWallPlaced += ShowWall;
        }

        private void CheckInitialization() {
            camera ??= Camera.main;
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            controller ??= FindObjectOfType<WallPlacerController>();
            places ??= FindObjectsOfType<PlaceForWall>();
        }

        private void Subscribe() {
            SubscribeToControllerEvents();
        }

        private void Unsubscribe() {
            UnsubscribeFromControllerEvents();
        }

        private void ShowWall(Classes.Field.Wall wall) {
            var wallPlace = places.First(place => place.GetY == wall.Y && place.GetX == wall.X);
            if (wall.WallType == Classes.Field.Wall.Type.Horizontal) {
                wallPlace.ActivateHorizontalWall();
            }
            else {
                wallPlace.ActivateVerticalWall();
            }
        }

        private void InitBoardPlane() {
            _boardPlane = new Plane(Vector3.up,
                places.Length > 0 ? places[0].transform.position : transform.position);
        }

        private void SubscribeToControllerEvents() {
            controller.OnVerticalWallKeyDown += SelectVerticalWall;
            controller.OnHorizontalWallKeyDown += SelectHorizontalWall;
            controller.OnTrySetWallKeyDown += TrySetWall;
        }

        private void UnsubscribeFromControllerEvents() {
            controller.OnVerticalWallKeyDown -= SelectVerticalWall;
            controller.OnHorizontalWallKeyDown -= SelectHorizontalWall;
            controller.OnTrySetWallKeyDown -= TrySetWall;
        }

        private void SelectVerticalWall() {
            SelectWall(Classes.Field.Wall.Type.Vertical);
        }

        private void SelectHorizontalWall() {
            SelectWall(Classes.Field.Wall.Type.Horizontal);
        }

        private void Update() {
            if (!gameManager.Initialized) {
                return;
            }

            if (_selectedWallGO == null) {
                return;
            }

            UpdateSelectedWallPos();
        }

        private void TrySetWall() {
            if (_selectedWall == null) return;
            if (gameManager.CurrentPlayer.TrySetWall(new Classes.Field.Wall(_closestPlace.GetY, _closestPlace.GetX,
                _type))) {
                // if (_selectedWall != null && gameManager.Field.TrySetWall(_selectedWall)) {
                _selectedWallGO.transform.parent = _closestPlace.gameObject.transform;
                _selectedWallRenderer.material.color = Color.white;
            }

            DestroySelectedWall();
        }

        private void UpdateSelectedWallPos() {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (_boardPlane.Raycast(ray, out var position)) {
                var point = ray.GetPoint(position);
                var closestDistance = float.PositiveInfinity;
                foreach (var place in places) {
                    var dist = Vector3.Distance(point, place.transform.position);
                    if (dist < closestDistance) {
                        closestDistance = dist;
                        _closestPlace = place;
                    }
                }

                if (closestDistance < maximumMagnetRange) {
                    _selectedWallGO.transform.position = _closestPlace.transform.position;
                    _selectedWall = new Classes.Field.Wall(_closestPlace.GetY, _closestPlace.GetX, _type);
                    ChangeWallOnAvailability(gameManager.Field.CanSetWall(_selectedWall));
                }
                else {
                    _selectedWallGO.transform.position = point;
                    _selectedWall = null;
                    _closestPlace = null;
                    ChangeWallOnAvailability(false);
                }
            }
        }

        public void SelectWall(Classes.Field.Wall.Type type) {
            if (_selectedWallGO != null) {
                DestroySelectedWall();
                if (_type == type) {
                    return;
                }
            }

            PlacingWall = true;
            _type = type;
            _selectedWallGO = type switch {
                Classes.Field.Wall.Type.Horizontal => Instantiate(horizontalWall),
                Classes.Field.Wall.Type.Vertical => Instantiate(verticalWall),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Wrong type")
            };
            _selectedWallRenderer = _selectedWallGO.GetComponent<Renderer>();
        }

        private void DestroySelectedWall() {
            Destroy(_selectedWallGO);
            _selectedWallGO = null;
            PlacingWall = false;
        }

        private void ChangeWallOnAvailability(bool available) {
            _selectedWallRenderer.material.color = available ? Color.green : Color.red;
        }
    }
}