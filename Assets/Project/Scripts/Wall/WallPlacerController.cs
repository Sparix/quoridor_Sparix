using System;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts.Wall {
    public class WallPlacerController : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private KeyCode verticalKeyCode = KeyCode.Alpha1;
        [SerializeField] private KeyCode horizontalKeyCode = KeyCode.Alpha2;
        [SerializeField] private KeyCode trySetWallKeyCode = KeyCode.Mouse0;

        public event Action OnVerticalWallKeyDown;
        public event Action OnHorizontalWallKeyDown;
        public event Action OnTrySetWallKeyDown;

        private void Awake() {
            CheckInitialization();
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void Update() {
            CheckForInput();
        }

        private void CheckForInput() {
            CheckForKeyDown(OnVerticalWallKeyDown, verticalKeyCode);
            CheckForKeyDown(OnHorizontalWallKeyDown, horizontalKeyCode);
            CheckForKeyDown(OnTrySetWallKeyDown, trySetWallKeyCode);
        }

        private void CheckForKeyDown(Action keyDownEvent, KeyCode keyCode) {
            if (!gameManager.Initialized) {
                return;
            }

            if (Input.GetKeyDown(keyCode)) {
                keyDownEvent?.Invoke();
            }
        }
    }
}