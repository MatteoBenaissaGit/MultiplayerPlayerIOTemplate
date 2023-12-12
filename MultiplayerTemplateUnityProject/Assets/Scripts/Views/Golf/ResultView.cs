using Multiplayer;
using TMPro;
using UnityEngine;

namespace Views.Golf
{
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _player, _moves;
        
        public void Set(string player, int numberOfMoves)
        {
            Color color = PlayerGameManager.Instance.PlayerColors[int.Parse(player[^1].ToString())-1];
            
            _player.text = player;
            _player.color = color;
            _moves.text = numberOfMoves.ToString();
            _moves.color = color;
        }
    }
}