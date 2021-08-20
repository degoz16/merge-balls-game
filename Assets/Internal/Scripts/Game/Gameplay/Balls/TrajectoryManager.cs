using Internal.Scripts.Core.Utils.Math;
using UnityEngine;

namespace Internal.Scripts.Game.Gameplay.Balls {
    public class TrajectoryManager : MonoBehaviour {
        [SerializeField] private GameObject directionLinePrefab;
        [SerializeField] private GameObject reflectionLinePrefab;
        [SerializeField] private GameObject targetReflectionLinePrefab;
        [SerializeField] private GameObject ballProjectionPrefab;
        [SerializeField] private GameObject powerLevelLinePrefab;
        [SerializeField] private GameObject tapDirectionLinePrefab;

        private GameObject _directionLineGameObject;
        private GameObject _reflectionLineGameObject;
        private GameObject _targetReflectionLineGameObject;
        private GameObject _ballProjectionGameObject;
        private GameObject _powerLevelLineGameObject;
        private GameObject _tapDirectionLineGameObject;
        
        private LineRenderer _directionLine;
        private LineRenderer _reflectionLine;
        private LineRenderer _targetReflectionLine;
        private LineRenderer _ballProjection;
        private LineRenderer _powerLevel;
        private LineRenderer _tapDirection;

        [SerializeField] private float powerLevelLength = 3f;
        [SerializeField] private float powerLevelViewMultiplier = 0.08f;
        [SerializeField] private float directionLineWidth = 0.05f;
        [SerializeField, Range(0f, 1f)] private float powerLevelLineFadeLength = 0.8f;
        [SerializeField] private Gradient powerLevelGradient;

        // private MainBall _mainBall;
        // private bool _isReadyToHit;
        private Camera _mainCamera;
        private float _radius;
        // private bool IsBallStopped { get; set; } = true;
        // private void SetStopped() => IsBallStopped = true;

        private void Awake() {
            // GameField.GameField.BallsStoppedEvent.AddListener(SetStopped);
            
            _mainCamera = Camera.main;
            // _mainBall = gameObject.GetComponent<MainBall>();

            if (transform.parent) {
                var circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
                _radius = circleCollider2D ? circleCollider2D.radius : 0;
            }

            var thisTransform = transform;

            _directionLineGameObject = Instantiate(directionLinePrefab, thisTransform, true);
            _reflectionLineGameObject = Instantiate(reflectionLinePrefab, thisTransform, true);
            _targetReflectionLineGameObject = Instantiate(targetReflectionLinePrefab, thisTransform, true);
            _ballProjectionGameObject = Instantiate(ballProjectionPrefab, thisTransform, true);
            _powerLevelLineGameObject = Instantiate(powerLevelLinePrefab, thisTransform, true);
            _tapDirectionLineGameObject = Instantiate(tapDirectionLinePrefab, thisTransform, true);
            
            _directionLineGameObject.SetActive(false);
            _reflectionLineGameObject.SetActive(false);
            _targetReflectionLineGameObject.SetActive(false);
            _ballProjectionGameObject.SetActive(false);
            _powerLevelLineGameObject.SetActive(false);
            _tapDirectionLineGameObject.SetActive(false);

            _directionLine = _directionLineGameObject.GetComponent<LineRenderer>();
            _reflectionLine = _reflectionLineGameObject.GetComponent<LineRenderer>();
            _targetReflectionLine = _targetReflectionLineGameObject.GetComponent<LineRenderer>();
            _ballProjection = _ballProjectionGameObject.GetComponent<LineRenderer>();
            _powerLevel = _powerLevelLineGameObject.GetComponent<LineRenderer>();
            _tapDirection = _tapDirectionLineGameObject.GetComponent<LineRenderer>();
            
            _ballProjection.widthMultiplier = directionLineWidth;
            _reflectionLine.widthMultiplier = directionLineWidth;
            _targetReflectionLine.widthMultiplier = directionLineWidth;
            _powerLevel.widthMultiplier = directionLineWidth * 4;
            _powerLevel.endWidth = directionLineWidth;
            
            // GameInputControl.AddMouseButtonDownCallback(_mainBall.MouseButton, OnAimingButtonDown);
            // GameInputControl.AddMouseButtonUpCallback(_mainBall.MouseButton, OnAimingButtonUp);
            // GameInputControl.AddMouseButtonHoldCallback(_mainBall.MouseButton, OnAimingButton);
        }

        private static void UpdateLine(LineRenderer line, params Vector2[] positions) {
            line.positionCount = positions.Length;
            for (var i = 0; i < positions.Length; i++) {
                line.SetPosition(i, new Vector3(positions[i].x, positions[i].y, -0.1f));
            }
        }

