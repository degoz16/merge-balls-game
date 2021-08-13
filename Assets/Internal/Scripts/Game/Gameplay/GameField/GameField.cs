using System;
using System.Collections;
using System.Linq;
using Internal.Scripts.Core.ScriptableObjects.DataObjects.GlobalControl;
using Internal.Scripts.Game.Gameplay.Balls;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Game.Gameplay.GameField {
    public class GameField : MonoBehaviour {
        [SerializeField] private int searchIterations = 1000;
        [SerializeField] private float maxVelocityToStop = 0.05f;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject balls;
        [SerializeField] private GameObject border;

        public static event EventHandler BallsStoppedEvent = delegate { };
        public static event EventHandler BallPlaceNotFoundEvent = delegate { };

        private float _width;
        private float _height;
        private Vector2 _fieldPos;

        private Rigidbody2D[] _ballsList;
        private float _ballRadius = 0.5f;

        private bool CheckIsBallsStopped() {
            return _ballsList.All(r => r.velocity.magnitude < maxVelocityToStop);
        }

        private IEnumerator DoBallsMotionCheck() {
            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(CheckIsBallsStopped);
            BallsStoppedEvent(this, EventArgs.Empty);
        }

        private void UpdateBallsList() {
            _ballsList = balls.GetComponentsInChildren<Rigidbody2D>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnBalls(int count = 2) {
            var flag = false;
            for (var i = 0; i < count; i++) {
                var pos = FindBallPos();
                if (pos == null) continue;
                var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                newBall.transform.SetParent(balls.transform);
                newBall.GetComponent<Ball>().PowerProperty = Random.Range(1, 3);
                flag = true;
            }
            if (!flag) {
                BallPlaceNotFoundEvent(this, EventArgs.Empty);
            }
        }

        private void OnGameOver(object sender, EventArgs args) {
            foreach (var rigidbody2D1 in _ballsList) {
                Destroy(rigidbody2D1.gameObject);
            }
        }
        
        private void OnGameStarted(object sender, StartEventArgs args) {
            SpawnBalls();
        }

        private void OnBallsStopped(object sender, EventArgs args) {
            SpawnBalls();
        }

        private void OnBallsCountChanged(object sender, EventArgs args) {
            UpdateBallsList();
        }
        
        // Start is called before the first frame update
        private void Awake() {
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

        private void Update() {
            if (Input.GetMouseButtonUp(0)) {
                StartCoroutine(DoBallsMotionCheck());
            }
        }

        private void OnEnable() {
            Ball.BallsCountChangedEvent += OnBallsCountChanged;
            BallsStoppedEvent += OnBallsStopped;
            GlobalControl.GameStartEvent += OnGameStarted;
            GlobalControl.GameOverEvent += OnGameOver;
        }

        private void OnDisable() {
            Ball.BallsCountChangedEvent -= OnBallsCountChanged;
            BallsStoppedEvent -= OnBallsStopped;
            GlobalControl.GameStartEvent -= OnGameStarted;
            GlobalControl.GameOverEvent -= OnGameOver;
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