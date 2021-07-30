using TMPro;
using UnityEngine;

namespace Internal.Scripts.Game.Balls {
    public class Ball : MonoBehaviour {
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private TextMeshPro _text;
        private long _number = 2;

        private long Number {
            get => _number;
            set {
                _number = value;
                if (_text) {
                    _text.SetText(_number.ToString());
                } 
            }
        }
        

        // Start is called before the first frame update
        private void Start() {
            _collider2D = gameObject.GetComponent<Collider2D>();
            _rigidbody2D = _collider2D.attachedRigidbody;
            _text = transform.Find("NumberText")
                .gameObject.GetComponent<TextMeshPro>();
            Number = 2;
        }

        // Update is called once per frame
        private void Update() {

        }

        private void FixedUpdate() {
            
        }

        private void OnCollisionEnter2D(Collision2D other) {
            // Debug.Log("COLLISION");
            var collisionObject = other.gameObject;
            if (collisionObject.tag.Equals("Ball")) {
                Ball ballScript = collisionObject.GetComponent<Ball>();
                if (ballScript.Number == _number) {
                    ballScript.Number *= 2;
                    Destroy(gameObject);
                }
            }
        }
    }
}