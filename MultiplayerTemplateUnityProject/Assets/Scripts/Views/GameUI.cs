using System;
using Golf;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Views.Golf;

namespace Views
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private DebugMessageUI _debugMessagePrefab;
        [SerializeField] private Transform _debugMessageLayout;
        [SerializeField] private Slider _strengthSlider, _directionSlider;
        [SerializeField] private Button _shootButton;

        public void SetTurnUI()
        {
            if (_turnText == null)
            {
                return;
            }
            
            bool turnTeam0 = PlayerGameManager.Instance.Turn % 2 == 0;
            string text = turnTeam0 ? "Turn team 0" : "Turn team 1";
            Color color = turnTeam0 ? Color.white : Color.black;
            _turnText.text = text;
            _turnText.color = color;
        }

        public void DebugMessage(string message)
        {
            DebugMessageUI debug = Instantiate(_debugMessagePrefab, _debugMessageLayout);
            debug.Set(message);
        }

        public void InitializeBallUI(GolfBallView ball)
        {
            _strengthSlider.onValueChanged.AddListener(ball.SetStrength);
            _directionSlider.onValueChanged.AddListener(ball.SetDirection);
            _shootButton.onClick.AddListener(() => RequestToLaunchBall((GolfBallController)ball.Controller));
        }

        private void OnDestroy()
        {
            _strengthSlider.onValueChanged.RemoveAllListeners();
            _directionSlider.onValueChanged.RemoveAllListeners();
        }

        private void RequestToLaunchBall(GolfBallController ball)
        {
            if (PlayerGameManager.Instance.CanPlay() == false)
            {
                return;
            }
            
            PlayerGameManager.Instance.UI.DebugMessage("requested to launch ball");
            PlayerGameManager.Instance.PlayerIoConnection.Send("RequestToLaunchBall", 
                ball.Data.ID, ball.Data.ElementOwnerID,
                _strengthSlider.value, _directionSlider.value);
        }
    }
}