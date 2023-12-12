using System;
using UnityEngine;
using Views.Golf;

namespace Golf
{
    public class GolfLevelManager : MonoBehaviour
    {
        [field:SerializeField] public Transform Start { get; private set; }
        [field:SerializeField] public Collider End { get; private set; }
        
        public GolfBallController PlayerBall { get;  set; }
        
        public void EndLevelHasBeenReached(GolfBallView golfBall)
        {
            Debug.LogError($"team {golfBall.Controller.Data.Team} has reached end");
        }
    }
}
