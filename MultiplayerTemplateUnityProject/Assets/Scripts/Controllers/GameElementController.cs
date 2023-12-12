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
        
        public void MoveTo(Vector3 coordinates)
        {
            Data.Position = coordinates;
            View.MoveTo(Data.Position);
        }
    }
}