using PlayerIO.GameLibrary;
using System;
using System.Collections.Generic;

namespace MushroomsUnity3DExample
{

	public class Player : BasePlayer
	{
	}

	[RoomType("UnityBaseRoom")]
	public class GameCode : Game<Player>
	{
		private const int MaxPlayer = 2;

		private int _gameElementInGameAmount;
		private Dictionary<string, int> _userIdToTeam = new Dictionary<string, int>();

		// This method is called when an instance of your the game is created
		public override void GameStarted()
		{
			Console.WriteLine("Game is started, room : " + RoomId);
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed()
		{
			Console.WriteLine("No more players in room : " + RoomId + ", room closed.");
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player playerJoining)
		{
            //if more than max player, disconnect
            if (base.PlayerCount >= MaxPlayer + 1) 
			{
				playerJoining.Disconnect();
				Console.WriteLine("no more player can join");
				return;
			}

            //if first player, set turn to 0
            if (base.PlayerCount == 1) 
			{
				playerJoining.Send("SetTurn", 0);
			}

            //set the player's team
            int team = base.PlayerCount - 1;
			_userIdToTeam.Add(playerJoining.ConnectUserId, team);
			playerJoining.Send("SetTeam", team);

			//request one of the current player to send the joining player infos
			foreach (Player player in base.Players)
			{
				if (player.ConnectUserId == playerJoining.ConnectUserId)
				{
					continue;
				}

                player.Send("PlayerJoined", playerJoining.ConnectUserId);

				Console.WriteLine("---\nj. SendGameInfosToServer");
                player.Send("SendGameInfosToServer");
                player.Send("SendGameElementsToServer");
                Console.WriteLine("j. SendGameElementsToServer\n---");

				break;
            }
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player)
		{
			player.Disconnect();
			Broadcast("PlayerLeft", player.ConnectUserId);
			Console.WriteLine($"Player {player.ConnectUserId} left");
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player playerSender, Message message)
		{
			switch (message.Type)
			{
				case "Chat":
					string chat = message.GetString(0);
					Console.WriteLine($"{playerSender}: {chat}");
					break;
                case "SetTurn":
                    int turn = message.GetInt(0);
                    foreach (Player player in base.Players)
                    {
                        player.Send("SetTurn", turn);
                    }
                    break;
                case "CreateGameElement":
					string gameElementType = message.GetString(0);
					string gameElementId = _gameElementInGameAmount.ToString();
					string ownerId = playerSender.ConnectUserId;
					int createX = message.GetInt(1);
					int createY = message.GetInt(2);
					int team = _userIdToTeam[ownerId];
					Console.WriteLine($"create game Element from {playerSender.ConnectUserId} : {gameElementType}");
					foreach (Player player in base.Players)
					{
						Console.WriteLine($"-> {player.Id}");
						player.Send("CreateGameElement", gameElementType, gameElementId, ownerId, createX, createY, team);
					}
                    _gameElementInGameAmount++;
					break;
				case "MoveGameElement":
					string gameElementID = message.GetString(0);
					int moveX = message.GetInt(1);
					int moveY = message.GetInt(2);
					Console.WriteLine($"move game Element from {playerSender.ConnectUserId} : {gameElementID} to {moveX},{moveY}");
					foreach (Player player in base.Players)
					{
						player.Send("MoveGameElement", gameElementID, message.GetInt(1), message.GetInt(2));
					}
					break;
                case "DestroyGameElement":
                    string destroyGameElementID = message.GetString(0);
                    foreach (Player player in base.Players)
                    {
                        player.Send("DestroyGameElement", destroyGameElementID);
                    }
                    break;
                case "SendGameElementToPlayers":
                    Console.WriteLine("received : SendGameElementToPlayers");
                    string getGameElementType = message.GetString(0);
					string getGameElementtId = message.GetString(1);
					string getGameElementOwnerId = message.GetString(2);
					int getGameElementX = message.GetInt(3);
					int getGameElementY = message.GetInt(4);
					int getGameElementTeam = message.GetInt(5);
					foreach (Player player in base.Players)
					{
						if (player.ConnectUserId == playerSender.ConnectUserId)
						{
							continue;
						}
						Console.WriteLine($"-> send game elemnt to player {player.Id}");
						player.Send("GetGameElementFromServer", getGameElementType, getGameElementtId, getGameElementOwnerId, getGameElementX, getGameElementY, getGameElementTeam);
					}
					break;
				case "SendGameInfosToPlayers":
					Console.WriteLine("received : SendGameInfosToPlayers");
					int turnInfo = message.GetInt(0);
					foreach (Player player in base.Players)
					{
						if (player.ConnectUserId == playerSender.ConnectUserId)
						{
							continue;
						}
						player.Send("GetGameInfosFromServer", turnInfo);
					}
					break;
			}
		}
	}
}