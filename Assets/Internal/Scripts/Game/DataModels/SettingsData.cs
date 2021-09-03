using System;
using Internal.Scripts.Game.Managers.Implementations;

namespace Internal.Scripts.Game.DataModels {
    [Serializable]
    public class SettingsData {
        public bool mute;
        public bool isNewGame;
        public bool isAdsRemoved;
        public SettingsData() {
            mute = SettingsManager.Instance.IsMuted;
            isNewGame = SettingsManager.Instance.IsNewGame;
            isAdsRemoved = SettingsManager.Instance.IsAdsRemoved;
        }
    }
}