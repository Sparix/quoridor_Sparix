using System.Threading.Tasks;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class PlaceForPawn : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private WallPlacer wallPlacer;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private PlaceForPawnController controller;
        [Range(0, 1f)] [SerializeField] private float radius = 0.01f;
        [SerializeField] private int y;
        [SerializeField] private int x;
        private int _highlightCounter = 0;
        public int GetY => y;
        public int GetX => x;

        private void Start() {
            CheckInitialization();
            controller.OnClick += TryMove;
        }

        private void CheckInitialization() {
            controller ??= GetComponent<PlaceForPawnController>();
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            wallPlacer ??= FindObjectOfType<WallPlacer>();
            meshRenderer ??= GetComponent<MeshRenderer>();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, radius);
        }

        private void TryMove() {
            if (wallPlacer.PlacingWall) {
                return;
            }

            var curPlayer = gameManager.CurrentPlayer;
            if (curPlayer is LocalPlayer) {
                curPlayer.TryMovePawn(new Point(y, x));
            }
        }

        public async void Highlight() {
            _highlightCounter++;
            var nextTurn = false;
            void OnNextTurn() => nextTurn = true;
            void OnGameFinished() => nextTurn = true;
            gameManager.Game.OnNextTurn += OnNextTurn;
            gameManager.Game.GameFinished += OnGameFinished;
            while (_highlightCounter > 1) {
                await Task.Yield();
            }

            meshRenderer.enabled = true;
            while (!nextTurn) {
                await Task.Yield();
            }

            gameManager.Game.OnNextTurn -= OnNextTurn;
            gameManager.Game.GameFinished -= OnGameFinished;
            _highlightCounter--;
            meshRenderer.enabled = false;
        }
    }
}