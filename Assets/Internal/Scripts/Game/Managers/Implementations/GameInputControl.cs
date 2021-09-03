using System.Collections.Generic;
using System.Linq;
using Internal.Scripts.Core.BaseClasses;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class GameInputControl : MonoBehaviourSingleton<GameInputControl> {
        private readonly Dictionary<int, UnityEvent> _mouseButtonsDownEvents = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<int, UnityEvent> _mouseButtonsUpEvents = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<int, UnityEvent> _mouseButtonsHoldEvents = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardDownEvents = new Dictionary<KeyCode, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardUpEvents = new Dictionary<KeyCode, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardHoldEvents = new Dictionary<KeyCode, UnityEvent>();

        private readonly Dictionary<int, UnityEvent> _mouseButtonsDownEventsUiCheck = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<int, UnityEvent> _mouseButtonsUpEventsUiCheck = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<int, UnityEvent> _mouseButtonsHoldEventsUiCheck = new Dictionary<int, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardDownEventsUiCheck = new Dictionary<KeyCode, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardUpEventsUiCheck = new Dictionary<KeyCode, UnityEvent>();
        private readonly Dictionary<KeyCode, UnityEvent> _keyboardHoldEventsUiCheck = new Dictionary<KeyCode, UnityEvent>();
        
        #region Internal helper methods
        private static void AddCallback<T>(IDictionary<T, UnityEvent> events, T key, UnityAction call) {
            if (!events.ContainsKey(key)) {
                events[key] = new UnityEvent();
            }
            
            events[key].AddListener(call);
        }

        private static void RemoveCallback<T>(IDictionary<T, UnityEvent> events, T key, UnityAction call) {
            if (events.ContainsKey(key)) {
                events[key].RemoveListener(call);
            }
        }
        
        #endregion
        
        public static void AddMouseButtonDownCallback(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsDownEvents, mouseButton, call);
        }
        
        public static void AddMouseButtonUpCallback(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsUpEvents, mouseButton, call);
        }
        
        public static void AddMouseButtonHoldCallback(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsHoldEvents, mouseButton, call);
        }
        
        public static void AddKeyboardButtonDownCallback(KeyCode key, UnityAction call) {
            AddCallback(Instance._keyboardDownEvents, key, call);
        }
        
        public static void AddKeyboardButtonUpCallback(KeyCode keyCode, UnityAction call) {
            AddCallback(Instance._keyboardUpEvents, keyCode, call);
        }
        
        public static void AddKeyboardButtonHoldCallback(KeyCode key, UnityAction call) {
            AddCallback(Instance._keyboardHoldEvents, key, call);
        }
        
        public static void RemoveMouseButtonDownCallback(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsDownEvents, mouseButton, call);
        }
        
        public static void RemoveMouseButtonUpCallback(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsUpEvents, mouseButton, call);
        }
        
        public static void RemoveMouseButtonHoldCallback(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsHoldEvents, mouseButton, call);
        }
        
        public static void RemoveKeyboardButtonDownCallback(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardDownEvents, key, call);
        }
        
        public static void RemoveKeyboardButtonUpCallback(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardUpEvents, key, call);
        }
        
        public static void RemoveKeyboardButtonHoldCallback(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardHoldEvents, key, call);
        }
        
        public static void AddMouseButtonDownCallbackUiCheck(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsDownEventsUiCheck, mouseButton, call);
        }
        
        public static void AddMouseButtonUpCallbackUiCheck(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsUpEventsUiCheck, mouseButton, call);
        }
        
        public static void AddMouseButtonHoldCallbackUiCheck(int mouseButton, UnityAction call) {
            AddCallback(Instance._mouseButtonsHoldEventsUiCheck, mouseButton, call);
        }
        
        public static void AddKeyboardButtonDownCallbackUiCheck(KeyCode key, UnityAction call) {
            AddCallback(Instance._keyboardDownEventsUiCheck, key, call);
        }
        
        public static void AddKeyboardButtonUpCallbackUiCheck(KeyCode key, UnityAction call) {
            AddCallback(Instance._keyboardUpEventsUiCheck, key, call);
        }
        
        public static void AddKeyboardButtonHoldCallbackUiCheck(KeyCode key, UnityAction call) {
            AddCallback(Instance._keyboardHoldEventsUiCheck, key, call);
        }
        
        public static void RemoveMouseButtonDownCallbackUiCheck(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsDownEventsUiCheck, mouseButton, call);
        }
        
        public static void RemoveMouseButtonUpCallbackUiCheck(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsUpEventsUiCheck, mouseButton, call);
        }
        
        public static void RemoveMouseButtonHoldCallbackUiCheck(int mouseButton, UnityAction call) {
            RemoveCallback(Instance._mouseButtonsHoldEventsUiCheck, mouseButton, call);
        }
        
        public static void RemoveKeyboardButtonDownCallbackUiCheck(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardDownEventsUiCheck, key, call);
        }
        
        public static void RemoveKeyboardButtonUpCallbackUiCheck(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardUpEventsUiCheck, key, call);
        }
        
        public static void RemoveKeyboardButtonHoldCallbackUiCheck(KeyCode key, UnityAction call) {
            RemoveCallback(Instance._keyboardHoldEventsUiCheck, key, call);
        }
        
        private void Update() {
            foreach (var pair in _mouseButtonsDownEvents.Where(pair => Input.GetMouseButtonDown(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _mouseButtonsUpEvents.Where(pair => Input.GetMouseButtonUp(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _mouseButtonsHoldEvents.Where(pair => Input.GetMouseButton(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardDownEvents.Where(pair => Input.GetKeyDown(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardUpEvents.Where(pair => Input.GetKeyUp(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardHoldEvents.Where(pair => Input.GetKey(pair.Key))) {
                pair.Value.Invoke();
            }
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began ){
                if(EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return;
            }
            
            foreach (var pair in _mouseButtonsDownEventsUiCheck.Where(pair => Input.GetMouseButtonDown(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _mouseButtonsUpEventsUiCheck.Where(pair => Input.GetMouseButtonUp(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _mouseButtonsHoldEventsUiCheck.Where(pair => Input.GetMouseButton(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardDownEventsUiCheck.Where(pair => Input.GetKeyDown(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardUpEventsUiCheck.Where(pair => Input.GetKeyUp(pair.Key))) {
                pair.Value.Invoke();
            }
            foreach (var pair in _keyboardHoldEventsUiCheck.Where(pair => Input.GetKey(pair.Key))) {
                pair.Value.Invoke();
            }
        }
    }
}