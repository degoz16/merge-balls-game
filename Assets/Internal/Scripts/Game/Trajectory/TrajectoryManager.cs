using UnityEngine;

namespace Internal.Scripts.Game.Trajectory {
    public class TrajectoryManager : MonoBehaviour {
        [SerializeField] private GameObject directionLineGameObject;
        [SerializeField] private GameObject reflectionLineGameObject;
        [SerializeField] private GameObject targetReflectionLineGameObject;
        [SerializeField] private GameObject ballProjectionGameObject;

        private LineRenderer _directionLine;
        private LineRenderer _reflectionLine;
        private LineRenderer _targetReflectionLine;
        private LineRenderer _ballProjection;

        private Camera _mainCamera;
        private float _radius;

        void Start() {
            _mainCamera = Camera.main;
            if (transform.parent) {
                CircleCollider2D circleCollider2D = transform.parent.gameObject.GetComponent<CircleCollider2D>();
                _radius = circleCollider2D ? circleCollider2D.radius : 0;
            }

            directionLineGameObject.SetActive(false);
            reflectionLineGameObject.SetActive(false);
            targetReflectionLineGameObject.SetActive(false);
            ballProjectionGameObject.SetActive(false);

            _directionLine = directionLineGameObject.GetComponent<LineRenderer>();
            _reflectionLine = reflectionLineGameObject.GetComponent<LineRenderer>();
            _targetReflectionLine = targetReflectionLineGameObject.GetComponent<LineRenderer>();
            _ballProjection = ballProjectionGameObject.GetComponent<LineRenderer>();
        }

        private static void UpdateLine(LineRenderer line, Vector2 position1, Vector2 position2) {
            Vector3 point = new Vector3(0, 0, -0.1f) {x = position1.x, y = position1.y};

            line.SetPosition(0, point);

            point.x = position2.x;
            point.y = position2.y;
            line.SetPosition(1, point);
        }

        private static void UpdateCircle(LineRenderer line, Vector2 pos, float radius) {
            if (line != null) {
                float drawStep = 2 * Mathf.PI / line.positionCount;
                for (int i = 0; i < line.positionCount; i++) {
                    line.SetPosition(i, new Vector3(
                        Mathf.Cos(i * drawStep) * radius + pos.x,
                        Mathf.Sin(i * drawStep) * radius + pos.y,
                        -0.1f));
                }
            }
        }

        // Update is called once per frame
        void Update() {
            Vector2 position = transform.position;
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButton(0)) {
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
                    _directionLine.startWidth = Mathf.Clamp(
                        (mousePosition - position).magnitude / 10, 0.01f, 2 * _radius * 0.9f);

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
            }

            if (Input.GetMouseButtonUp(0)) {
                directionLineGameObject.SetActive(false);
                reflectionLineGameObject.SetActive(false);
                targetReflectionLineGameObject.SetActive(false);
                ballProjectionGameObject.SetActive(false);
            }
        }
    }
}