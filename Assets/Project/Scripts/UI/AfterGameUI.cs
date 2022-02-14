using Project.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.UI {
    public class AfterGameUI : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Button restartButton;
        [SerializeField] private TextMeshProUGUI winnerText;
        
        private void Awake() {
            CheckInitialization();
            Deactivate();
            AddListenersForButtons();
            gameManager.OnGameModeChosen += Subscribe;
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void Subscribe() {
            // todo when Deactivate restart button?
            gameManager.Game.GameFinishedWithWinner += Activate;
        }

        private void Unsubscribe() {
            gameManager.Game.GameFinishedWithWinner -= Activate;
        }
        
        private void AddListenersForButtons() {
            AddListener(restartButton, RestartGame);
        }

        private void AddListener(Button button, UnityAction listener) => button.onClick.AddListener(listener);

        private void ActivateGameObject(GameObject go) => go.SetActive(true);
        private void DeactivateGameObject(GameObject go) => go.SetActive(false);
        
        private void Activate(Player winner) {
            ActivateRestartButton();
            ActivateWinnerText(winner);
        }

        private void Deactivate() {
            DeactivateRestartButton();
            DeactivateWinnerText();
        }

        private void ActivateRestartButton() => ActivateGameObject(restartButton.gameObject);
        private void DeactivateRestartButton() => DeactivateGameObject(restartButton.gameObject);

        private void ActivateWinnerText(Player winner) {
            winnerText.text = Consts.DEFAULT_WINNER_CONGRATULATION + (winner.Pawn.PawnPriority == Pawn.Priority.First ? "RED" : "GREEN");
            ActivateGameObject(winnerText.gameObject);
        }

        private void DeactivateWinnerText() => DeactivateGameObject(winnerText.gameObject);
       
        private void RestartGame() {
            // todo change with normal method
            SceneManager.LoadScene(0);
        }
    }
}