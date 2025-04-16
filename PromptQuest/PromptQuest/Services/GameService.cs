using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface IGameService {
		GameState GetGameState();

		bool DoesUserHaveSavedGame();
		void StartCombat();
		void RespawnPlayer();
		PQActionResult ExecutePlayerAction(string action);
		PQActionResult ExecuteEnemyAction();
		PQActionResult SkipToBoss();
		void EquipItem(int itemIndex);
		void StartNewGame(Player player);
		public bool IsTutorial();
		public void SetTutorialFlag(bool Flag);
		public Map GetMap();
		 List<Item> GetDefaultItems();
	}

	public class GameService : IGameService {
		private readonly ISessionService _sessionService;
		private readonly IDatabaseService _databaseService;
		private readonly ICombatService _combatService;
		private readonly IMapService _mapService;

		public GameService(ISessionService sessionService, IDatabaseService databaseService, ICombatService combatService, IMapService mapService) {
			_sessionService = sessionService;
			_databaseService = databaseService;
			_combatService = combatService;
			_mapService = mapService;
		}

		#region Game State and Session Management Methods

		/// <summary> Adds a new game state to the session, and if the user is logged in, then it is also added to the database with nothing but the user's id. If the user already has a GameState it will be overwritten.</summary>
		public void StartNewGame(Player player) {
			GameState gameState = GetGameState();
			if(DoesUserHaveSavedGame()) {
				//They do. Delete it.
				string userGoogleId = _databaseService.GetUserGoogleId();
				_databaseService.DeleteGameState(userGoogleId); //All other deletes should cascade from this.
			}
			//Create fresh game state
			gameState = new GameState();
			gameState.Player = player;
			gameState.Enemy = new Enemy(); // Avoids null references later.
			//Store their id in the gamestate (blank if user isn't authenticated).
			gameState.UserGoogleId = _databaseService.GetUserGoogleId();
			UpdateGameState(gameState);
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
			//Add default Items if they have none
			if(gameState != null && gameState.Player.Items.Count == 0) {
				gameState.Player.Items.AddRange(GetDefaultItems());
			}
			return gameState;
		}
		public bool IsTutorial() {
			return _sessionService.GetTutorialFlag();
		}
		public void SetTutorialFlag(bool Flag) {
			_sessionService.SetTutorialFlag(Flag);
		}

		/// <summary> Updates the current game state in the session for all users and in the database for logged in users.</summary>
		private void UpdateGameState(GameState gameState) {
			if(_databaseService.IsAuthenticatedUser()) {
				// User is logged in, so update the current gamestate in the database.
				_databaseService.SaveGameState(gameState);
			}
			// User is not logged in, so only update the current gamestate in the session.
			_sessionService.UpdateGameState(gameState);
		}

		/// <summary> Returns true if the user has a saved game associated with thier account. Used to enable/disable the continue button</summary>
		public bool DoesUserHaveSavedGame() {
			if(!_databaseService.IsAuthenticatedUser()) {// User isn't logged in.
				return false;
			}
			if(GetGameState() == null) { // User is logged in but doesn't have a saved game.
				return false;
			}
			return true; // User is logged in and has a saved game.
		}

		#endregion Game State and Session Management Methods - End

		#region Get Methods

		public Map GetMap() {
			return _mapService.GetMap();
		}
		#endregion Get Methods - End

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
			// Restart the player at the first location.
			gameState.PlayerLocation = 1;
			// Start a new fight.
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
			string message = "";
			switch(action.ToLower()) {
				case "attack":
					message += _combatService.PlayerAttack(gameState);
					break;
				case "heal":
					message += _combatService.PlayerUseHealthPotion(gameState);
					break;
				case "move":
					_mapService.MovePlayer(gameState);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action), action, null);
			}
			// Update current gamesate
			UpdateGameState(gameState);
			PQActionResult pQActionResult = gameState.ToPQActionResult();
			pQActionResult.Message = message;
			return pQActionResult;
		}

		#region Inventory functions (could be placed in its own service at some point when there is more of them)
		/// <summary> equips the item with the given itemId </summary>
		public void EquipItem(int itemIndex)
		{
			GameState gameState = GetGameState();
			//Mark currently equipped item as unequipped if there is one.
			Item item=gameState.Player.Items.FirstOrDefault(i => i.Equipped);
			if(item!=null) {
				item.Equipped=false;
			}
			//Mark new item as equipped.
			item=gameState.Player.Items[itemIndex];
			item.Equipped=true;
			UpdateGameState(gameState);
		}

		public List<Item> GetDefaultItems() {
			return new List<Item> { //Default items
				new Item { Name = "Jeweled Helmet", Attack = 0, Defense = 2, ImageSrc = "/images/PlaceholderItem1.png", },
				new Item { Name = "Fiery Sword", Attack = 4, Defense = 0, ImageSrc = "/images/PlaceholderItem2.png", },
				new Item { Name = "Frozen Shield", Attack = 1, Defense = 3, ImageSrc = "/images/PlaceholderItem3.png", },
				new Item { Name = "Warded Sword", Attack = 3, Defense = 2, ImageSrc = "/images/PlaceholderItem4.png", }
			};
		}

		#endregion

		/// <summary>Execute an enemy action.  Does not take an action string because the enemy's action is determined server side. </summary>
		public PQActionResult ExecuteEnemyAction() {
			// Get current gamestate
			GameState gameState = GetGameState();
			// Execute the action and return a PQActionResult
			string message = _combatService.EnemyAttack(gameState); // Enemy only attacks for now.
			// Update current gamesate
			UpdateGameState(gameState);
		  PQActionResult pQActionResult = gameState.ToPQActionResult();
			pQActionResult.Message = message;
			return pQActionResult;
		}

		public PQActionResult SkipToBoss() {				// This is a skip to the boss for testing purposes
			// Get current gamestate
			GameState gameState = GetGameState();
			// Move the player to the room before the boss.
			_mapService.MovePlayer(gameState, 9);
			_combatService.StartCombat(gameState);//Make sure combat starts when they get there or you could get stuck their.
			// Update current gamesate
			UpdateGameState(gameState);
			PQActionResult pQActionResult = gameState.ToPQActionResult();
			pQActionResult.Message = "You have been teleported to the room before the boss.";
			return pQActionResult;
		}

		#endregion Action Routing Methods - End
	}
}
