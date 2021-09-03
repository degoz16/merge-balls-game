using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;

namespace Internal.Scripts.Game.UI {
    public class PlayButton : MonoBehaviour{
        public void OnClick() {
            GlobalStateManager.Instance.StartGame();
        }
    }
}