using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.GameField {
    [ExecuteInEditMode]
    public class BorderInitializer : MonoBehaviour {
        private EdgeCollider2D _edgeCollider2D;
        private RectTransform _rectTransform;
        private float _width;
        private float _height;
        
        // Start is called before the first frame update
        private void Start() {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();
        }

        // Update is called once per frame
        private void Update() {
            var rect = _rectTransform.rect;
            _width = rect.width;
            _height = rect.height;
            _edgeCollider2D.SetPoints(new List<Vector2> {
                new Vector2(-_width / 2f, -_height / 2f),
                new Vector2(-_width / 2f, +_height / 2f),
                new Vector2(+_width / 2f, +_height / 2f),
                new Vector2(+_width / 2f, -_height / 2f),
                new Vector2(-_width / 2f, -_height / 2f)
            });
        }
    }
}