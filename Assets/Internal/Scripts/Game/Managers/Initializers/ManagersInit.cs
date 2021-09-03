using Internal.Scripts.Game.DataObjects.ScriptableObjects;
using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Initializers {
    public class ManagersInit : MonoBehaviour {
        [SerializeField] private ColorSchemeObject colorScheme;
        [SerializeField] private AudioSchemeObject audioScheme;
        private void Awake() {
            SettingsManager.Instance.ColorScheme = colorScheme;
            SettingsManager.Instance.AudioScheme = audioScheme;
            IapManager.Init();
            GameInputControl.AddKeyboardButtonDownCallback(KeyCode.Escape, GlobalStateManager.Instance.Home);
            GameInputControl.AddKeyboardButtonDownCallback(KeyCode.Space, GlobalStateManager.Instance.GameOver);
            Application.targetFrameRate = 90;
        }
    }
}
