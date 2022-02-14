using Project.Classes;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI {
    public class InGameUI : MonoBehaviour {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TextMeshProUGUI numOfWallsP1;
        [SerializeField] private TextMeshProUGUI numOfWallsP2;
        private Player _p1;
        private Player _p2;
        private bool _firstMatch = true;

        private void Awake() {
            CheckInitialization();
            Deactivate();
            gameManager.OnGameModeChosen += OnGameModeChosen;
        }

        private void CheckInitialization() {
            gameManager ??= GameObject.FindGameObjectWithTag(Consts.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void OnGameModeChosen() {
            Unsubscribe();
            FillPlayers();
            UpdateNumOfWallsP1();
            UpdateNumOfWallsP2();
            Subscribe();
            _firstMatch = false;
        }

        private void Subscribe() {
            _p1.NumOfWallsChanged += UpdateNumOfWallsP1;
            _p2.NumOfWallsChanged += UpdateNumOfWallsP2;
            gameManager.Game.GameStarted += Activate;
            gameManager.Game.GameFinished += Deactivate;
        }

        private void Unsubscribe() {
            if (_firstMatch) {
                return;
            }

            _p1.NumOfWallsChanged -= UpdateNumOfWallsP1;
            _p2.NumOfWallsChanged -= UpdateNumOfWallsP2;
            gameManager.Game.GameStarted -= Activate;
            gameManager.Game.GameFinished -= Deactivate;
        }

        private void FillPlayers() {
            _p1 = gameManager.Game.Players[0];
            _p2 = gameManager.Game.Players[1];
        }


        private void UpdateNumOfWallsText(Player player, TMP_Text textComponent) {
            textComponent.text = player.NumOfWalls.ToString();
        }

        private void UpdateNumOfWallsP1() {
            UpdateNumOfWallsText(_p1, numOfWallsP1);
        }

        private void UpdateNumOfWallsP2() {
            UpdateNumOfWallsText(_p2, numOfWallsP2);
        }

        private void ActivateGameObject(GameObject go) => go.SetActive(true);
        private void DeactivateGameObject(GameObject go) => go.SetActive(false);

        private void Activate() {
            ActivateNumOfWallsGOs();
        }

        private void Deactivate() {
            DeactivateNumOfWallsGOs();
        }

        private void ActivateNumOfWallsGOs() {
            ActivateGameObject(numOfWallsP1.gameObject);
            ActivateGameObject(numOfWallsP2.gameObject);
        }

        private void DeactivateNumOfWallsGOs() {
            DeactivateGameObject(numOfWallsP1.gameObject);
            DeactivateGameObject(numOfWallsP2.gameObject);
        }
    }
}