using System;
using Internal.Scripts.Game.Gameplay.Balls;
using UnityEngine;

namespace Internal.Scripts.Game.DataModels {
    [Serializable]
    public class MainBallData {
        public Vector2 position;
        public Vector2 velocity;
        
        public MainBallData(MainBall ball) {
            position = ball.transform.position;
            velocity = ball.gameObject.GetComponent<Rigidbody2D>().velocity;
        }

        public MainBallData() {
            position = Vector2.zero;
            velocity = Vector2.zero;
        }
    }
}