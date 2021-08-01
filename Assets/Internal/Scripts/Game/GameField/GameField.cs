using System;
using System.Collections;
using System.Linq;
using Internal.Scripts.Game.Balls;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Game.GameField {
    public class GameField : MonoBehaviour {
        [SerializeField] private float width = 10;
        [SerializeField] private float height = 8;
        [SerializeField] private int searchIterations = 1000;
        
        [SerializeField] private GameObject ballPrefab;

        public delegate void BallsStoppedEventHandler(bool isStopped);
        
        public static event BallsStoppedEventHandler BallsStoppedEvent = delegate {  };

        private Transform _balls;
        private Rigidbody2D[] _ballsList;
        private float _ballRadius = 0.5f;

        private bool CheckIsBallsStopped() {
            return _ballsList.All(r => r.velocity.magnitude < 0.05f);
        }
        
        private IEnumerator DoBallsMotionCheck() {
            while (enabled) {
                BallsStoppedEvent(CheckIsBallsStopped());
                yield return new WaitForSeconds(.5f);
            }
        }

        private void UpdateBallsList() {
            _ballsList = _balls.GetComponentsInChildren<Rigidbody2D>();
        }
        
        // Start is called before the first frame update
        private void Awake() {
            _balls = transform.Find("Balls");
            UpdateBallsList();
            if (ballPrefab != null) {
                _ballRadius = ballPrefab.GetComponent<CircleCollider2D>().radius;
            }
        }

        private void OnEnable() {
            Ball.BallsCountChangedEvent += UpdateBallsList;
            StartCoroutine(DoBallsMotionCheck());
        }

        private void OnDisable() {
            Ball.BallsCountChangedEvent -= UpdateBallsList;
            StopAllCoroutines();
        }

        // Update is called once per frame
        private void Update() {
            if (Input.GetMouseButtonUp(0)) {
                BallsStoppedEvent(false);
            }
            if (Input.GetKey(KeyCode.Space)) {
                var pos = FindBallPos();
                if (pos != null) {
                    var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                    newBall.transform.SetParent(_balls);
                }
            }
        }

        private Vector2? FindBallPos() {
            for (int i = 0; i < searchIterations; i++) {
                Vector2 pos = new Vector2((Random.value - 0.5f) * (width - _ballRadius),
                    (Random.value - 0.5f) * (height - _ballRadius));
                RaycastHit2D raycastHit2D = Physics2D.CircleCast(pos, _ballRadius + 0.05f, Vector2.zero, 0f);
                if (!raycastHit2D.collider) {
                    return pos;
                }
            }
            return null;
        }
    }
}