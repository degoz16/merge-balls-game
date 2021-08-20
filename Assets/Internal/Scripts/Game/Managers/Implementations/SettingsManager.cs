using Internal.Scripts.Core.BaseClasses;
using Internal.Scripts.Game.DataObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class SettingsManager : MonoBehaviorSingleton<SettingsManager> {
        [SerializeField] private ColorSchemeObject colorScheme;
        [SerializeField] private AudioSchemeObject audioScheme;
        private bool _isMuted;
        public UnityEvent MuteSettingChangedEvent { get; } = new UnityEvent();

        public bool IsMuted {
            get => _isMuted;
            set {
                _isMuted = value;
                MuteSettingChangedEvent.Invoke();
            } 
        }

        public ColorSchemeObject ColorScheme {
            get => colorScheme;
            set => colorScheme = value;
        }

        public AudioSchemeObject AudioScheme {
            get => audioScheme;
            set => audioScheme = value;
        }

        protected override void SingletonAwakened() {
            // LOAD SAVED SETTINGS
            
        }
    }
}