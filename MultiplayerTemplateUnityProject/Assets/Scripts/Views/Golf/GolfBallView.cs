using System;
using Controllers;
using DG.Tweening;
using Golf;
using Multiplayer;
using UnityEngine;

namespace Views.Golf
{
    public class GolfBallView : GameElementView
    {
        [SerializeField] private GameObject _team1Mesh, _team2Mesh;
        [SerializeField] private Transform _directionUI;
        [SerializeField] private SpriteRenderer _directionSprite;
        [SerializeField] private Rigidbody _rigidbody;

        private bool _isLaunched;
        private float _timeToCheckEndMovement;
        
        private void Update()
        {
            CheckForFall();
            CheckForEndMovement();
        }
        
        public override void SetElementView(GameElementController controller)
        {
            base.SetElementView(controller);
            
            _team1Mesh.SetActive(Controller.Data.Team == 0);
            _team2Mesh.SetActive(Controller.Data.Team == 1);

            _directionSprite.color = Color.yellow;

            if (controller.Data.Team != PlayerGameManager.Instance.Team)
            {
                _directionUI.gameObject.SetActive(false);
            }
        }

        public void SetDirection(float value)
        {
            float direction = value * 360f;
            _directionUI.transform.rotation = Quaternion.Euler(0,direction,0);
        }
        
        public void SetStrength(float value)
        {
            _directionSprite.color = Color.Lerp(Color.yellow, Color.red, value);
        }

        public void LaunchBall(float direction, float strength)
        {
            strength = Mathf.Clamp(strength,0.1f, 1f) * 10;
            direction = direction * 360f;
            PlayerGameManager.Instance.UI.DebugMessage($"ball view launch ball f:{strength} d:{(int)direction}");
            
            float angleDegrees = direction;
            float angleRadians = Mathf.Deg2Rad * angleDegrees;
            Vector3 forceDirection = new Vector3(Mathf.Sin(angleRadians), 0f,Mathf.Cos(angleRadians));
            _rigidbody.AddForce(forceDirection * strength, ForceMode.VelocityChange);
            PlayerGameManager.Instance.UI.DebugMessage($"vector {forceDirection}");

            _isLaunched = true;
            _timeToCheckEndMovement = 0.5f;
        }
        
        private void CheckForEndMovement()
        {
            if (Controller.Data.Team != PlayerGameManager.Instance.Team)
            {
                return;
            }

            if (_isLaunched == false)
            {
                return;
            }

            _timeToCheckEndMovement -= Time.deltaTime;
            if (_timeToCheckEndMovement > 0)
            {
                return;
            }

            if (_rigidbody.velocity.magnitude > 0.05f)
            {
                return;
            }

            _isLaunched = false;
            GolfBallController ball = (GolfBallController)Controller;
            _rigidbody.velocity = Vector3.zero;
            ball.BallEndMovement();
        }
        
        private void CheckForFall()
        {
            if (transform.position.y > -1)
            {
                return;
            }
            
            transform.position = Controller.Data.Position;
            _rigidbody.velocity = Vector3.zero;

            if (_isLaunched)
            {
                _isLaunched = false;
                GolfBallController ball = (GolfBallController)Controller;
                _rigidbody.velocity = Vector3.zero;
                ball.BallEndMovement();
            }
        }

        public void CorrectPosition(Vector3 position)
        {
            Controller.Data.Position = position;
            transform.position = position;
        }

        public void SetBallUI(bool show)
        {
            _directionUI.gameObject.SetActive(show);
            PlayerGameManager.Instance.UI.SetPlayUI(show);
        }

        public void Disappear()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            transform.DOScale(Vector3.zero, 0.5f);
            GolfBallController ball = (GolfBallController)Controller;
            ball.BallEndMovement();
        }
    }
}