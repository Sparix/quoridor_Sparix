using Project.Classes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI {
    public class BeforeGameUI : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Button humanVsHumanButton;
        [SerializeField] private Button humanVsBotButton;
        [SerializeField] private Button moveFirstButton;
        [SerializeField] private Button moveSecondButton;
        
        private void Awake() {
            CheckInitialization();
            Activate();
            DeactivateMoveOrderChose();
            Subscribe();
            AddListenersForButtons();
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void Subscribe() {
            gameManager.OnGameModeChosen += Deactivate;
            // todo gameManager. += Activate();
        }

        private void Unsubscribe() {
            gameManager.OnGameModeChosen -= Deactivate;
        }

        private void ActivateHumanVsHumanButton() => ActivateGameObject(humanVsHumanButton.gameObject);
        private void DeactivateHumanVsHumanButton() => DeactivateGameObject(humanVsHumanButton.gameObject);

        private void ActivateHumanVsBotButton() => ActivateGameObject(humanVsBotButton.gameObject);
        private void DeactivateHumanVsBotButton() => DeactivateGameObject(humanVsBotButton.gameObject);

        private void ActivateMoveFirstButton() => ActivateGameObject(moveFirstButton.gameObject);
        private void DeactivateMoveFirstButton() => DeactivateGameObject(moveFirstButton.gameObject);

        private void ActivateMoveSecondButton() => ActivateGameObject(moveSecondButton.gameObject);
        private void DeactivateMoveSecondButton() => DeactivateGameObject(moveSecondButton.gameObject);

        private void AddListenersForButtons() {
            AddListener(humanVsHumanButton, CreateHumanVsHuman);
            AddListener(humanVsBotButton, ActivateMoveChose);
            AddListener(moveFirstButton, CreateHumanVsBotHumanFirst);
            AddListener(moveSecondButton, CreateHumanVsBotBotFirst);
        }

        private void AddListener(Button button, UnityAction listener) => button.onClick.AddListener(listener);

        private void ActivateGameObject(GameObject go) => go.SetActive(true);
        private void DeactivateGameObject(GameObject go) => go.SetActive(false);

        private void Activate() {
            ActivateHumanVsHumanButton();
            ActivateHumanVsBotButton();
        }

        private void Deactivate() {
            DeactivateHumanVsHumanButton();
            DeactivateHumanVsBotButton();
            DeactivateMoveOrderChose();
        }

        private void DeactivateMoveOrderChose() {
            DeactivateMoveFirstButton();
            DeactivateMoveSecondButton();
        }

        private void CreateHumanVsBotHumanFirst() => gameManager.CreatePlayerVsBot(true);

        private void CreateHumanVsBotBotFirst() => gameManager.CreatePlayerVsBot(false);

        private void CreateHumanVsHuman() {
            gameManager.CreatePlayerVsPlayer();
        }

        private void ActivateMoveChose() {
            ActivateMoveFirstButton();
            ActivateMoveSecondButton();
        }
    }
}