using System.Collections;
using Internal.Scripts.Core.BaseClasses;
using Internal.Scripts.Core.Utils.Cryptography;
using Internal.Scripts.Game.Gameplay.GameFieldObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class GlobalStateManager : MonoBehaviourSingleton<GlobalStateManager> {
        private readonly SafeLong _highScore = new SafeLong();
        private readonly SafeLong _score = new SafeLong();
        public UnityEvent GameOverEvent { get; } = new UnityEvent();
        public UnityEvent<bool> GameStartedEvent { get; } = new UnityEvent<bool>();
        public UnityEvent RestartEvent { get; } = new UnityEvent();
        public UnityEvent HomeEvent { get; } = new UnityEvent();
        public UnityEvent ScoreUpdatedEvent { get; } = new UnityEvent();
        public UnityEvent HighScoreUpdatedEvent { get; } = new UnityEvent();

        public GameField CurrentGameField { get; set; }
        public bool IsGameScene { get; set; }
        public long HighScore {
            get => _highScore.Value;
            set {
                _highScore.Value = value;
                HighScoreUpdatedEvent.Invoke();
            }
        }

        public long Score {
            get => _score.Value;
            set {
                _score.Value = value;
                if (value >= _highScore.Value) HighScore = value;
                ScoreUpdatedEvent.Invoke();
            }
        }

        private IEnumerator StartGameCoroutine(bool waitForSettingsLoad) {
            if (waitForSettingsLoad)
                yield return new WaitUntil(() => SettingsManager.Instance.IsSettingsLoaded);
            IsGameScene = true;
            GameStartedEvent.Invoke(SettingsManager.Instance.IsNewGame);
        }
        
        public void StartGame(bool waitForSettingsLoad = true) {
            if (IsGameScene) return;
            StartCoroutine(StartGameCoroutine(waitForSettingsLoad));
        }

        public void GameOver() {
            GameOverEvent.Invoke();
        }

        public void Home() {
            if (!IsGameScene) return;
            SaveAll();
            HomeEvent.Invoke();
            IsGameScene = false;
        }

        public void Restart() {
            RestartEvent.Invoke();
        }
        
        private void OnApplicationPause(bool pauseStatus) {
            SaveAll();
        }

        private void OnApplicationQuit() {
            SaveAll();
        }

        private void SaveAll() {
            if (IsGameScene)
                SaveLoadManager.Instance.SaveGameData(true);
            SaveLoadManager.Instance.SaveSettingsData(true);
        }
    }
}