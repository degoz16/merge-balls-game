using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.Game.UI {
    public class MuteButton : MonoBehaviour {
        [SerializeField] private Sprite mutedSprite;
        [SerializeField] private Sprite unMutedSprite;

        [SerializeField] private GameObject imageGameObject;
        private Image _image;

        private void Awake() {
            SettingsManager.Instance.MuteSettingChangedEvent.AddListener(OnMuteUpdated);
            _image = imageGameObject ? imageGameObject.GetComponent<Image>() : gameObject.GetComponent<Image>();
        }

        private void OnEnable() {
            _image.sprite = SettingsManager.Instance.IsMuted ? mutedSprite : unMutedSprite;
        }

        private void OnMuteUpdated() {
            _image.sprite = SettingsManager.Instance.IsMuted ? mutedSprite : unMutedSprite;
        }

        public void OnClick() {
            SettingsManager.Instance.IsMuted = !SettingsManager.Instance.IsMuted;
        }
    }
}