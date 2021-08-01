using UnityEngine;

namespace Internal.Scripts.Game.Balls {
    public class MainBall : MonoBehaviour {
        [SerializeField] private float forceMultiplier = 100f;

        private Rigidbody2D _rigidbody2D;
        private CircleCollider2D _collider2D;
        private Camera _mainCamera;

        private bool IsBallStopped { get; set; } = false;
        private void SetStopped(bool stopped) => IsBallStopped = stopped;
        
        private void OnEnable() {
            GameField.GameField.BallsStoppedEvent += SetStopped;
        }

        private void OnDisable() {
            GameField.GameField.BallsStoppedEvent -= SetStopped;
        }
        
        private void Awake() {
            _mainCamera = Camera.main;
            _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            _collider2D = gameObject.GetComponent<CircleCollider2D>();
        }

        private void Update() {
            Vector2 position = transform.position;
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // mousePosition.z = 0;
            if (Input.GetMouseButtonUp(0)) {
                if (IsBallStopped) {
                    Vector2 forceVec = forceMultiplier * (mousePosition - position);
                    _rigidbody2D.AddForce(forceVec);
                }
            }
        }

        private void FixedUpdate() {
            
        }
    }
}