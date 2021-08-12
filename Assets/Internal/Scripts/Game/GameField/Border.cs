using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.GameField {
    [ExecuteAlways]
    public class Border : MonoBehaviour {
        [SerializeField] private float width = 1f;
        [SerializeField] private float height = 1f;

        public float Width => width;
        public float Height => height;

        private EdgeCollider2D _edgeCollider2D;

        private void UpdatePoints() {
            _edgeCollider2D.SetPoints(new List<Vector2> {
                new Vector2(-width / 2f, -height / 2f),
                new Vector2(-width / 2f, +height / 2f),
                new Vector2(+width / 2f, +height / 2f),
                new Vector2(+width / 2f, -height / 2f),
                new Vector2(-width / 2f, -height / 2f)
            });
        }
        
        private void OnValidate() {
            _edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();
            UpdatePoints();
        }
        
        // Update is called once per frame
        private void Update() {
            
        }
    }
}