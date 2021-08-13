using System;
using Internal.Scripts.Core.ScriptableObjects.Base;
using Internal.Scripts.Game.Gameplay.GameField;
using UnityEngine;

namespace Internal.Scripts.Core.ScriptableObjects.DataObjects.GlobalControl {
    [CreateAssetMenu(fileName = "GlobalConfigData", menuName = "ScriptableObjects/GlobalConfigData", order = 1)]
    public partial class GlobalControl : ScriptableObjectSingleton<GlobalControl> {
        private bool _isPause = false;

        public bool IsPause {
            get => _isPause;
            set {
                _isPause = value;
                if (value) {
                    GamePausedEvent(this, EventArgs.Empty);
                } else {
                    GameUnpausedEvent(this, EventArgs.Empty);
                }
            }
        }

        [SerializeField] private ColorSchemeObject colorScheme;
        public ColorSchemeObject ColorScheme => colorScheme;

        public void PauseGame() {
            IsPause = true;
        }

        public void UnpauseGame() {
            IsPause = false;
        }

        public void StartGame(bool isNewGame) {
            GameStartEvent(this, new StartEventArgs {IsNewGame = isNewGame});
            _isPause = false;
        }
        
        public void GameOver() {
            GameOverEvent(this, EventArgs.Empty);
        }

        private void OnBallsStopped(object sender, EventArgs args) {
            GameOver();
        }
        
        private void Awake() {
            GameField.BallPlaceNotFoundEvent += OnBallsStopped;
        }

        private void OnDestroy() {
            GameField.BallPlaceNotFoundEvent -= OnBallsStopped;
        }
    }
}