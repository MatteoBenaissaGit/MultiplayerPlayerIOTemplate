using Controllers;
using Multiplayer;
using UnityEngine;
using Views.Golf;

namespace Views
{
    public class GameElementView : MonoBehaviour
    {
        public GameElementController Controller { get; private set; }

        public virtual void SetElementView(GameElementController controller)
        {
            Controller = controller;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"view move to {position}");
            transform.position = new Vector3(position.x,position.y,position.z); 
        }
    }
}