using System;
using UnityEngine;
using Views.Golf;

namespace Golf
{
    public class GolfEndController : MonoBehaviour
    {
        [SerializeField] private GolfLevelManager _level;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GolfBallView golfBall) == false)
            {
                return;
            }
            
            _level.EndLevelHasBeenReached(golfBall);
        }
    }
}