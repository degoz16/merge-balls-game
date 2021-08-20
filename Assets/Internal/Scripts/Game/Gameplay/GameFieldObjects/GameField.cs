using System;
using System.Collections;
using System.Linq;
using Internal.Scripts.Game.Gameplay.Balls;
using Internal.Scripts.Game.Managers.Implementations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Game.Gameplay.GameFieldObjects {
    public class GameField : MonoBehaviour {
        [SerializeField] private int searchIterations = 1000;
        [SerializeField] private float maxVelocityToStop = 0.05f;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject balls;
        [SerializeField] private GameObject border;
        [SerializeField] private MainBall mainBall;

        private float _width;
        private float _height;
        private Vector2 _fieldPos;
        private Rigidbody2D[] _ballsList;
        private float _ballRadius = 0.5f;
        public bool IsBallsStopped { get; private set; } = true;

        public MainBall MainBall {
            get => mainBall;
            set => mainBall = value;
        }

        private bool CheckIsBallsStopped() {
            return _ballsList.All(r => r.velocity.magnitude < maxVelocityToStop);
        }

        private IEnumerator DoBallsMotionCheck() {
            IsBallsStopped = false;
            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(CheckIsBallsStopped);
            yield return new WaitForSeconds(Ball.AnimationTime + 0.2f);
            IsBallsStopped = true;
            OnBallsStopped();
        }

        private void UpdateBallsList() {
            _ballsList = balls.GetComponentsInChildren<Rigidbody2D>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnBalls(int count = 2) {
            if (_ballRadius * _ballRadius * Mathf.PI * _ballsList.Length > _width * _height / 3.5f) {
                GlobalStateManager.Instance.GameOver();
                return;
            }

            // var flag = false;
            for (var i = 0; i < count; i++) {
                var pos = FindBallPos();
                if (pos == null) continue;
                var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                newBall.transform.SetParent(balls.transform);
                newBall.GetComponent<Ball>().PowerProperty = Random.Range(1, 3);
                // flag = true;
            }
            // if (!flag) {
            //     GlobalStateManager.Instance.GameOver();
            // }
        }

        private void OnGameOver() {
            foreach (var rigidbody2D1 in _ballsList) {
                Destroy(rigidbody2D1.gameObject);
            }
        }

        private void OnGameStarted(bool isNewGame) {
            if (isNewGame)
                SpawnBalls();
        }

        private void OnBallsStopped() {
            SpawnBalls(
                Math.Max(
                    Random.Range(1, 3), Random.Range(
                        Math.Max(1, Ball.DestroyedCount - 2), 
                        Math.Max(3, Ball.DestroyedCount + 1))));
        }

        // Start is called before the first frame update
        private void Awake() {
            if (mainBall) {
                if (!mainBall.GameField || mainBall.GameField != this) {
                    MainBall.GameField = this;
                }
            }

            mainBall.PlayerShotEvent.AddListener(() => StartCoroutine(DoBallsMotionCheck()));
            UpdateBallsList();
            if (ballPrefab != null) {
                _ballRadius = ballPrefab.GetComponent<CircleCollider2D>().radius;
            }

            var borderTransform = border.transform;

            var borderScript = border.GetComponent<Border>();
            _width = borderScript.Width;
            _height = borderScript.Height;
            _fieldPos = borderTransform.position - new Vector3(_width / 2f, _height / 2f);
        }

        private void OnEnable() {
            Ball.BallsCountChangedEvent.AddListener(UpdateBallsList);
            GlobalStateManager.Instance.GameStartEvent.AddListener(OnGameStarted);
            GlobalStateManager.Instance.GameOverEvent.AddListener(OnGameOver);
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        private Vector2? FindBallPos() {
            for (var i = 0; i < searchIterations; i++) {
                var pos = new Vector2((Random.value) * (_width - _ballRadius * 2) + _ballRadius + _fieldPos.x,
                    (Random.value) * (_height - _ballRadius * 2) + _ballRadius + _fieldPos.y);
                var raycastHit2D = Physics2D.CircleCast(pos, _ballRadius + 0.05f, Vector2.zero, 0f);
                if (!raycastHit2D.collider) {
                    return pos;
                }
            }

            return null;
        }
    }
}