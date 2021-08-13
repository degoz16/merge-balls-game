using System;
using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.Base {
    public class DataHolder : MonoBehaviour {
        [SerializeField] private List<ScriptableObject> dataObjects = new List<ScriptableObject>();

        private void Awake() {
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}
