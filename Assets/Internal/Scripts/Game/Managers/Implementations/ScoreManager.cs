using System;
using TMPro;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class ScoreManager : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private TextMeshProUGUI highScore;

        private void Start() {
            OnScoreChanged();
            OnHighScoreChanged();
            GlobalStateManager.Instance.ScoreUpdatedEvent.AddListener(OnScoreChanged);
            GlobalStateManager.Instance.HighScoreUpdatedEvent.AddListener(OnHighScoreChanged);
        }

        private void OnHighScoreChanged() {
            highScore.text = GlobalStateManager.Instance.HighScore.ToString();
        }

        private void OnScoreChanged() {
            score.text = GlobalStateManager.Instance.Score.ToString();
        }
    }
}
