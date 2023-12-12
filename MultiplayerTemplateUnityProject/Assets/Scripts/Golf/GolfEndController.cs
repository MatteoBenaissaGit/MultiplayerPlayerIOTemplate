using System;
using UnityEngine;
using Views.Golf;

namespace Golf
{
    public class GolfEndController : MonoBehaviour
    {
        [SerializeField] private GolfLevelManager _level;
        [SerializeField] private ParticleSystem _particleEnd;

        private void OnTriggerEnter(Collider other)
        {
            _particleEnd.Play();
            
            if (other.TryGetComponent(out GolfBallView golfBall) == false)
            {
                return;
            }

            other.enabled = false;
            Debug.LogError("trigger end");
            _level.EndLevelHasBeenReached(golfBall);
        }
    }
}