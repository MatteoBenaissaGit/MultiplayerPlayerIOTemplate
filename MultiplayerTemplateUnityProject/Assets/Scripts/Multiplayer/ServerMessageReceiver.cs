using Golf;
using PlayerIOClient;
using UnityEngine;
using Views.Golf;

namespace Multiplayer
{
    /// <summary>
    /// The abstract class parent of all messages receivers classes 
    /// </summary>
    public abstract class ServerMessageReceiver
    {
        public abstract void Receive(Message m);
    }

    /// <summary>
    /// Handle the receiving of a turn set
    /// </summary>
    public class ReceiveSetTurn : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            int turn = m.GetInt(0);
            PlayerGameManager.Instance.SetTurn(turn);
        }
    }

    /// <summary>
    /// Handle the receiving of a request to send game infos to the server
    /// </summary>
    public class ReceiveSendGameInfosToServer : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            PlayerGameManager.Instance.UI.DebugMessage("send game infos from server");
            PlayerGameManager.Instance.PlayerIoConnection.Send("SendGameInfosToPlayers",
                PlayerGameManager.Instance.Turn);
        }
    }

    /// <summary>
    /// Handle the receiving of game infos from the server
    /// </summary>
    public class ReceiveGetGameInfosFromServer : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            PlayerGameManager.Instance.UI.DebugMessage("get game infos from server");
            int turnInfo = m.GetInt(0);
            PlayerGameManager.Instance.SetTurn(turnInfo);
        }
    }

    /// <summary>
    /// Handle the receiving of a team set 
    /// </summary>
    public class ReceiveSetTeam : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            //set the team
            int team = m.GetInt(0);
            PlayerGameManager.Instance.UI.DebugMessage($"set team {team}");
            PlayerGameManager.Instance.Team = team;
        
            //set the camera
            if (PlayerGameManager.Instance.Team >= PlayerGameManager.Instance.Cameras.Length)
            {
                Debug.LogError("No camera for this player");
            }
            for (int i = 0; i < PlayerGameManager.Instance.Cameras.Length; i++)
            {
                PlayerGameManager.Instance.Cameras[i].gameObject.SetActive(PlayerGameManager.Instance.Team == i);
            }
            
            //set the game
            GameElementsManager.Instance.Initialize();
        }
    }

    /// <summary>
    /// Handle the receiving of a new game element creation
    /// </summary>
    public class ReceiveCreateGameElement : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string createGameElementType = m.GetString(0);
            string createGameElementId = m.GetString(1);
            string createGameElementOwnerId = m.GetString(2);
            Vector3 gameElementCreateCoordinates = new Vector3(m.GetFloat(3), m.GetFloat(4), m.GetFloat(5));
            int createGameElementTeam = m.GetInt(6);
            PlayerGameManager.Instance.UI.DebugMessage($"create Element {createGameElementType}_{createGameElementId} for {createGameElementOwnerId}, at {gameElementCreateCoordinates}");
            GameElementsManager.Instance.CreateGameElementAt(
                createGameElementType, 
                createGameElementId,
                createGameElementOwnerId,
                gameElementCreateCoordinates,
                createGameElementTeam);
        }
    }
    
    /// <summary>
    /// Handle the receiving of a game element movement
    /// </summary>
    public class ReceiveMoveGameElement : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string gameElementId = m.GetString(0);
            Vector3 moveCoordinates = new Vector3(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
            PlayerGameManager.Instance.UI.DebugMessage(
                $"move element {gameElementId} to {moveCoordinates.x},{moveCoordinates.y}");
            GameElementsManager.Instance.MoveGameElement(gameElementId, moveCoordinates);
        }
    }
    
    /// <summary>
    /// Handle the receiving of a game element destruction
    /// </summary>
    public class ReceiveDestroyGameElement : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string destroyGameElementId = m.GetString(0);
            PlayerGameManager.Instance.UI.DebugMessage($"Destroy element {destroyGameElementId}");
            GameElementsManager.Instance.DestroyGameElement(destroyGameElementId);
        }
    }
    
    /// <summary>
    /// Handle the receiving of a game element information from server
    /// Used when connecting to a room and one of the player send you every game element present for you to create
    /// </summary>
    public class ReceiveGetGameElementFromServer : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string getGameElementType = m.GetString(0);
            string getGameElementId = m.GetString(1);
            string getGameElementOwnerId = m.GetString(2);
            Vector3 getGameElementCreateCoordinates = new Vector3(m.GetFloat(3), m.GetFloat(4), m.GetFloat(5));
            int getGameElementTeam = m.GetInt(6);
            PlayerGameManager.Instance.UI.DebugMessage("get game Element from server");
            GameElementsManager.Instance.GetGameElementDataFromServer(
                getGameElementType,
                getGameElementId,
                getGameElementOwnerId,
                getGameElementCreateCoordinates,
                getGameElementTeam);
        }
    }
    /// <summary>
    /// Handle the receiving of a request to send all present game element information to server
    /// Used when someone joined the room and need every game element present for him to create
    /// </summary>
    public class ReceiveSendGameElementsToServer : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            PlayerGameManager.Instance.UI.DebugMessage("send game Element to server");
            GameElementsManager.Instance.SendAllGameElementsDataToServer();
        }
    }

    public class ReceiveLaunchBall : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string getGameElementId = m.GetString(0);
            string getGameElementOwnerId = m.GetString(1);
            float strength = m.GetFloat(2);
            float direction = m.GetFloat(3);

            GolfBallController ball = (GolfBallController)GameElementsManager.Instance.GetGameElementFromID(getGameElementId);
            if (ball == null)
            {
                Debug.LogError("ball not found");
                return;
            }
            
            ball.LaunchBall(direction,strength);
        }
    }

    public class ReceiveCorrectBall : ServerMessageReceiver
    {
        public override void Receive(Message m)
        {
            string getGameElementId = m.GetString(0);
            float ballX = m.GetFloat(1);
            float ballY = m.GetFloat(2);
            float ballZ = m.GetFloat(3);
            
            GolfBallController ball = (GolfBallController)GameElementsManager.Instance.GetGameElementFromID(getGameElementId);
            if (ball == null)
            {
                Debug.LogError("ball not found");
                return;
            }

            PlayerGameManager.Instance.UI.DebugMessage("correct ball position");
            GolfBallView view = (GolfBallView)ball.View;
            view.CorrectPosition(new Vector3(ballX,ballY,ballZ));
        }
    }
    
}