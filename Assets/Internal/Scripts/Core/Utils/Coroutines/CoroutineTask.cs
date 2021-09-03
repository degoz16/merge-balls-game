using System;
using System.Collections;
using UnityEngine;

namespace Internal.Scripts.Core.Utils.Coroutines {
    public class CoroutineTask {
        private MonoBehaviour _executor;
        private IEnumerator _routine;
        private bool _isRunning;
        
        public CoroutineTask(IEnumerator routine, MonoBehaviour executor) {
            _routine = routine;
            _executor = executor;
        }

        public void StopAndSetNewExecutor(MonoBehaviour executor) {
            if (_executor == null || _routine == null) return;
            if (_isRunning)
                _executor.StopCoroutine(_routine);
            _executor = executor;
        }
        
        public void StopAndSetNewRoutine(IEnumerator routine) {
            if (_executor == null || _routine == null) return;
            if (_isRunning)
                _executor.StopCoroutine(_routine);
            _routine = routine;
        }

        public void Start() {
            if (_executor == null || _routine == null) return;
            if (_isRunning) return;
            _isRunning = true;
            _executor.StartCoroutine(_routine);
        }

        private IEnumerator CoroutineWaiter() {
            yield return _executor.StartCoroutine(_routine);
            _isRunning = false;
        }

        private IEnumerator CoroutineWithTimeout(float seconds, Action onTimeOut = null) {
            var routine = CoroutineWaiter();
            _executor.StartCoroutine(routine);
            yield return new WaitForSecondsRealtime(seconds);
            if (_isRunning) {
                _executor.StartCoroutine(routine);
                onTimeOut?.Invoke();
            }

            _isRunning = false;
        }

        public void StartWithTimeout(float seconds, Action onTimeOut = null) {
            if (_executor == null || _routine == null) return;
            if (_isRunning) return;
            _isRunning = true;
            _executor.StartCoroutine(CoroutineWithTimeout(seconds, onTimeOut));
        }
    }
}