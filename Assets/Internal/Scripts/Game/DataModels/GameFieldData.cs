using System;
using System.Collections.Generic;
using System.Linq;
using Internal.Scripts.Game.Managers.Implementations;

namespace Internal.Scripts.Game.DataModels {
    [Serializable]
    public class GameFieldData {
        public List<BallData> balls;
        public MainBallData mainBall;
        public bool isBallsStopped;
        public long score;
        public long highScore;
        
        public GameFieldData() {
            score = GlobalStateManager.Instance.Score;
            highScore = GlobalStateManager.Instance.Score;
            mainBall = new MainBallData(GlobalStateManager.Instance.CurrentGameField.MainBall);
            balls = GlobalStateManager.Instance.CurrentGameField.Balls.Select(ball => new BallData(ball)).ToList();
            isBallsStopped = GlobalStateManager.Instance.CurrentGameField.IsBallsStopped;
        }
    }
}