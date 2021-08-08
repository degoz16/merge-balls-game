using Internal.Scripts.Core.Utils;
using Internal.Scripts.Game.Balls;
using UnityEngine;

namespace Internal.Scripts.Game.Trajectory {
    public class TrajectoryManager : MonoBehaviour {
        [SerializeField] private GameObject directionLineGameObject;
        [SerializeField] private GameObject reflectionLineGameObject;
        [SerializeField] private GameObject targetReflectionLineGameObject;
        [SerializeField] private GameObject ballProjectionGameObject;
        [SerializeField] private GameObject powerLevelLineGameObject;

        private LineRenderer _directionLine;
        private LineRenderer _reflectionLine;
        private LineRenderer _targetReflectionLine;
        private LineRenderer _ballProjection;
        private LineRenderer _powerLevel;

        [SerializeField] private float powerLevelLength = 3f;
        [SerializeField] private float powerLevelViewMultiplier = 0.08f;
        [SerializeField] private float directionLineWidth = 0.05f;
        [SerializeField, Range(0f, 1f)] private float powerLevelLineFadeLength = 0.8f;
        [SerializeField] private Gradient powerLevelGradient;

        private bool _isReadyToHit = false;
        private Camera _mainCamera;
        private float _radius;
        private bool IsBallStopped { get; set; } = true;
        private void SetStopped() => IsBallStopped = true;
        
        private void OnEnable() {
            GameField.GameField.BallsStoppedEvent += SetStopped;
            MainBall.AimingStartedEvent += OnAimingStarted;
            MainBall.HitEvent += OnHit;
        }

        private void OnDisable() {
            GameField.GameField.BallsStoppedEvent -= SetStopped;
            MainBall.AimingStartedEvent -= OnAimingStarted;
            MainBall.HitEvent -= OnHit;
        }

        private void Awake() {
            _mainCamera = Camera.main;
            if (transform.parent) {
                CircleCollider2D circleCollider2D = transform.parent.gameObject.GetComponent<CircleCollider2D>();
                _radius = circleCollider2D ? circleCollider2D.radius : 0;
            }

            directionLineGameObject.SetActive(false);
            reflectionLineGameObject.SetActive(false);
            targetReflectionLineGameObject.SetActive(false);
            ballProjectionGameObject.SetActive(false);
            powerLevelLineGameObject.SetActive(false);

            _directionLine = directionLineGameObject.GetComponent<LineRenderer>();
            _reflectionLine = reflectionLineGameObject.GetComponent<LineRenderer>();
            _targetReflectionLine = targetReflectionLineGameObject.GetComponent<LineRenderer>();
            _ballProjection = ballProjectionGameObject.GetComponent<LineRenderer>();
            _powerLevel = powerLevelLineGameObject.GetComponent<LineRenderer>();

            _ballProjection.widthMultiplier = directionLineWidth;
            _reflectionLine.widthMultiplier = directionLineWidth;
            _targetReflectionLine.widthMultiplier = directionLineWidth;
            _powerLevel.widthMultiplier = directionLineWidth * 4;
            _powerLevel.endWidth = directionLineWidth;
        }

        private static void UpdateLine(LineRenderer line, params Vector2[] positions) {
            line.positionCount = positions.Length;
            for (int i = 0; i < positions.Length; i++) {
                line.SetPosition(i, new Vector3(positions[i].x, positions[i].y, -0.1f));
            }
        }

        private static void UpdateCircle(LineRenderer line, Vector2 pos, float radius) {
            if (line) {
                float drawStep = 2 * Mathf.PI / line.positionCount;
                for (int i = 0; i < line.positionCount; i++) {
                    line.SetPosition(i, new Vector3(
                        Mathf.Cos(i * drawStep) * radius + pos.x,
                        Mathf.Sin(i * drawStep) * radius + pos.y,
                        -0.1f));
                }
            }
        }

        private static Vector2[] GetInterpolatedPoints(Vector2 pos1, Vector2 pos2, int cnt) {
            var points = new Vector2[cnt + 1];
            float step = 1f / cnt;
            for (int i = 0; i <= cnt; i++) {
                points[i] = Vector2.Lerp(pos1, pos2, step * i);
            }

            return points;
        }

