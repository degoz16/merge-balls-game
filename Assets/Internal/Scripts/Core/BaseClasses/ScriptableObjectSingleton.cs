using System.Linq;
using UnityEngine;

namespace Internal.Scripts.Core.BaseClasses {
    /// <summary>
    /// Class helper to create singletons out of Scriptable Objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject {
        private static T _instance = null;

        public static T Instance {
            get {
                if (!_instance) {
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
                return _instance;
            }
        }
    }
}