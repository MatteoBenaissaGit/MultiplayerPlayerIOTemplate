using System;
using Controllers;
using DG.Tweening;
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
        [SerializeField] private Transform _turnUI;
        [SerializeField] private DebugMessageUI _debugMessagePrefab;
        [SerializeField] private Transform _debugMessageLayout;
        [SerializeField] private Slider _strengthSlider, _directionSlider;
        [SerializeField] private Button _shootButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private TMP_Text _playerIdText;
        [SerializeField] private Transform _resultsLayout;
        [SerializeField] private ResultView _resultPrefab;
        [SerializeField] private Transform _playUI;

        public void SetPlayerUI(string playerId)
        {
            _playerIdText.text = playerId;
            _playerIdText.color = PlayerGameManager.Instance.PlayerColors[PlayerGameManager.Instance.Team];
            
            _resultsLayout.gameObject.SetActive(false);
            _quitButton.gameObject.SetActive(false);
        }
        
        public void SetTurnUI()
        {
            if (_turnText == null)
            {
                return;
            }
            
            GolfBallView view = null;
            if (PlayerGameManager.Instance.CurrentGolfLevel.PlayerBall != null)
            {
                view = (GolfBallView)PlayerGameManager.Instance.CurrentGolfLevel.PlayerBall.View;
            }
            string text;
            
            if (PlayerGameManager.Instance.Turn == PlayerGameManager.Instance.Team)
            {
                text = "It's your turn !";
                _turnText.color = Color.white;
                if (view != null) view.SetBallUI(true);
            }
            else
            {
                text = "Waiting for other player to play...";
                _turnText.color = Color.gray;
                if (view != null) view.SetBallUI(false);
            }
            _turnText.text = text;
        }

        public void DebugMessage(string message)
        {
            DebugMessageUI debug = Instantiate(_debugMessagePrefab, _debugMessageLayout);
            debug.Set(message);
        }

        public void InitializeBallUI(GolfBallView ball)
        {
            _strengthSlider.onValueChanged.AddListener(ball.SetStrength);
            _directionSlider.value = 0.5f;
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
            if (PlayerGameManager.Instance.CanPlay() == false
                || PlayerGameManager.Instance.HasFinishedLevel
                || PlayerGameManager.Instance.IsLevelEnded)
            {
                return;
            }
            
            PlayerGameManager.Instance.UI.DebugMessage("requested to launch ball");
            PlayerGameManager.Instance.PlayerIoConnection.Send("RequestToLaunchBall", 
                ball.Data.ID, ball.Data.ElementOwnerID,
                _strengthSlider.value, _directionSlider.value);
        }

        public void SetPlayUI(bool show)
        {
            _playUI.gameObject.SetActive(show);
        }

        public void ShowResults()
        {
            _turnUI.DOScaleY(0, 0.5f);
            
            _quitButton.gameObject.SetActive(true);
            _quitButton.onClick.AddListener(Application.Quit);
            
            _resultsLayout.gameObject.SetActive(true);
            Vector3 scale = _resultsLayout.localScale;
            _resultsLayout.localScale = new Vector3(scale.x, 0, scale.z);
            _resultsLayout.DOScaleY(scale.y, 0.5f);
            
            foreach (GolfBallController ball in GameElementsManager.Instance.Balls)
            {
                ResultView result = Instantiate(_resultPrefab, _resultsLayout);
                result.Set(ball.Data.ElementOwnerID, ball.NumberOfMoves - (PlayerGameManager.Instance.PlayerCount-1));
            }
        }
    }
}