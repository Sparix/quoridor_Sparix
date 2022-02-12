using System;
using System.Threading;
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

            // meshRenderer.enabled = false;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, radius);
        }

        // todo to Controller
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

        // public IEnumerator Highlight2() {
        //     meshRenderer.enabled = true;
        //     var nextTurn = false;
        //     gameManager.Game.OnNextTurn += () => nextTurn = true;
        //     yield return new WaitWhile(() => nextTurn);
        //     // while (!nextTurn) {
        //     // await Task.Yield();
        //     // }
        //
        //     meshRenderer.enabled = false;
        // }

        public async void Highlight1() {
            Debug.Log("Start");
            var nextTurn = false;
            Action trigger = () => nextTurn = true;
            gameManager.Game.AddHandlerForOnNextTurn(0, trigger);

            await Task.CompletedTask;
            meshRenderer.enabled = true;

            // gameManager.Game.OnNextTurn += () => nextTurn = true;
            while (!nextTurn) {
                // meshRenderer.enabled = true;
                await Task.Yield();
            }

            Debug.Log("Finish");
            gameManager.Game.OnNextTurn -= trigger;
            meshRenderer.enabled = false;
        }

        private async void Highlight4() {
            Debug.Log("Start");
            meshRenderer.enabled = true;
            var nextTurn = false;
            void Trigger() => nextTurn = true;
            gameManager.Game.AddHandlerForOnNextTurn(0, Trigger);
            while (!nextTurn) {
                await Task.Yield();
            }

            Debug.Log("Finish");
            gameManager.Game.OnNextTurn -= Trigger;
            meshRenderer.enabled = false;
        }


        public async void Highlight() {
            Debug.Log("Start");
            _highlightCounter++;
            var nextTurn = false;
            void Trigger() => nextTurn = true;
            gameManager.Game.AddHandlerForOnNextTurn(0, Trigger);
            while (_highlightCounter > 1) {
                await Task.Yield();
            }

            meshRenderer.enabled = true;
            while (!nextTurn) {
                await Task.Yield();
            }

            Debug.Log("Finish");
            gameManager.Game.OnNextTurn -= Trigger;
            _highlightCounter--;
            meshRenderer.enabled = false;
        }

        public async void Highlight3() {
            if (_highlightTask != null) {
                await Task.WhenAny(_highlightTask);
            }

            _highlightTask = Task.Run(Highlight4);
        }

        public async void Highlight(CancellationToken ct) {
            Debug.Log("Start");
            meshRenderer.enabled = true;
            var nextTurn = false;
            Action trigger = () => nextTurn = true;
            gameManager.Game.OnNextTurn += trigger;
            while (!nextTurn) {
                // meshRenderer.enabled = true;
                if (ct.IsCancellationRequested) {
                    // meshRenderer.enabled = false;
                    gameManager.Game.OnNextTurn -= trigger;
                    Debug.Log("Cancel");
                    return;
                }

                await Task.Yield();
            }

            gameManager.Game.OnNextTurn -= trigger;
            meshRenderer.enabled = false;
            Debug.Log("Finish");
        }

        // public void Highlight() {
        // meshRenderer.enabled = true;
        // }

        // public void DisableHighlight() {
        // meshRenderer.enabled = false;
    }
} 