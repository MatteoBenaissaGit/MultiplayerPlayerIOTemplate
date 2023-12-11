using Multiplayer;
using UnityEngine;

namespace Controllers
{
    public class InputController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerGameManager.Instance.PlayerIoConnection.Send("Chat", "Clicked space");
            }
        }
    }
}
