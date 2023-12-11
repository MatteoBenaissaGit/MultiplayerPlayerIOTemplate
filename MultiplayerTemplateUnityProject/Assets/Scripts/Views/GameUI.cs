using Multiplayer;
using TMPro;
using UnityEngine;

namespace Views
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private DebugMessageUI _debugMessagePrefab;
        [SerializeField] private Transform _debugMessageLayout;

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
    }
}