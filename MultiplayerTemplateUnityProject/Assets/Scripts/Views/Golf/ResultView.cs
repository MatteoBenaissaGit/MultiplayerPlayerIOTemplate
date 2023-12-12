using TMPro;
using UnityEngine;

namespace Views.Golf
{
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _player, _moves;
        
        public void Set(string player, int numberOfMoves)
        {
            _player.text = player;
            _moves.text = numberOfMoves.ToString();
        }
    }
}