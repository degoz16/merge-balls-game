using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Initializers {
    public class GpgsInit : MonoBehaviour {
        private void Start() {
            GpgsManager.Initialize();
            GpgsManager.Authenticate(status => {
                CloudSaveManager.Initialize();
            });
        }

        private void OnApplicationQuit() {
            GpgsManager.LogOut();
        }
    }
}