using System;
using UnityEngine;

namespace Project.Scripts {
    public class WallPlacerController : MonoBehaviour {
        [SerializeField] private KeyCode verticalKeyCode = KeyCode.Alpha1;
        [SerializeField] private KeyCode horizontalKeyCode = KeyCode.Alpha2;
        [SerializeField] private KeyCode trySetWallKeyCode = KeyCode.Mouse0;
        
        public event Action OnVerticalWallKeyDown;
        public event Action OnHorizontalWallKeyDown;
        public event Action OnTrySetWallKeyDown;

        private void Update() {
            CheckForInput();
        }

        private void CheckForInput() {
            CheckForKeyDown(OnVerticalWallKeyDown, verticalKeyCode);
            CheckForKeyDown(OnHorizontalWallKeyDown, horizontalKeyCode);
            CheckForKeyDown(OnTrySetWallKeyDown, trySetWallKeyCode);
        }

        private void CheckForKeyDown(Action keyDownEvent, KeyCode keyCode) {
            if (Input.GetKeyDown(keyCode)) {
                keyDownEvent?.Invoke();
            }
        }
    }
}