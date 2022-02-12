using Project.Classes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Project.Scripts {
    public class UIManager : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TextMeshProUGUI numOfWallsP1;
        [SerializeField] private TextMeshProUGUI numOfWallsP2;
        [SerializeField] private Button restartButton;

        private Player p1;
        private Player p2;

        private /*async*/ void Start() {
            DeactivateRestartButton();

            if (gameManager == null) {
                gameManager = GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
            }

            // while (gameManager.Game == null) {
                // await Task.Yield();
            // }

            var players = gameManager.Game.Players;
            p1 = players[0];
            p2 = players[1];
            UpdateNumOfWallsP1();
            UpdateNumOfWallsP2();
            p1.NumOfWallsChanged += UpdateNumOfWallsP1;
            p2.NumOfWallsChanged += UpdateNumOfWallsP2;
            
            restartButton.onClick.AddListener(RestartGame);
            gameManager.Game.GameStarted += DeactivateRestartButton;
            gameManager.Game.GameFinished += ActivateRestartButton;
        }

        private void UpdateNumOfWallsText(Player player, TMP_Text textComponent) {
            textComponent.text = player.NumOfWalls.ToString();
        }

        private void UpdateNumOfWallsP1() {
            UpdateNumOfWallsText(p1, numOfWallsP1);
        }

        private void UpdateNumOfWallsP2() {
            UpdateNumOfWallsText(p2, numOfWallsP2);
        }

        private void RestartGame() {
            // todo change with normal method
            SceneManager.LoadScene(0);
        }

        private void ActivateRestartButton() {
            restartButton.gameObject.SetActive(true);
        }

        private void DeactivateRestartButton() {
            restartButton.gameObject.SetActive(false);
        }
    }
}