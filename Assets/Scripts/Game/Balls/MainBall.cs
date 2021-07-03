using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Balls {
    public class MainBall : MonoBehaviour {
        [SerializeField] private float forceMultiplier = 100f;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _lastPos = Vector2.zero;
        
        // Start is called before the first frame update
        void Start() {
            _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            // var dt = Time.deltaTime;
        }

        // Update is called once per frame
        void Update() {
            var dt = Time.deltaTime;
            
            
            // var forceVec = dt * _forceMultiplier * (Input.touches[0].position - _lastPos);
            if (Input.GetMouseButtonDown(0)) {
                _lastPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0)) {
                var forceVec = dt * forceMultiplier * ((Vector2) Input.mousePosition - _lastPos);
                _rigidbody2D.AddForce(forceVec);
            }
        }
        
    }
}