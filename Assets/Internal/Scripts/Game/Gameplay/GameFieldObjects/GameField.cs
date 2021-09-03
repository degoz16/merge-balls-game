using System;
using System.Collections;
using System.Collections.Generic;
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
        private Rigidbody2D[] _ballsRigidBodyList;
        private float _ballRadius = 0.5f;
        public bool IsBallsStopped { get; private set; } = true;
        private IEnumerator _lastAdCoroutine;

        public List<Ball> Balls => balls.transform.GetComponentsInChildren<Ball>().ToList();

        public MainBall MainBall {
            get => mainBall;
            set => mainBall = value;
        }

        private bool CheckIsBallsStopped() {
            return _ballsRigidBodyList.All(r => r.velocity.magnitude < maxVelocityToStop);
        }

        private IEnumerator DoBallsMotionCheck() {
            IsBallsStopped = false;
            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(CheckIsBallsStopped);
            OnBallsStopped();
            yield return new WaitForSeconds(Ball.AnimationTime + 0.1f);
            IsBallsStopped = true;
        }

        private void StartAdShowing() {
            _lastAdCoroutine = AdShower();
            StartCoroutine(_lastAdCoroutine);
        }

        private void StopAdShowing() {
            if (_lastAdCoroutine != null) {
                StopCoroutine(_lastAdCoroutine);
                _lastAdCoroutine = null;
            }
        }
        
        private IEnumerator AdShower() {
            var period = new WaitForSecondsRealtime(10);
            while (GlobalStateManager.Instance.IsGameScene) {
                yield return new WaitUntil(() => AdMobManager.Instance.IsAdClosed);
                yield return period;
                AdMobManager.Instance.ShowInterstitialAd();
            }
        }
        
        private void UpdateBallsList() {
            _ballsRigidBodyList = balls.GetComponentsInChildren<Rigidbody2D>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnBalls(int count = 2) {
            // var flag = false;
            for (var i = 0; i < count; i++) {
                var pos = FindBallPos();
                if (pos == null) continue;
                var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                var ballScript = newBall.GetComponent<Ball>();
                ballScript.PlaySpawnAnimation();
                newBall.transform.SetParent(balls.transform);
                ballScript.PowerProperty = Random.Range(1, 3);
                // flag = true;
            }
            // if (!flag) {
            //     GlobalStateManager.Instance.GameOver();
            // }
        }

        private void OnRestart() {
            StopAdShowing();
            StartAdShowing();
            OnGameStarted(true);
        }
        
        private void OnGameOver() {
            StopAdShowing();
            AdMobManager.Instance.ShowInterstitialAd();
            GlobalStateManager.Instance.Score = 0;
            Balls.ForEach(b => Destroy(b.gameObject));
            mainBall.transform.localPosition = Vector3.zero;
            mainBall.Rigidbody2D.velocity = Vector2.zero;
            SettingsManager.Instance.IsNewGame = true;
        }

        private void OnGameStarted(bool isNewGame) {
            StartAdShowing();
            SaveLoadManager.Instance.LoadGameFieldData(gameFieldData => {
                var ballsList = Balls;
                ballsList.ForEach(b => Destroy(b.gameObject));
                GlobalStateManager.Instance.HighScore = Math.Max(gameFieldData.highScore, GlobalStateManager.Instance.HighScore);
                if (isNewGame) {
                    GlobalStateManager.Instance.Score = 0;
                    mainBall.transform.localPosition = Vector3.zero;
                    SpawnBalls();
                    SettingsManager.Instance.IsNewGame = false;
                } else {
                    GlobalStateManager.Instance.Score = gameFieldData.score;
                    mainBall.transform.position = gameFieldData.mainBall.position;
                    mainBall.Rigidbody2D.velocity = gameFieldData.mainBall.velocity;
                    IsBallsStopped = gameFieldData.isBallsStopped;
                    gameFieldData.balls.ForEach(b => {
                        var newBall = Instantiate(ballPrefab, b.position, b.rotation);
                        newBall.transform.SetParent(balls.transform);
                        var ballScript = newBall.GetComponent<Ball>();
                        ballScript.PowerProperty = b.power;
                        ballScript.Rigidbody2D.velocity = b.velocity;
                        // Debug.Log(b.velocity);
                    });
                    if (!IsBallsStopped) StartCoroutine(DoBallsMotionCheck());
                }
            });
        }

        private void OnHome() {
            StopAdShowing();
        }
        
        private void OnBallsStopped() {
            if (_ballRadius * _ballRadius * Mathf.PI * _ballsRigidBodyList.Length > _width * _height / 3f) {
                GlobalStateManager.Instance.GameOver();
                return;
            }

            var ballsCnt = Math.Max(2, Ball.DestroyedCount);
            SpawnBalls(Random.Range(1, ballsCnt + 1));
        }

        // Start is called before the first frame update
        private void Awake() {
            GlobalStateManager.Instance.CurrentGameField = this;
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
            
            Ball.BallsCountChangedEvent.AddListener(UpdateBallsList);
            GlobalStateManager.Instance.GameStartedEvent.AddListener(OnGameStarted);
            GlobalStateManager.Instance.GameOverEvent.AddListener(OnGameOver);
            GlobalStateManager.Instance.HomeEvent.AddListener(OnHome);
            GlobalStateManager.Instance.RestartEvent.AddListener(OnRestart);
        }

        

        private void OnDisable() {
            StopAllCoroutines();
        }

        private Vector2? FindBallPos() {
            for (var i = 0; i < searchIterations; i++) {
                var pos = new Vector2(Random.value * (_width - _ballRadius * 2) + _ballRadius + _fieldPos.x,
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