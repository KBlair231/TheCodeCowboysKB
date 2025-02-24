using System.Text.Json;
using System.Threading.Tasks;
using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface IGameService {
		GameState GetGameState();
		void ResetGameState();
		void UpdatePlayer(Player player);
		void StartCombat();
		PQActionResult ExecutePlayerAction(string action);
		PQActionResult ExecuteEnemyAction();
	}

	public class GameService:IGameService{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly CombatService _combatService;
		private const string GameStateSessionKey = "GameState";

		public GameService(IHttpContextAccessor httpContextAccessor) {
			_httpContextAccessor = httpContextAccessor;
			_combatService = new CombatService();
		}

		#region Game State and Session Management Methods

		/// <summary> Get the game state stored inside current session. </summary>
		private GameState GetSession() {
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = session.GetString(GameStateSessionKey);
			return gameStateJson != null ? JsonSerializer.Deserialize<GameState>(gameStateJson) : new GameState();
		}

		/// <summary> Update the game state stored inside current session. </summary>
		private void UpdateSession(GameState gameState) {
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = JsonSerializer.Serialize(gameState);
			session.SetString(GameStateSessionKey,gameStateJson);
		}

		/// <summary> Resets the current game state and updates the session.  </summary>
		public void ResetGameState() {
			var session = _httpContextAccessor.HttpContext.Session;
			session.Remove(GameStateSessionKey);
			var newGameState = new GameState();
			UpdateSession(newGameState);
		}

		#endregion Game State and Session Management Methods - End

		#region Get Methods

		/// <summary>Returns the current gamestate. Used for loading the game view.</summary>
		public GameState GetGameState() {
			return GetSession();
		}

		#endregion Get Methods - End

		#region Update Methods
		/// <summary>Updates the current player character. Also functions as a create character method if one does not exist yet.</summary>
		public void UpdatePlayer(Player playerModel) {
			// Get current gamestate from the session
			GameState gameState = GetSession();
			// Override current playerModel with the new one.
			gameState.Player = playerModel;
			// Update current gamesate in the session
			UpdateSession(gameState);
		}
		#endregion Update Methods - End

		#region Game Flow Methods

		/// <summary>Initiates combat with a default enemy.</summary>
		public void StartCombat() {
			// Get current gamestate from the session
			GameState gameState = GetSession();
			// Initiate combat
			_combatService.StartCombat(gameState);
			// Update current gamesate in the session
			UpdateSession(gameState);
		}

		#endregion Game Flow Methods - End

		#region Action Routing Methods

		/// <summary>Execute a player action based on the action string.</summary>
		public PQActionResult ExecutePlayerAction(string action) {
			// Get current gamestate from the session
			GameState gameState = GetSession();
			// Determine which action it is and execute it, then return a PQActionResult
			PQActionResult actionResult;
			switch(action.ToLower()) {
				case "attack":
					actionResult = _combatService.PlayerAttack(gameState);
					break;
				case "heal":
					actionResult = _combatService.PlayerUseHealthPotion(gameState);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action),action,null);
			}
			// Update current gamesate in the session
			UpdateSession(gameState);
			return actionResult;
		}

		/// <summary>Execute an enemy action.  Does not take an action string because the enemy's action is determined server side. </summary>
		public PQActionResult ExecuteEnemyAction() {
			// Get current gamestate from the session
			GameState gameState = GetSession();
			// Execute the action and return a PQActionResult
			PQActionResult actionResult = _combatService.EnemyAttack(gameState); // Enemy only attacks for now.
			// Update current gamesate in the session
			UpdateSession(gameState);
			return actionResult;
		}

		#endregion Action Routing Methods - End
	}
}
