using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;

namespace Internal.Scripts.Game.UI {
    public class RestartButton : MonoBehaviour {
        public void Restart() {
            GlobalStateManager.Instance.Restart();
        }
    }
}