using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Internal.Scripts.Game.Balls;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Game.GameField {
    public class GameField : MonoBehaviour {
        [SerializeField] private int searchIterations = 1000;
        [SerializeField] private float maxVelocityToStop = 0.05f;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject balls;
        [SerializeField] private GameObject border;
        
        public delegate void BallsStoppedEventHandler();

        public delegate void GameOverEventHandler();

        public delegate void GameStartedEventHandler();

        public static event BallsStoppedEventHandler BallsStoppedEvent = delegate { };
        public static event GameOverEventHandler GameOverEvent = delegate { };
        public static event GameStartedEventHandler GameStartedEvent = delegate { };

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
            BallsStoppedEvent();
        }

        private void UpdateBallsList() {
            _ballsList = balls.GetComponentsInChildren<Rigidbody2D>();
        }

        private void SpawnBalls() {
            SpawnBalls(2);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnBalls(int count) {
            for (int i = 0; i < count; i++) {
                var pos = FindBallPos();
                if (pos != null) {
                    var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                    newBall.transform.SetParent(balls.transform);
                    newBall.GetComponent<Ball>().PowerProperty = Random.Range(1, 3);
                }
                else if (i == 0) {
                    GameOverEvent();
                    return;
                }
            }
        }

        // Start is called before the first frame update
        private void Awake() {
            UpdateBallsList();
            if (ballPrefab != null) {
                _ballRadius = ballPrefab.GetComponent<CircleCollider2D>().radius;
            }

            Transform borderTransform = border.transform;
            
            RectTransform borderRectTransform = border.GetComponent<RectTransform>();
            var rect = borderRectTransform.rect;
            _width = rect.width;
            _height = rect.height;
            _fieldPos = borderTransform.position - new Vector3(_width / 2f, _height / 2f);
        }

        private void Start() {
            // TEMP
            GameStartedEvent();
        }

        private void OnEnable() {
            Ball.BallsCountChangedEvent += UpdateBallsList;
            BallsStoppedEvent += SpawnBalls;
            GameStartedEvent += SpawnBalls;
            // MainBall.AimingStartedEvent += OnAimingStarted;
            MainBall.HitEvent += OnHit;
        }

        private void OnDisable() {
            Ball.BallsCountChangedEvent -= UpdateBallsList;
            BallsStoppedEvent -= SpawnBalls;
            GameStartedEvent -= SpawnBalls;
            // MainBall.AimingStartedEvent -= OnAimingStarted;
            MainBall.HitEvent -= OnHit;
            StopAllCoroutines();
        }

        // private void OnAimingStarted() {
        //     
        // }

        private void OnHit() {
            StartCoroutine(DoBallsMotionCheck());
        }

        // Update is called once per frame
        private void Update() {
        }

        private Vector2? FindBallPos() {
            for (int i = 0; i < searchIterations; i++) {
                Vector2 pos = new Vector2((Random.value) * (_width - _ballRadius * 2) + _ballRadius + _fieldPos.x,
                    (Random.value) * (_height - _ballRadius * 2) + _ballRadius + _fieldPos.y);
                RaycastHit2D raycastHit2D = Physics2D.CircleCast(pos, _ballRadius + 0.05f, Vector2.zero, 0f);
                if (!raycastHit2D.collider) {
                    return pos;
                }
            }

            return null;
        }
        
        
    }
}