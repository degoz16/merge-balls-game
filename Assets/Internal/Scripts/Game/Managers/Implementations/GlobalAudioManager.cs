using Internal.Scripts.Core.BaseClasses;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class GlobalAudioManager : MonoBehaviourSingleton<GlobalAudioManager> {
        private AudioSource _audioSource;

        protected override void SingletonAwakened() {
            var audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource) _audioSource = audioSource;
            else {
                gameObject.AddComponent<AudioSource>();
                _audioSource = gameObject.GetComponent<AudioSource>();
            }

            _audioSource.playOnAwake = false;
            _audioSource.mute = SettingsManager.Instance.IsMuted;
            SettingsManager.Instance.MuteSettingChangedEvent.AddListener(OnMuteChanged);
        }

        private void OnMuteChanged() {
            _audioSource.mute = SettingsManager.Instance.IsMuted;
        }
        
        public static void PlayHitSound(float volume = 0.5f) {
            var hitSounds = SettingsManager.Instance.AudioScheme.HitSounds;
            Instance._audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Count)], volume);
        }
        
        public static void PlayDestructionSound() {
            var destructionSounds = SettingsManager.Instance.AudioScheme.BallDestructionSounds;
            Instance._audioSource.PlayOneShot(destructionSounds[Random.Range(0, destructionSounds.Count)]);
        }
    }
}