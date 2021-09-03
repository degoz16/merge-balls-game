using System;
using Internal.Scripts.Game.Gameplay.Balls;
using UnityEngine;

namespace Internal.Scripts.Game.DataModels {
    [Serializable]
    public class BallData {
        public Quaternion rotation;
        public Vector2 position;
        public int power;
        public Vector2 velocity;
        
        public BallData(Ball ball) {
            var transform = ball.transform;
            position = transform.position;
            rotation = transform.rotation;
            power = ball.PowerProperty;
            velocity = ball.gameObject.GetComponent<Rigidbody2D>().velocity;
        }
    }
}