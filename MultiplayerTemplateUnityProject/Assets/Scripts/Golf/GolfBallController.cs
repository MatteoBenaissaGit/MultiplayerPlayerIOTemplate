using Controllers;
using Multiplayer;
using UnityEngine;
using Views;
using Views.Golf;

namespace Golf
{
    public class GolfBallController : GameElementController
    {
        public int NumberOfMoves { get; set; }
        
        public GolfBallController(GameElementData data, GameElementView view) : base(data, view)
        {
            
        }

        public void LaunchBall(float direction, float strength)
        {
            PlayerGameManager.Instance.UI.DebugMessage("launched ball");
            GolfBallView view = (GolfBallView)View;
            view.LaunchBall(direction,strength);
        }

        public void BallEndMovement()
        {
            Debug.LogError("end movement");

            Data.Position = View.transform.position;
            PlayerGameManager.Instance.SetEndTurn();
        }
    }
}