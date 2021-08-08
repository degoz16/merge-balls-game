using UnityEngine;

namespace Internal.Scripts.Game.Balls {
    public class MainBall : MonoBehaviour {
        [SerializeField] private float forceMultiplier = 100f;

        private Rigidbody2D _rigidbody2D;
        private CircleCollider2D _collider2D;
        private Camera _mainCamera;
        private bool _isReadyToHit;
        private bool IsBallStopped { get; set; } = true;
        private void SetStopped() => IsBallStopped = true;

        public delegate void AimingStartedEventHandler();
        public delegate void HitEventHandler();
        
        public static event AimingStartedEventHandler AimingStartedEvent = delegate {  };
        public static event HitEventHandler HitEvent = delegate {  };
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
            if (Input.GetMouseButtonDown(0)) {
                if (IsBallStopped) {
                    _isReadyToHit = true;
                    AimingStartedEvent();
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                if (IsBallStopped && _isReadyToHit) {
                    _isReadyToHit = false;
                    Vector2 forceVec = forceMultiplier * (mousePosition - position);
                    _rigidbody2D.AddForce(forceVec);
                    IsBallStopped = false;
                    HitEvent();
                }
            }
        }
    }
}