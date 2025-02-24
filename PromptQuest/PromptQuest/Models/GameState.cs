namespace PromptQuest.Models {
	/// <summary> A model of the state of the game.  Contains all of the relevant information about the game.  </summary>
	public class GameState {
		///<summary> The current user's player character. </summary>
		public Player Player { get; set; }
		///<summary> The current enemy that the player is fighting. </summary>
		public Enemy Enemy { get; set; }
		///<summary> A list of all the messages in the dialog box. </summary>
		public List<string> MessageLog { get; set; } = new List<string>();
		///<summary> Whether or not the player is in combat. </summary>
		public bool InCombat { get; set; } = false;
		///<summary> Whether or not it is the player's turn. </summary>
		public bool IsPlayersTurn {get; set;} = false;
	}

	/// <summary> A partial model of the GameStateModel returned to the view so that it can update what the action changed.  This way we don't have to return the entire GameStateModel. </summary>
	public class PQActionResult {
		///<summary> A message describing the result of the action. </summary>
		public string Message { get; set; } = "";
		///<summary> The player's health after the action. </summary>
		public int PlayerHealth { get; set; } = 0;
		///<summary> The player's number of potions after the action. </summary>
		public int PlayerHealthPotions { get; set; } = 0;
		///<summary> The enemy's health after the action. </summary>
		public int EnemyHealth { get; set; } = 0;
		///<summary> Whether or not the player is in combat. </summary>
		public bool InCombat { get; set; } = false;
		///<summary> Whether or not it is the player's turn. </summary>
		public bool IsPlayersTurn {get; set;} = false;
	}

	/// <summary> Extension methods for the GameState model. </summary>
	public static class GameStateExtensionMethods {

		/// <summary> Creates an ActionResult with a blank Message based on the GameState. </summary>
		public static PQActionResult ToActionResult(this GameState gameState) {
			PQActionResult actionResult = new PQActionResult();
			actionResult.PlayerHealth = gameState.Player.CurrentHealth;
			actionResult.PlayerHealthPotions = gameState.Player.HealthPotions;
			actionResult.EnemyHealth = gameState.Enemy.CurrentHealth;
			actionResult.InCombat = gameState.InCombat;
			actionResult.IsPlayersTurn = gameState.IsPlayersTurn;
			return actionResult;
		}
	}
}
