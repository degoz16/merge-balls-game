using UnityEngine;

// ReSharper disable StaticMemberInGenericType

namespace Internal.Scripts.Core.BaseClasses {
    public abstract class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        /// <summary>
        ///     <c>true</c> if this Singleton Awake() method has already been called by Unity; otherwise, <c>false</c>.
        /// </summary>
        private static bool IsAwakened { get; set; }

        /// <summary>
        ///     <c>true</c> if this Singleton Start() method has already been called by Unity; otherwise, <c>false</c>.
        /// </summary>
        private static bool IsStarted { get; set; }

        /// <summary>
        ///     <c>true</c> if this Singleton OnDestroy() method has already been called by Unity; otherwise, <c>false</c>.
        /// </summary>
        private static bool IsDestroyed { get; set; }

        /// <summary>
        ///     Global access point to the unique instance of this class.
        /// </summary>
        public static T Instance {
            get {
                if (_instance != null) return _instance;
                if (IsDestroyed) return null;

                _instance = FindExistingInstance() ?? CreateNewInstance();

                return _instance;
            }
        }

        /// <summary>
        ///     Holds the unique instance for this class
        /// </summary>
        private static T _instance;

        /// <summary>
        ///     Finds an existing instance of this singleton in the scene.
        /// </summary>
        private static T FindExistingInstance() {
            var existingInstances = FindObjectsOfType<T>();

            // No instance found
            if (existingInstances == null || existingInstances.Length == 0) return null;

            return existingInstances[0];
        }

        /// <summary>
        ///     If no instance of the T MonoBehaviour exists, creates a new GameObject in the scene
        ///     and adds T to it.
        /// </summary>
        private static T CreateNewInstance() {
            var containerGO = new GameObject("__" + typeof(T).Name + " (Singleton)");
            return containerGO.AddComponent<T>();
        }
        
        /// <summary>
        ///     Unity3D Awake method.
        /// </summary>
        /// <remarks>
        ///     This method will only be called once even if multiple instances of the
        ///     singleton MonoBehaviour exist in the scene.
        ///     You can override this method in derived classes to customize the initialization of your MonoBehaviour
        /// </remarks>
        protected virtual void SingletonAwakened() {
        }

        /// <summary>
        ///     Unity3D Start method.
        /// </summary>
        /// <remarks>
        ///     This method will only be called once even if multiple instances of the
        ///     singleton MonoBehaviour exist in the scene.
        ///     You can override this method in derived classes to customize the initialization of your MonoBehaviour
        /// </remarks>
        protected virtual void SingletonStarted() { }

        /// <summary>
        ///     Unity3D OnDestroy method.
        /// </summary>
        /// <remarks>
        ///     This method will only be called once even if multiple instances of the
        ///     singleton MonoBehaviour exist in the scene.
        ///     You can override this method in derived classes to customize the initialization of your MonoBehaviour
        /// </remarks>
        protected virtual void SingletonDestroyed() { }


        /// <summary>
        ///     If a duplicated instance of a Singleton MonoBehaviour is loaded into the scene
        ///     this method will be called instead of SingletonAwakened(). That way you can customize
        ///     what to do with repeated instances.
        /// </summary>
        /// <remarks>
        ///     The default approach is delete the duplicated MonoBehaviour
        /// </remarks>
        protected virtual void NotifyInstanceRepeated() {
            Destroy(GetComponent<T>());
        }

        private void Awake() {
            var thisInstance = GetComponent<T>();

            if (_instance == null) {
                _instance = thisInstance;
                DontDestroyOnLoad(_instance.gameObject);
            } else if (thisInstance != _instance) {
                NotifyInstanceRepeated();
                return;
            }


            if (IsAwakened) return;
            
            SingletonAwakened();
            IsAwakened = true;
        }

        private void Start() {
            if (IsStarted) return;

            SingletonStarted();
            IsStarted = true;
        }

        private void OnDestroy() {
            if (this != _instance) return;
            
            IsDestroyed = true;
            SingletonDestroyed();
        }
    }
}