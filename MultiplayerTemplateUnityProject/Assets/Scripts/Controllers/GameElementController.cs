using UnityEngine;
using Views;

namespace Controllers
{
    public class GameElementController
    {
        public GameElementData Data { get; private set; }
        public GameElementView View { get; internal set; }

        public GameElementController(GameElementData data, GameElementView view)
        {
            Data = data;
            View = view;
        }
        
        public void MoveTo(Vector2Int coordinates)
        {
            Data.Coordinates = coordinates;
            View.MoveTo(Data.Coordinates);
        }
    }
}