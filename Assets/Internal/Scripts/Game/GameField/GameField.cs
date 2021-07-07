using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.GameField {
    public class GameField : MonoBehaviour {
        [SerializeField] private float ballRadius = 0.5f;
        [SerializeField] private float width = 10;
        [SerializeField] private float height = 8;

        [SerializeField] private int searchIterations = 1000;
        
        [SerializeField] private GameObject ballPrefab;
        
        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                var pos = FindBallPos();
                if (pos != null) {
                    var newBall = Instantiate(ballPrefab, pos.Value, Quaternion.identity);
                    newBall.transform.SetParent(transform);
                }
            }
        }

        private void FixedUpdate() {
            
        }

        private Vector2? FindBallPos() {
            for (int i = 0; i < searchIterations; i++) {
                Vector2 pos = new Vector2((Random.value - 0.5f) * (width - ballRadius),
                    (Random.value - 0.5f) * (height - ballRadius));
                RaycastHit2D raycastHit2D = Physics2D.CircleCast(pos, ballRadius + 0.05f, Vector2.zero, 0f);
                if (!raycastHit2D.collider) {
                    return pos;
                }
            }
            return null;
        }
    }
}