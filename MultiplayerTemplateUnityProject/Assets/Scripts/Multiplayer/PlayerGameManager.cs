using System.Collections.Generic;
using Common;
using PlayerIOClient;
using UnityEngine;
using Views;

namespace Multiplayer
{
	public class PlayerGameManager : Singleton<PlayerGameManager>
	{
		[field:SerializeField] public Camera[] Cameras { get; private set; }
		[field:SerializeField] public GameUI UI { get; private set; }
	
		public Connection PlayerIoConnection { get; private set; }
		public int Team { get; set; }
		public int Turn { get; private set; }
	
		[SerializeField] private bool _useLocalServer;
		[SerializeField] private string _gameId;
	
		private const string ConnectionId = "public";
		private const string RoomId = "UnityDefaultRoomID";
		private const string RoomType = "UnityBaseRoom";
	
		private List<Message> _messagesList = new List<Message>();
		private bool _joinedRoom;
		private string _userId;

		private Dictionary<string, ServerMessageReceiver> _receivedMessageToMethod = new Dictionary<string, ServerMessageReceiver>();

		protected override void InternalAwake()
		{
			//add classes for receiving message from server
			_receivedMessageToMethod.Add("SetTeam", new ReceiveSetTeam());
			_receivedMessageToMethod.Add("SetTurn", new ReceiveSetTurn());
			_receivedMessageToMethod.Add("GetGameInfosFromServer", new ReceiveGetGameInfosFromServer());
			_receivedMessageToMethod.Add("SendGameInfosToServer", new ReceiveSendGameInfosToServer());
			_receivedMessageToMethod.Add("CreateGameElement", new ReceiveCreateGameElement());
			_receivedMessageToMethod.Add("MoveGameElement", new ReceiveMoveGameElement());
			_receivedMessageToMethod.Add("DestroyGameElement", new ReceiveDestroyGameElement());
			_receivedMessageToMethod.Add("SendGameElementsToServer", new ReceiveSendGameElementsToServer());
			_receivedMessageToMethod.Add("GetGameElementFromServer", new ReceiveGetGameElementFromServer());
		
			//set the application to run in background
			Application.runInBackground = true;
		
			//set the Id of the player
			System.Random random = new System.Random();
			_userId = "Guest" + random.Next(0, int.MaxValue);
		
			//authenticate process
			PlayerIO.Authenticate(
				_gameId,
				ConnectionId,
				new Dictionary<string, string> { { "userId", _userId } },
				null,
				MasterServerJoined,
				delegate (PlayerIOError error) { UI.DebugMessage("Error connecting: " + error.ToString()); }
			);
		}

		private void Update()
		{
			ProcessMessageQueue();
		}
	
		private void OnApplicationQuit()
		{
			PlayerIoConnection?.Send("Chat", "I left !");
		}
	
		/// <summary>
		/// Called when the played has joined the master server
		/// </summary>
		/// <param name="client">The client joining the server</param>
		private void MasterServerJoined(Client client)
		{
			UI.DebugMessage("Successfully connected to Player.IO");
		
			if (_useLocalServer)
			{
				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
				UI.DebugMessage("Successfully created server end point");
			}

			client.Multiplayer.CreateJoinRoom(
				RoomId, 
				RoomType, 
				true,
				null,
				null,
				RoomJoined,
				delegate(PlayerIOError error) { UI.DebugMessage("Error Joining Room: " + error.ToString()); }
			);
		}

		/// <summary>
		/// Called when the client has joined a room
		/// </summary>
		/// <param name="connection">The connection to the playerIO server</param>
		private void RoomJoined(Connection connection)
		{
			UI.DebugMessage("Joined Room.");
		
			PlayerIoConnection = connection;
			PlayerIoConnection.OnMessage += HandleMessage;
			_joinedRoom = true;
		
			PlayerIoConnection.Send("Chat", $"I'm connected !");
		}

		/// <summary>
		/// Called when a message is received
		/// </summary>
		/// <param name="sender">the sender of the message</param>
		/// <param name="m">the message from the server</param>
		private void HandleMessage(object sender, Message m) 
		{
			_messagesList.Add(m);
		}

		/// <summary>
		/// Process the element in the message queue
		/// </summary>
		private void ProcessMessageQueue()
		{
			foreach (Message m in _messagesList)
			{
				if (_receivedMessageToMethod.TryGetValue(m.Type, out ServerMessageReceiver receiver) == false)
				{
					continue;
				}
				receiver.Receive(m);
			}

			_messagesList.Clear();
		}

		/// <summary>
		/// Set the requested turn on the client
		/// </summary>
		/// <param name="turn">the turn to set</param>
		public void SetTurn(int turn)
		{
			UI.DebugMessage($"set turn {turn}");
			Turn = turn;
			UI.SetTurnUI();
		}
	
		/// <summary>
		/// Tell if the player can play 
		/// </summary>
		/// <returns>can the player play ?</returns>
		public bool CanPlay()
		{
			bool isMyTurn = Turn % 2 == Team;
			return isMyTurn;
		}
	}
}
