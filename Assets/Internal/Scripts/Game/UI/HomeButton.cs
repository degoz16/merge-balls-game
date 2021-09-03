using Internal.Scripts.Game.Managers.Implementations;
using UnityEditor;
using UnityEngine;

namespace Internal.Scripts.Game.UI {
    public class HomeButton : MonoBehaviour {
        public void OnClick() {
            GlobalStateManager.Instance.Home();
        }
    }
}