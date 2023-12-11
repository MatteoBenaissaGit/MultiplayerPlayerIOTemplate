using System;
using System.Collections.Generic;
using Common;
using Controllers;
using UnityEngine;
using Views;

namespace Multiplayer
{
    public class GameElementsManager : Singleton<GameElementsManager>
    {
        public HashSet<GameElementController> GameElementsInGame { get; private set; }

        [SerializeField] private GameElementView _testGameElementViewPrefab;

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
            Debug.Log("initialize");
            PlayerGameManager.Instance.PlayerIoConnection.Send("CreateGameElement","Test", 0, 0);
        }

        #region Create/Move/Destroy Elements

        public void CreateGameElementAt(string type, string id, string ownerID, Vector2Int coordinates, int team)
        {
            Debug.Log("create game element at");
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
                Coordinates = coordinates,
                Team = team
            };
            
            switch (type)
            {
                case "Test":
                    gameElementView = Instantiate(_testGameElementViewPrefab);
                    gameElementController = new GameElementController(gameElementData, gameElementView);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (gameElementController == null)
            {
                throw new Exception("GameElement not created correctly");
            }

            _idToGameElement.Add(id, gameElementController);
            _gameElementToId.Add(gameElementController, id);
            GameElementsInGame.Add(gameElementController);

            Debug.Log("move game element");
            PlayerGameManager.Instance.PlayerIoConnection.Send("MoveGameElement", id, coordinates.x, coordinates.y);
        }

        public void MoveGameElement(string elementID, Vector2Int coordinates)
        {
            if (_idToGameElement.TryGetValue(elementID, out GameElementController element) == false)
            {
                throw new Exception("this element doesn't exist");
            }

            element.MoveTo(coordinates);
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
                    element.Data.Type, element.Data.ID, element.Data.ElementOwnerID, element.Data.Coordinates.x,
                    element.Data.Coordinates.y, element.Data.Team);
            }
        }

        public void GetGameElementDataFromServer(string type, string id, string ownerID, Vector2Int coordinates, int team)
        {
            Debug.LogError("get game element data from server");
            CreateGameElementAt(type, id, ownerID, coordinates, team);
        }

        #endregion
    }
}