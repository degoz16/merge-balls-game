using System;
using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class SceneChangeManager : MonoBehaviour {
        [SerializeField] private List<GameObject> mainMenuGameObjects = new List<GameObject>();
        [SerializeField] private List<GameObject> gameSceneGameObjects = new List<GameObject>();
        [SerializeField] private List<GameObject> gameOverGameObjects = new List<GameObject>();

        private void Awake() {
            GlobalStateManager.Instance.GameOverEvent.AddListener(() => SetGameOverPanelActive(true));
            GlobalStateManager.Instance.HomeEvent.AddListener(ToMainMenu);
            GlobalStateManager.Instance.GameStartedEvent.AddListener(isNewGame => ToGame());
            GlobalStateManager.Instance.RestartEvent.AddListener(() => SetGameOverPanelActive(false));
        }

        private void Start() {
            ToMainMenu();
        }

        private void ToMainMenu() {
            gameSceneGameObjects.ForEach(go => go.SetActive(false));
            mainMenuGameObjects.ForEach(go => go.SetActive(true));
            SetGameOverPanelActive(false);
        }

        private void ToGame() {
            gameSceneGameObjects.ForEach(go => go.SetActive(true));
            mainMenuGameObjects.ForEach(go => go.SetActive(false));
        }

        private void SetGameOverPanelActive(bool status) {
            gameOverGameObjects.ForEach(go => go.SetActive(status));
        }
    }
}