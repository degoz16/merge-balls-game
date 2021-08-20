using Internal.Scripts.Game.DataObjects.ScriptableObjects;
using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Initializers {
    public class GameInit : MonoBehaviour {
        [SerializeField] private ColorSchemeObject colorScheme;
        [SerializeField] private AudioSchemeObject audioScheme;
        private void Awake() {
            SettingsManager.Instance.ColorScheme = colorScheme;
            SettingsManager.Instance.AudioScheme = audioScheme;
            
            GameInputControl.AddKeyboardButtonDownCallback(KeyCode.Escape, GlobalStateManager.Instance.Restart);
            Application.targetFrameRate = 90;
        }
    }
}
