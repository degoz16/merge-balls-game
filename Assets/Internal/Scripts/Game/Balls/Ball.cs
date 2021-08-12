using System;
using System.Collections;
using Internal.Scripts.Game.ColorSchemes;
using TMPro;
using UnityEngine;

namespace Internal.Scripts.Game.Balls {
    public class Ball : MonoBehaviour {
        [SerializeField] private ColorSchemeObject colorScheme;
        [SerializeField] private GameObject particlesPrefab;
        
        
        private Color _color = Color.gray;
        private Color _textColor = Color.white;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;
        private TextMeshPro _text;
        private long _number = 2;
        private int _power = 1;

        public delegate void BallsCountChangedEventHandler();
        public static event BallsCountChangedEventHandler BallsCountChangedEvent = delegate {  };
        
        public int PowerProperty {
            get => _power;
            set {
                _power = value;
                _number = 1 << value;

                if (_text) {
                    _text.SetText(_number.ToString());
                }

                if (colorScheme != null && colorScheme.NumberColors.Count > 0) {
                    ColorProperty = colorScheme
                        .NumberColors[Math.Min(value, colorScheme.NumberColors.Count) - 1];
                }
                else {
                    ColorProperty = _color;
                }
                
                if (colorScheme != null && colorScheme.TextColors.Count > 0) {
                    TextColorProperty = colorScheme
                        .TextColors[Math.Min(value, colorScheme.TextColors.Count) - 1];
                }
                else {
                    TextColorProperty = _textColor;
                }
            }
        }

        private Color ColorProperty {
            get => _color;
            set {
                _color = value;
                if (_spriteRenderer != null)
                    _spriteRenderer.color = value;
            }
        }

        private Color TextColorProperty {
            get => _textColor;
            set {
                _textColor = value;
                if (_text != null)
                    _text.color = _textColor;
            }
        }
        
        // Start is called before the first frame update
        private void Awake() {
            _collider2D = gameObject.GetComponent<Collider2D>();
            _rigidbody2D = _collider2D.attachedRigidbody;
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _text = transform.Find("NumberText")
                .gameObject.GetComponent<TextMeshPro>();
            PowerProperty = 1;
        }

        private void Start() {
            BallsCountChangedEvent();
        }

        // Update is called once per frame
        private void Update() {
        }

        private void OnCollisionEnter2D(Collision2D other) {
            // Debug.Log("COLLISION");
            var collisionObject = other.gameObject;
            if (collisionObject.tag.Equals("Ball")) {
                Ball ballScript = collisionObject.GetComponent<Ball>();
                if (ballScript.PowerProperty == _power) {
                    ballScript.PowerProperty++;
                    Destroy(gameObject);
                }
            }
        }
        
        private void OnDestroy() {
            BallsCountChangedEvent();
            GameObject particlesObject = Instantiate(particlesPrefab, transform.parent, true);
            ParticleSystem particles = particlesObject.GetComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = _color;
            particlesObject.transform.position = transform.position;
            particles.Play();
        }
    }
}