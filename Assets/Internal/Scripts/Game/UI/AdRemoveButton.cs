using System.Collections;
using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.Game.UI {
    public class AdRemoveButton : MonoBehaviour {
        private Button _button;

        private IEnumerator OnStartCoroutine() {
            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            _button.interactable = true;
        }
        
        private void Awake() {
            _button = gameObject.GetComponent<Button>();
            _button.interactable = false;
            StartCoroutine(OnStartCoroutine());
            SettingsManager.Instance.AdsSettingChangedEvent.AddListener(OnSettingChanged);
            OnSettingChanged();
        }

        private IEnumerator OnSettingChangedCoroutine() {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(!SettingsManager.Instance.IsAdsRemoved);
        }

        private void OnSettingChanged() {
            StartCoroutine(OnSettingChangedCoroutine());
        }

        public void OnPurchase() {
            SettingsManager.Instance.IsAdsRemoved = true;
        }
    }
}