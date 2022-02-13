using System;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class PlaceForPawnController : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        public event Action OnClick;

        private void Awake() {
            CheckInitialization();
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void OnMouseDown() {
            if (!gameManager.Initialized) {
                return;
            }

            // todo may not working on mobile 
            OnClick?.Invoke();
        }
    }
}