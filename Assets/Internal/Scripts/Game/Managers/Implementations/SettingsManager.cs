using Internal.Scripts.Core.BaseClasses;
using Internal.Scripts.Game.DataObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class SettingsManager : MonoBehaviourSingleton<SettingsManager> {
        [SerializeField] private ColorSchemeObject colorScheme;
        [SerializeField] private AudioSchemeObject audioScheme;
        private bool _isMuted;
        private bool _isAdsRemoved;
        public UnityEvent MuteSettingChangedEvent { get; } = new UnityEvent();
        public UnityEvent AdsSettingChangedEvent { get; } = new UnityEvent();

        public bool IsMuted {
            get => _isMuted;
            set {
                _isMuted = value;
                MuteSettingChangedEvent.Invoke();
            } 
        }

        public bool IsNewGame { get; set; } = true;

        public bool IsAdsRemoved {
            get => _isAdsRemoved;
            set {
                _isAdsRemoved = value;
                AdsSettingChangedEvent.Invoke();
            }
        }

        public bool IsSettingsLoaded { get; private set; }
        // public bool IsProductsVerified { get; private set; }
        public ColorSchemeObject ColorScheme {
            get => colorScheme;
            set => colorScheme = value;
        }

        public AudioSchemeObject AudioScheme {
            get => audioScheme;
            set => audioScheme = value;
        }

        protected override void SingletonAwakened() {
            IsSettingsLoaded = false;
            SaveLoadManager.Instance.LoadSettingsData(data => {
                IsMuted = data.mute;
                IsNewGame = data.isNewGame;
                IsSettingsLoaded = true;
                IsAdsRemoved = data.isAdsRemoved;
            });
            // IsProductsVerified = true;
        }
    }
}