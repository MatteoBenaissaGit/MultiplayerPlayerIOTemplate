using Controllers;
using Multiplayer;
using UnityEngine;

namespace Views
{
    public class GameElementView : MonoBehaviour
    {
        public GameElementController Controller { get; private set; }

        public void SetPawnView(GameElementController controller)
        {
            Controller = controller;
        }

        public void MoveTo(Vector2Int coordinates)
        {
            Debug.Log($"view move to {coordinates}");
            //TODO | be careful here, might need to put the coordinate y in the y position instead of 0 if it's a 2d game or 
            //TODO | might need to transfer coordinates to real world position.
            transform.position = new Vector3(coordinates.x,0,coordinates.y); 
        }
    }
}