        private static void UpdateCircle(LineRenderer line, Vector2 pos, float radius) {
            if (!line) return;
            var drawStep = 2 * Mathf.PI / line.positionCount;
            for (var i = 0; i < line.positionCount; i++) {
                line.SetPosition(i, new Vector3(
                    Mathf.Cos(i * drawStep) * radius + pos.x,
                    Mathf.Sin(i * drawStep) * radius + pos.y,
                    -0.1f));
            }
        }

        private static Vector2[] GetInterpolatedPoints(Vector2 pos1, Vector2 pos2, int cnt) {
            var points = new Vector2[cnt + 1];
            var step = 1f / cnt;
            for (var i = 0; i <= cnt; i++) {
                points[i] = Vector2.Lerp(pos1, pos2, step * i);
            }

            return points;
        }

        // public void OnAimingButtonDown() {
        //     _isReadyToHit = true;
        // }

        public void OnAimingButtonUp() {
            // if (_isReadyToHit) {
            //     // IsBallStopped = false;
            //     _isReadyToHit = false;
            // }

            _directionLineGameObject.SetActive(false);
            _reflectionLineGameObject.SetActive(false);
            _targetReflectionLineGameObject.SetActive(false);
            _ballProjectionGameObject.SetActive(false);
            _powerLevelLineGameObject.SetActive(false);
            _tapDirectionLineGameObject.SetActive(false);
        }

        public void OnAimingButton(Vector2 targetPos, Vector2 mousePos) {
            // if (!IsBallStopped || !_isReadyToHit) return;
            Vector2 position = transform.position;

            var direction = targetPos - position;
            direction.Normalize();
            
            var hit = Physics2D.CircleCast(
                position,
                _radius,
                direction,
                Mathf.Infinity,
                LayerMask.GetMask("Game Field"));
            if (hit) {
                // Draw a direction line
                UpdateLine(_directionLine, position, hit.centroid);

                if (!_directionLineGameObject.activeSelf) {
                    _directionLineGameObject.SetActive(true);
                }

                // Draw a reflection line
                var rotatedNormal = new Vector2(hit.normal.y, -hit.normal.x);

                var reflection = hit.transform.tag.Equals("Ball")
                    ? (Vector2.Dot(direction, rotatedNormal) * rotatedNormal).normalized
                    : Vector2.Reflect(direction, hit.normal).normalized;


                UpdateLine(_reflectionLine, hit.centroid, hit.centroid + reflection * 1.8f);
                if (!_reflectionLineGameObject.activeSelf) {
                    _reflectionLineGameObject.SetActive(true);
                }

                // Draw a target ball reflection line
                Vector2 hitObjectPos = hit.transform.position;
                if (hit.transform.tag.Equals("Ball")) {
                    UpdateLine(_targetReflectionLine, hitObjectPos, hitObjectPos - hit.normal * 1.8f);
                    if (!_targetReflectionLineGameObject.activeSelf) {
                        _targetReflectionLineGameObject.SetActive(true);
                    }
                } else {
                    if (_targetReflectionLineGameObject.activeSelf) {
                        _targetReflectionLineGameObject.SetActive(false);
                    }
                }

                // Draw a ball position preview
                UpdateCircle(_ballProjection, hit.centroid, _radius);
                if (!_ballProjectionGameObject.activeSelf) {
                    _ballProjectionGameObject.SetActive(true);
                }
            } else {
                _reflectionLineGameObject.SetActive(false);
                _targetReflectionLineGameObject.SetActive(false);
                _ballProjectionGameObject.SetActive(false);
                
                // Draw a direction line
                UpdateLine(_directionLine, position, position + direction * 2);

                if (!_directionLineGameObject.activeSelf) {
                    _directionLineGameObject.SetActive(true);
                }
            }

            // Draw a power level
            if (!_powerLevelLineGameObject.activeSelf) {
                _powerLevelLineGameObject.SetActive(true);
            }

            var sigmoid =
                Functions.RangeSigmoid(0.1f, powerLevelLength,
                    (targetPos - position).magnitude * powerLevelViewMultiplier);
            var secondPos = Vector2.Lerp(position, position + direction * powerLevelLength, sigmoid);

            UpdateLine(_powerLevel, GetInterpolatedPoints(position, secondPos, 10));

            var newGradient = new Gradient();
            var powerColor = powerLevelGradient.Evaluate(sigmoid);
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
            
            // Draw tap line
            _tapDirectionLineGameObject.SetActive(true);
            UpdateLine(_tapDirection, mousePos, mousePos + direction * 1.1f);
        }
    }
}