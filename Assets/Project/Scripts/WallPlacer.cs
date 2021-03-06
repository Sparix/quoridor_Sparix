using System;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
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
        private Wall _selectedWall;
        private Renderer _selectedWallRenderer;
        private Wall.Type _type;
        private PlaceForWall _closestPlace;

        public bool PlacingWall { get; private set; }

        private void Awake() {
            CheckInitialization();
            SubscribeToControllerEvents();
            _boardPlane = new Plane(Vector3.up,
                places.Length > 0 ? places[0].transform.position : transform.position);
        }

        private void CheckInitialization() {
            camera ??= Camera.main;
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            controller ??= FindObjectOfType<WallPlacerController>();
            places ??= FindObjectsOfType<PlaceForWall>();
        }
        
        private void SubscribeToControllerEvents() {
            controller.OnVerticalWallKeyDown += () => SelectWall(Wall.Type.Vertical);
            controller.OnHorizontalWallKeyDown += () => SelectWall(Wall.Type.Horizontal);
            controller.OnTrySetWallKeyDown += TrySetWall;
        }

        private void Update() {
            if (!gameManager.Game.GameRunning) {
                return;
            }

            if (_selectedWallGO == null) {
                return;
            }

            UpdateSelectedWallPos();
        }

        private void TrySetWall() {
            if (_selectedWall == null) return;
            if (gameManager.CurrentPlayer.TrySetWall(new Wall(_closestPlace.GetY, _closestPlace.GetX, _type))) {
                // if (_selectedWall != null && gameManager.Field.TrySetWall(_selectedWall)) {
                _selectedWallGO.transform.parent = _closestPlace.gameObject.transform;
                _selectedWallRenderer.material.color = Color.white;
                _selectedWallGO = null;
                PlacingWall = false;
            }
            else {
                DestroySelectedWall();
            }
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
                    _selectedWall = new Wall(_closestPlace.GetY, _closestPlace.GetX, _type);
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

        public void SelectWall(Wall.Type type) {
            if (_selectedWallGO != null) {
                DestroySelectedWall();
                if (_type == type) {
                    return;
                }
            }

            PlacingWall = true;
            _type = type;
            _selectedWallGO = type switch {
                Wall.Type.Horizontal => Instantiate(horizontalWall),
                Wall.Type.Vertical => Instantiate(verticalWall),
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
            if (available) {
                _selectedWallRenderer.material.color = Color.green;
            }
            else {
                _selectedWallRenderer.material.color = Color.red;
            }
        }
    }
}