using System.Threading.Tasks;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class PlaceForPawn : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private WallPlacer wallPlacer;
        [SerializeField] private MeshRenderer meshRenderer;
        [Range(0, 1f)] [SerializeField] private float radius = 0.01f;
        [SerializeField] private int y;
        [SerializeField] private int x;
        private int _highlightCounter = 0;
        private Task _highlightTask;
        public int GetY => y;
        public int GetX => x;

        private void Start() {
            if (gameManager == null) {
                gameManager = GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            }

            if (wallPlacer == null) {
                wallPlacer = FindObjectOfType<WallPlacer>();
            }

            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, radius);
        }

        // todo move to Controller
        private void OnMouseDown() {
            // todo may not working on mobile 
            // Debug.Log($"Click on \ny = {y}\nx = {x}");\
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
            // void OnGameFinished() => nextTurn = true;
            gameManager.Game.OnNextTurn += OnNextTurn;
            // gameManager.Game.GameFinished += OnGameFinished;
            while (_highlightCounter > 1) {
                await Task.Yield();
            }

            meshRenderer.enabled = true;
            while (!nextTurn) {
                await Task.Yield();
            }

            gameManager.Game.OnNextTurn -= OnNextTurn;
            // gameManager.Game.GameFinished -= OnGameFinished;
            _highlightCounter--;
            meshRenderer.enabled = false;
        }
    }
}