using System;
using System.Collections.Generic;
using Common;
using Controllers;
using Golf;
using UnityEngine;
using Views;
using Views.Golf;

namespace Multiplayer
{
    public class GameElementsManager : Singleton<GameElementsManager>
    {
        public HashSet<GameElementController> GameElementsInGame { get; private set; }

        [SerializeField] private GameElementView _testGameElementViewPrefab;
        [SerializeField] private GameElementView _golfBallGameElementViewPrefab;

        private Dictionary<string, GameElementController> _idToGameElement;
        private Dictionary<GameElementController, string> _gameElementToId;

        protected override void InternalAwake()
        {
            _idToGameElement = new Dictionary<string, GameElementController>();
            _gameElementToId = new Dictionary<GameElementController, string>();
            GameElementsInGame = new HashSet<GameElementController>();
        }

        public void Initialize()
        {
            Vector3 startPosition = PlayerGameManager.Instance.CurrentGolfLevel.Start.position;
            PlayerGameManager.Instance.PlayerIoConnection.Send("CreateGameElement","GolfBall", startPosition.x, startPosition.y, startPosition.z);
        }

        #region Create/Move/Destroy Elements

        public void CreateGameElementAt(string type, string id, string ownerID, Vector3 position, int team)
        {
            PlayerGameManager.Instance.UI.DebugMessage($"create game element for team {team}");
            if (_idToGameElement.ContainsKey(id))
            {
                PlayerGameManager.Instance.UI.DebugMessage("this element already exist");
                return;
            }

            GameElementController gameElementController = null;
            GameElementView gameElementView = null;
            GameElementData gameElementData = new GameElementData()
            {
                Type = type,
                ID = id,
                ElementOwnerID = ownerID,
                Position = position,
                Team = team
            };
            
            switch (type)
            {
                case "Test":
                    gameElementView = Instantiate(_testGameElementViewPrefab);
                    gameElementController = new GameElementController(gameElementData, gameElementView);
                    break;
                case "GolfBall":
                    gameElementView = Instantiate(_golfBallGameElementViewPrefab);
                    gameElementController = new GolfBallController(gameElementData, gameElementView);
                    if (team == PlayerGameManager.Instance.Team)
                    {
                        PlayerGameManager.Instance.CurrentGolfLevel.PlayerBall = (GolfBallController)gameElementController;
                        PlayerGameManager.Instance.UI.InitializeBallUI((GolfBallView)gameElementView);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (gameElementController == null)
            {
                throw new Exception("GameElement not created correctly");
            }
            
            gameElementView.SetElementView(gameElementController);

            _idToGameElement.Add(id, gameElementController);
            _gameElementToId.Add(gameElementController, id);
            GameElementsInGame.Add(gameElementController);

            Debug.Log("move game element");
            PlayerGameManager.Instance.PlayerIoConnection.Send("MoveGameElement", id, position.x, position.y, position.z);
        }

        public void MoveGameElement(string elementID, Vector3 position)
        {
            if (_idToGameElement.TryGetValue(elementID, out GameElementController element) == false)
            {
                throw new Exception("this element doesn't exist");
            }

            element.MoveTo(position);
        }

        public void DestroyGameElement(string elementID)
        {
            GameElementController element = _idToGameElement[elementID];
            GameElementsInGame.Remove(element);
            Destroy(element.View.gameObject);
        }
        
        #endregion

        #region Send/Get Elements

        public void SendAllGameElementsDataToServer()
        {
            foreach (GameElementController element in GameElementsInGame)
            {
                PlayerGameManager.Instance.UI.DebugMessage($"send game element {element.Data.Type} {element.Data.ID} to players");
                PlayerGameManager.Instance.PlayerIoConnection.Send("SendGameElementToPlayers",
                    element.Data.Type, element.Data.ID, element.Data.ElementOwnerID, element.Data.Position.x,
                    element.Data.Position.y, element.Data.Position.z, element.Data.Team);
            }
        }

        public void GetGameElementDataFromServer(string type, string id, string ownerID, Vector3 position, int team)
        {
            Debug.LogError("get game element data from server");
            CreateGameElementAt(type, id, ownerID, position, team);
        }

        #endregion

        public GameElementController GetGameElementFromID(string id)
        {
            return _idToGameElement.TryGetValue(id, out GameElementController element) == false ? null : element;
        }
    }
}