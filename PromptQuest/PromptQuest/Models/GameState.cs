using Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using static PromptQuest.Models.GameState;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;

namespace PromptQuest.Models {
	/// <summary> A model of the state of the game.  Contains all of the relevant information about the game.  </summary>
	public class GameState {
		/// <summary> Primary Key. The Google ID of the user that owns this game. </summary>
		public string UserGoogleId { get; set; } = "";
		/// <summary> Foreign Key </summary>
		public int? PlayerId { get; set; }
		///<summary> The current user's player character. </summary>
		public Player Player { get; set; } = new Player();
		/// <summary> Foreign Key </summary>
		public int? EnemyId { get; set; }
		///<summary> The current enemy that the player is fighting. </summary>
		public Enemy Enemy { get; set; }
		/// <summary> For storing messages in the db. Contains ListMessages serialized into json </summary>
		public string StoredMessages { get; set; } = "[]";
		/// <summary> Not stored directly in the db and readonly. Changes made to this via GameState.AddMessag() are serialized and saved in the db under the StoredMessages column. </summary>
		[NotMapped]
		public List<string> ListMessages => JsonConvert.DeserializeObject<List<string>>(StoredMessages) ?? new List<string>(); //Deserialize messages stored in the db into a List of strings.
		///<summary> Whether or not the player is in combat. </summary>
		public bool InCombat { get; set; } = false;
		///<summary> Whether or not the player is in a campsite. </summary>
		public bool InCampsite { get; set; } = false;
		///<summary> Whether or not the player is in an event. </summary>
		public bool InEvent { get; set; } = false;
		///<summary> Whether or not the player is in a treasure node. </summary>
		public bool InTreasure { get; set; } = false;
		///<summary> Whether or not it is the player's turn. </summary>
		public bool IsPlayersTurn { get; set; } = false;
		///<summary> The mapNodeId of the mapNode the player is currently at. </summary>
		public int PlayerLocation { get; set; } = 1;
		///<summary> Whether or not the player has completed the current area </summary>
		public bool IsLocationComplete { get; set; } = false;
		///<summary> The current floor the player is on. </summary>
		public int Floor { get; set; } = 1;
	}

	/// <summary> Extension methods for the GameState model. </summary>
	public static class GameStateExtensionMethods {
		/// <summary> Adds a new message that will be show to the user. Max number is 50 for now.</summary>
		public static void AddMessage(this GameState gameState, string message) {
			List<string> listMessages = gameState.ListMessages.TakeLast(10).ToList(); //Keep last 10 messages only
			listMessages.Add(message); //Add the new one.
			gameState.StoredMessages = JsonConvert.SerializeObject(listMessages); //Serialize the list of messages into json so they can be stored in the db.
		}

		/// <summary> Adds a new message that will be show to the user. Max number is 50 for now.</summary>
		public static void ClearMessages(this GameState gameState) {
			List<string> listMessages = new List<string>(); //Overwrite stored messages by passing in a blank list
			gameState.StoredMessages = JsonConvert.SerializeObject(listMessages); //Serialize the list of messages into json so they can be stored in the db.
		}

		/// <summary> Returns a deep copy of this GameState. </summary>
		public static GameState CreateDeepCopy(this GameState gameState) {
			//Serialize the object than deserialize it to create a deep copy of it.
			var json = JsonConvert.SerializeObject(gameState);
			return JsonConvert.DeserializeObject<GameState>(json);
		}
	}
	/// <summary> Bitwise Enum for Status Effects. </summary>
	public enum StatusEffect {
		None = 0,
		Bleeding = 1,
		Burning = 2
	}
}