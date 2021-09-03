using Internal.Scripts.Core.Utils.Math;
using Internal.Scripts.Game.Gameplay.GameFieldObjects;
using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Game.Gameplay.Balls {
    public class MainBall : MonoBehaviour {
        [SerializeField] private float forceMultiplier = 100f;
        [SerializeField, Range(0, 3)] private int mouseButton;
        [SerializeField] private GameField gameField;

        [SerializeField] private UnityEvent playerShotEvent = new UnityEvent();
        // public int MouseButton => mouseButton;

        public UnityEvent PlayerShotEvent => playerShotEvent;

        public GameField GameField {
            get => gameField;
            set => gameField = value;
        }

        public Rigidbody2D Rigidbody2D { get; private set; }

        private TrajectoryManager _trajectoryManager;
        private Camera _mainCamera;
        private bool _isReadyToHit;
        private Vector2 _firstClickPos;

        private Vector2 GetCurrMousePos() => _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        private void OnAimingButtonDown() {
            if (!gameField.IsBallsStopped) return;
            _isReadyToHit = true;
            _firstClickPos = GetCurrMousePos();
        }

        private void OnAimingButton() {
            if (!gameField.IsBallsStopped || !_isReadyToHit) return;
            if (_trajectoryManager) {
                _trajectoryManager.OnAimingButton(
                    (Vector2) transform.position + (GetCurrMousePos() - _firstClickPos),
                    _firstClickPos);
            }
        }

        private void OnAimingButtonUp() {
            if (!gameField.IsBallsStopped || !_isReadyToHit) return;
            Vector2 position = transform.position;
            var targetPosition = position + (GetCurrMousePos() - _firstClickPos);
            _isReadyToHit = false;
            if (_trajectoryManager) {
                _trajectoryManager.OnAimingButtonUp();
            }
            if ((targetPosition - position).magnitude < 0.05f) return;
            var direction = (targetPosition - position).normalized;
            var forceVec = direction * Functions.RangeSigmoid(0f, forceMultiplier, (targetPosition - position).magnitude * 0.08f);
            Rigidbody2D.AddForce(forceVec);

            GlobalAudioManager.PlayHitSound();
            Ball.DestroyedCount = 0;
            PlayerShotEvent.Invoke();
        }

        private void Awake() {
            if (gameField) {
                if (!gameField.MainBall || gameField.MainBall != this) {
                    gameField.MainBall = this;
                }
            }
            
            _trajectoryManager = gameObject.GetComponent<TrajectoryManager>();
            _mainCamera = Camera.main;
            Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            if (!gameField) {
                gameField = transform.parent.parent.gameObject.GetComponent<GameField>();
            }

            // _collider2D = gameObject.GetComponent<CircleCollider2D>();
            GameInputControl.AddMouseButtonDownCallbackUiCheck(mouseButton, OnAimingButtonDown);
            GameInputControl.AddMouseButtonUpCallback(mouseButton, OnAimingButtonUp);
            GameInputControl.AddMouseButtonHoldCallback(mouseButton, OnAimingButton);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Equals("Border")
                || transform.GetInstanceID() < other.transform.GetInstanceID()) {
                GlobalAudioManager.PlayHitSound(Rigidbody2D.velocity.magnitude / 60f);
            }
        }
    }
}