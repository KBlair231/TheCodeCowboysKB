using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface IGameService {
		GameState GetGameState();

		bool DoesUserHaveSavedGame();
		void CreateCharacter(Player player);
		void StartCombat();
		void RespawnPlayer();
		PQActionResult ExecutePlayerAction(string action);
		PQActionResult ExecuteEnemyAction();
		void StartNewGame();
	}

	public class GameService:IGameService {
		private readonly ISessionService _sessionService;
		private readonly IDatabaseService _databaseService;
		private readonly CombatService _combatService;

		public GameService(IHttpContextAccessor httpContextAccessor,ISessionService sessionService,IDatabaseService databaseService) {
			_sessionService = sessionService;
			_databaseService = databaseService;
			_combatService = new CombatService();
		}

		#region Game State and Session Management Methods

		/// <summary> Adds a new game state to the session, and if the user is logged in, then it is also added to the database with nothing but the user's id. If the user already has a GameState it will be overwritten.</summary>
		public void StartNewGame() {
			GameState gameState = new GameState();
			if(_databaseService.IsAuthenticatedUser()) {
				//User is logged in. store their id in the gamestate and add it to the database.
				//This will overwrite their last game with the new blank one because updates are done on the basis of Google Id.
				gameState.UserGoogleId = _databaseService.GetUserGoogleId();
				_databaseService.AddOrUpdateGameState(gameState);
			}
			//User isn't logged in. Only add the new GameState to the session.
			_sessionService.UpdateGameState(gameState);
		}

		/// <summary> Gets the current game state from the database for logged in users, and from the session for not logged in users.</summary>
		public GameState GetGameState() {
			// Get the current game state from the session.
			GameState gameState = null;
			//Check if user is authenticated
			if(_databaseService.IsAuthenticatedUser()) {
				// Authenticated users get their data from the db
				string userGoogleId = _databaseService.GetUserGoogleId();
				gameState = _databaseService.GetGameState(userGoogleId);
			}
			else {
				// UnAuthenticated users get their data from the current session.
				gameState = _sessionService.GetGameState();
			}
			return gameState;
		}

		/// <summary> Updates the current game state in the session for all users and in the database for logged in users.</summary>
		private void UpdateGameState(GameState gameState) {
			if(_databaseService.IsAuthenticatedUser()) {
				// User is logged in, so update the current gamestate in the database.
				_databaseService.AddOrUpdateGameState(gameState);
			}
			// User is not logged in, so only update the current gamestate in the session.
			_sessionService.UpdateGameState(gameState);
		}

		/// <summary> Returns true if the user has a saved game associated with thier account. Used to enable/disable the continue button</summary>
		public bool DoesUserHaveSavedGame() {
			if(!_databaseService.IsAuthenticatedUser()) {// User isn't logged in.
				return false;
			}
			if (GetGameState() == null) { // User is logged in but doesn't have a saved game.
				return false;
			}
			return true; // User is logged in and has a saved game.
		}

		#endregion Game State and Session Management Methods - End

		#region Update Methods
		/// <summary>Saves the player character to the game state. overwrites the current player character if called multiple times.</summary>
		public void CreateCharacter(Player player) {
			// Get current gamestate
			GameState gameState = GetGameState();
			if(_databaseService.IsAuthenticatedUser()) {
				// Check if user already has a character.
				if(gameState.Player != null) {
					// Delete old character.
					_databaseService.DeletePlayer(gameState.Player.PlayerId);
				}
			}
			// Add new character to the game state.
			gameState.Player = player;
			// Update current gamesate
			UpdateGameState(gameState);
		}
		#endregion Update Methods - End

		#region Game Flow Methods

		/// <summary>Initiates combat with a default enemy.</summary>
		public void StartCombat() {
			// Get current gamestate
			GameState gameState = GetGameState();
			// This pattern feels wrong, we'll figure something better out later.
			if(_databaseService.IsAuthenticatedUser()) {
				// StartCombat adds an enemy so lets delete the old one from the db if there is one.
				if(gameState.Enemy != null) {
					_databaseService.DeleteEnemy(gameState.Enemy.EnemyId);
				}
			}
			// Initiate combat
			_combatService.StartCombat(gameState);
			// Update current gamesate
			UpdateGameState(gameState);
		}

		public void RespawnPlayer() {
			// Get current gamestate from the session
			GameState gameState = GetGameState();
			// Reset player health and potions back to max
			_combatService.RespawnPlayer(gameState);
			// This pattern feels wrong, we'll figure something better out later.
			if(_databaseService.IsAuthenticatedUser()) {
				// StartCombat adds an enemy so lets delete the old one from the db if there is one.
				if(gameState.Enemy != null) {
					_databaseService.DeleteEnemy(gameState.Enemy.EnemyId);
				}
			}
			_combatService.StartCombat(gameState);
			// Update current gamesate in the session
			UpdateGameState(gameState);
		}

		#endregion Game Flow Methods - End

		#region Action Routing Methods

		/// <summary>Execute a player action based on the action string.</summary>
		public PQActionResult ExecutePlayerAction(string action) {
			// Get current gamestate
			GameState gameState = GetGameState();
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
					throw new ArgumentOutOfRangeException(nameof(action), action, null);
			}
			// Update current gamesate
			UpdateGameState(gameState);
			return actionResult;
		}

		/// <summary>Execute an enemy action.  Does not take an action string because the enemy's action is determined server side. </summary>
		public PQActionResult ExecuteEnemyAction() {
			// Get current gamestate
			GameState gameState = GetGameState();
			// Execute the action and return a PQActionResult
			PQActionResult actionResult = _combatService.EnemyAttack(gameState); // Enemy only attacks for now.
																																					 // Update current gamesate
			UpdateGameState(gameState);
			return actionResult;
		}

		#endregion Action Routing Methods - End
	}
}