        private void OnAimingStarted() {
            if (IsBallStopped) {
                _isReadyToHit = true;
            }
        }
        
        private void OnHit() {
            if (_isReadyToHit) {
                IsBallStopped = false;
                _isReadyToHit = false;
            }

            directionLineGameObject.SetActive(false);
            reflectionLineGameObject.SetActive(false);
            targetReflectionLineGameObject.SetActive(false);
            ballProjectionGameObject.SetActive(false);
            powerLevelLineGameObject.SetActive(false);
        }
        
        // Update is called once per frame
        private void Update() {
            Vector2 position = transform.position;
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButton(0)) {
                if (IsBallStopped && _isReadyToHit) {
                    Vector2 direction = mousePosition - position;
                    direction.Normalize();

                    RaycastHit2D hit = Physics2D.CircleCast(
                        position,
                        _radius,
                        direction,
                        Mathf.Infinity,
                        LayerMask.GetMask("Game Field"));
                    if (hit) {
                        // Draw a direction line
                        UpdateLine(_directionLine, position, hit.centroid);

                        if (!directionLineGameObject.activeSelf) {
                            directionLineGameObject.SetActive(true);
                        }

                        // Draw a reflection line
                        Vector2 rotatedNormal = new Vector2(hit.normal.y, -hit.normal.x);

                        Vector2 reflection = hit.transform.tag.Equals("Ball")
                            ? (Vector2.Dot(direction, rotatedNormal) * rotatedNormal).normalized
                            : Vector2.Reflect(direction, hit.normal).normalized;


                        UpdateLine(_reflectionLine, hit.centroid, hit.centroid + reflection * 1.8f);
                        if (!reflectionLineGameObject.activeSelf) {
                            reflectionLineGameObject.SetActive(true);
                        }

                        // Draw a target ball reflection line
                        Vector2 hitObjectPos = hit.transform.position;
                        if (hit.transform.tag.Equals("Ball")) {
                            UpdateLine(_targetReflectionLine, hitObjectPos, hitObjectPos - hit.normal * 1.8f);
                            if (!targetReflectionLineGameObject.activeSelf) {
                                targetReflectionLineGameObject.SetActive(true);
                            }
                        }
                        else {
                            if (targetReflectionLineGameObject.activeSelf) {
                                targetReflectionLineGameObject.SetActive(false);
                            }
                        }
    
                        // Draw a ball position preview
                        UpdateCircle(_ballProjection, hit.centroid, _radius);
                        if (!ballProjectionGameObject.activeSelf) {
                            ballProjectionGameObject.SetActive(true);
                        }
                    }
                    else {
                        reflectionLineGameObject.SetActive(false);
                        targetReflectionLineGameObject.SetActive(false);
                        ballProjectionGameObject.SetActive(false);
                        
                        // Draw a direction line
                        UpdateLine(_directionLine, position, position + direction * 2);

                        if (!directionLineGameObject.activeSelf) {
                            directionLineGameObject.SetActive(true);
                        }
                    }
                    // Draw a power level
                    if (!powerLevelLineGameObject.activeSelf) {
                        powerLevelLineGameObject.SetActive(true);
                    }

                    float sigmoid =
                        Functions.RangeSigmoid(0.1f, powerLevelLength,
                            (mousePosition - position).magnitude * powerLevelViewMultiplier);
                    Vector2 secondPos = Vector2.Lerp(position, position + direction * powerLevelLength, sigmoid);

                    UpdateLine(_powerLevel, GetInterpolatedPoints(position, secondPos, 10));

                    Gradient newGradient = new Gradient();
                    Color powerColor = powerLevelGradient.Evaluate(sigmoid);
                    newGradient.SetKeys(
                        new[] {
                            new GradientColorKey(powerColor, 0f),
                            new GradientColorKey(powerColor, 1f)
                        },
                        new[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(1f, powerLevelLineFadeLength),
                            new GradientAlphaKey(0f, 1f)
                        });
                    _powerLevel.colorGradient = newGradient;
                }
            }
        }
    }
}