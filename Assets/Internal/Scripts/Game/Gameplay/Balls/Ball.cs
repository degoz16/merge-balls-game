using System;
using System.Collections;
using Internal.Scripts.Game.Managers.Implementations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace Internal.Scripts.Game.Gameplay.Balls {
    public class Ball : MonoBehaviour {
        [SerializeField] private GameObject particlesPrefab;
        public static float AnimationTime => 0.2f;
        public static int DestroyedCount { get; set; }
        private Color _color = Color.gray;
        private Color _textColor = Color.white;
        private SpriteRenderer _spriteRenderer;
        private TextMeshPro _text;
        private long _number = 2;
        private int _power = 1;

        public Rigidbody2D Rigidbody2D { get; set; }

        public static UnityEvent BallsCountChangedEvent { get; } = new UnityEvent();
        
        public int PowerProperty {
            get => _power;
            set {
                _power = value;
                _number = 1 << value;

                if (_text) {
                    _text.SetText(_number.ToString());
                }

                var colorScheme = SettingsManager.Instance.ColorScheme;
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
            // get => _color;
            set {
                _color = value;
                if (_spriteRenderer != null)
                    _spriteRenderer.color = value;
            }
        }

        private Color TextColorProperty {
            // get => _textColor;
            set {
                _textColor = value;
                if (_text != null)
                    _text.color = _textColor;
            }
        }
        
        // Start is called before the first frame update
        private void Awake() {
            // _collider2D = gameObject.GetComponent<Collider2D>();
            Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _text = transform.Find("NumberText")
                .gameObject.GetComponent<TextMeshPro>();
            PowerProperty = 1;
        }

        private IEnumerator SpawnAnimation(float timeSeconds = 1f) {
            var currTime = 0f;
            while (currTime < timeSeconds) {
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Math.Min(1f, currTime / timeSeconds));
                currTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = Vector3.one;
        }

        public void PlaySpawnAnimation() {
            StartCoroutine(SpawnAnimation(AnimationTime));
        }
        
        private void Start() {
            BallsCountChangedEvent.Invoke();
        }

        
        
        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Equals("Border") 
                || transform.GetInstanceID() < other.transform.GetInstanceID()) {
                GlobalAudioManager.PlayHitSound(Rigidbody2D.velocity.magnitude / 60f);
            }
            var collisionObject = other.gameObject;
            if (!collisionObject.tag.Equals("Ball")) return;
            var ballScript = collisionObject.GetComponent<Ball>();
            if (ballScript.PowerProperty != _power) return;
            ballScript.PowerProperty++;
            DestroyBall();
        }

        private void DestroyBall() {
            DestroyedCount++;
            GlobalStateManager.Instance.Score += _number;
            var particlesObject = Instantiate(particlesPrefab, transform.parent, true);
            var particles = particlesObject.GetComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = _color;
            particlesObject.transform.position = transform.position;
            particles.Play();
            GlobalAudioManager.PlayDestructionSound();
            Destroy(gameObject);
        }

        private void OnDestroy() {
            BallsCountChangedEvent.Invoke();
        }
    }
}