using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IGameStateService {
		/// <summary> Grabs the GameState stored in the current session. Does not return null, throws exceptions. </summary>
		GameState GetGameState();
		/// <summary> Loads the user's GameState from the db into the session. Returns true if successful. This method should be called before the first call of GetGameState(). </summary>
		Task<bool> LoadGameStateAsync();
		/// <summary> Synchronously update the GameState in the session </summary>
		void UpdateGameState(GameState gameState);
		/// <summary> Asynchronously update the saved GameState in the db. This should be called before UpdateGameState when a new GameState is created. </summary>
		Task<bool> SaveGameStateAsync(GameState gameState);
		/// <summary> Adds a new game state to the session, and if the user is logged in, then it is also added to the database with nothing but the user's id. If the user already has a GameState it will be overwritten.</summary>
		Task<GameState> StartNewGame(Player player);
		/// <summary> Deletes the Enemy with the given EnemyId. If it isn't found, nothing happens. </summary>
		void DeleteEnemy(int enemyId);
		/// <summary> Determines if the user has a saved game or not. Primarily used in the HomeController to activate the Continue button. </summary>
		Task<bool> DoesUserHaveSavedGame();
	}

	public class GameStateService:IGameStateService {
		private readonly GameStateDbContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string GameStateSessionKey = "GameState";

		public GameStateService(GameStateDbContext dbContext,IHttpContextAccessor httpContextAccessor) {
			_dbContext = dbContext;
			_httpContextAccessor = httpContextAccessor;
		}

		#region Getters and Setters
		public GameState GetGameState() {
			if(_httpContextAccessor.HttpContext == null) {
				throw new Exception("Error reading GameState from session: _httpContextAccessor.HttpContext was null.");
			}
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = session.GetString(GameStateSessionKey);
			if(string.IsNullOrEmpty(gameStateJson)) {
				throw new Exception("Error reading GameState from session: No GameState saved in session");
			}
			GameState? gameState = JsonSerializer.Deserialize<GameState>(gameStateJson);
			if(gameState == null) {
				throw new Exception("Error reading GameState from session: Could not deserialize GameState json from the session");
			}
			return gameState;
		}

		public void UpdateGameState(GameState gameState) {
			if(_httpContextAccessor.HttpContext == null) {
				throw new Exception("Error writing GameState to session: _httpContextAccessor.HttpContext was null.");
			}
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = JsonSerializer.Serialize(gameState);
			session.SetString(GameStateSessionKey,gameStateJson);
		}

		public async Task<bool> SaveGameStateAsync(GameState gameState) {
			if(!IsAuthenticatedUser()) {
				return false; //We can't save data for users that aren't logged in.
			}
			var existingGameState = await FetchGameStateFromDb(gameState.UserGoogleId);
			if(existingGameState == null) {
				await _dbContext.GameStates.AddAsync(gameState);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			UpdateGameState(existingGameState,gameState);
			UpdatePlayer(existingGameState.Player,gameState.Player);
			UpdateEnemy(existingGameState.Enemy,gameState.Enemy);
			UpdateItems(existingGameState.Player.Items,gameState.Player.Items,existingGameState.Player.PlayerId);
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> LoadGameStateAsync() {
			if(!IsAuthenticatedUser()) {
				return false; //User isn't logged in
			}
			//The users is logged in, grab their GameState based on their GoogleAccountId
			string userGoogleId = GetGoogleAccountId();
			var gameState = await FetchGameStateFromDb(userGoogleId);
			if(gameState == null) {
				return false; //User doesn't have a GameState saved in the db.
			}
			UpdateGameState(gameState);//Store the retrieved GameState in the current session.
			return true;
		}
		#endregion

		#region Crud Operations
		public async Task<GameState> StartNewGame(Player player) {
			//Delete current user's saved GameState if they have one. All other deletes should cascade from this.
			await DeleteGameState();
			//Create fresh game state
			GameState gameState = new GameState();
			gameState.Player = player;
			//Give the player the default Items. This will go away eventually.
			player.Items.AddRange(new List<Item>{
				new Item { Name = "Jeweled Boots", Attack = 0, Defense = 1, itemType = ItemType.Boots, ImageSrc = "/images/BaseBoots.png"},
				new Item { Name = "Jeweled Chestplate", Attack = 0, Defense = 2, itemType = ItemType.Chest, ImageSrc = "/images/BaseChest.png"},
				new Item { Name = "Jeweled Leggings", Attack = 0, Defense = 1, itemType = ItemType.Legs, ImageSrc = "/images/BaseLegs.png"},
				new Item { Name = "Jeweled Helmet", Attack = 0, Defense = 1, itemType = ItemType.Helm, ImageSrc = "/images/BaseHelm.png"},
				new Item { Name = "Fiery Sword", Attack = 2, Defense = 0, StatusEffects = StatusEffect.Burning, ImageSrc = "/images/PlaceholderItem2.png"},
				new Item { Name = "Frozen Shield", Attack = 1, Defense = 3, ImageSrc = "/images/PlaceholderItem3.png"},
				new Item { Name = "Warded Sword", Attack = 3, Defense = 2, ImageSrc = "/images/PlaceholderItem4.png"}
			});
			gameState.UserGoogleId = GetGoogleAccountId();//Store their id in the gamestate (blank if user isn't authenticated).
			gameState.InTutorial = true;//New games start with the tutorial
			gameState.Enemy = new Enemy();
			return gameState;
		}

		/// <summary> Deletes the GameState with the given GoogleUserId. If it isn't found, nothing happens. </summary>
		private async Task DeleteGameState() {
			string userGoogleId = GetGoogleAccountId();
			var existingGameState = await FetchGameStateFromDb(userGoogleId);
			if(existingGameState == null) {
				return;//There is no GameState in the db for this user. So, do nothing.
			}
			//Manually delete related items because EF's cascade delete be buggin'
			_dbContext.Items.RemoveRange(existingGameState.Player.Items);
			_dbContext.Players.Remove(existingGameState.Player);
			_dbContext.Enemies.Remove(existingGameState.Enemy);
			_dbContext.GameStates.Remove(existingGameState);
			_dbContext.SaveChanges();
		}

		public void DeleteEnemy(int enemyId) {
			var enemy = _dbContext.Enemies.Find(enemyId);
			if(enemy == null) {
				return;//This Enemy doesn't exist in the db. So, do nothing.
			}
			_dbContext.Enemies.Remove(enemy);
			_dbContext.SaveChanges();
		}
		#endregion

		#region Db Helper Methods
		///<summary> Private helper to eagerly load the GameState for the current user. Can return null. </summary>
		private async Task<GameState?> FetchGameStateFromDb(string userGoogleId) {
			return await _dbContext.GameStates
					.Include(gs => gs.Player)
					.ThenInclude(p => p.Items)
					.Include(gs => gs.Enemy)
					.FirstOrDefaultAsync(gs => gs.UserGoogleId == userGoogleId);
		}

		///<summary> Private helper to update GameState </summary>
		private void UpdateGameState(GameState existing,GameState updated) {
			if(existing == null || updated == null) {
				return;
			}
			//Preserve primary key
			updated.UserGoogleId = existing.UserGoogleId;
			//Preserve foreign key
			updated.PlayerId = existing.PlayerId;
			updated.EnemyId = existing.EnemyId;
			//Update the rest of the values
			_dbContext.Entry(existing).CurrentValues.SetValues(updated);
		}

		///<summary> Private helper to update Player </summary>
		private void UpdatePlayer(Player existing,Player updated) {
			if(existing == null || updated == null) {
				return;
			}
			//Preserve primary key
			updated.PlayerId = existing.PlayerId;
			//Update the rest of the values
			_dbContext.Entry(existing).CurrentValues.SetValues(updated);
		}

		///<summary> Private helper to update Enemy </summary>
		private void UpdateEnemy(Enemy existing,Enemy updated) {
			if(existing == null || updated == null) {
				return;
			}
			//Preserve primary key
			updated.EnemyId = existing.EnemyId;
			//Update the rest of the values
			_dbContext.Entry(existing).CurrentValues.SetValues(updated);
		}

		///<summary> Private helper to update Player's Item list </summary>
		private void UpdateItems(ICollection<Item> existingItems,ICollection<Item> updatedItems,int playerId) {
			//Wipe out current items
			existingItems.Clear();
			//Replace them with new ones
			foreach(var item in updatedItems) {
				//Preserve foreign key
				item.PlayerId = playerId;
				existingItems.Add(item);
			}
		}
		#endregion

		#region Identity helper methods
		/// <summary> Returns true if the current user is authenticated. </summary>
		private bool IsAuthenticatedUser() {
			return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
		}

		/// <summary> Get the currently logged in user's GoogleAccountId from the session. Returns "" if the users isn't logged in or it isn't found for some reason. </summary>
		private string GetGoogleAccountId() {
			if(!IsAuthenticatedUser()) {
				return "";//User isn't logged in so we don't know their GoogleAccountId.
			}
			return _httpContextAccessor.HttpContext?.User.FindFirst("GoogleAccountId")?.Value??"";
		}

		/// <summary> Returns true if the current user has a saved game in the database. </summary>
		public async Task<bool> DoesUserHaveSavedGame() {
			string googleAccountId = GetGoogleAccountId();
			if(googleAccountId == "") {
				return false;//User isn't logged in so we can't know if they have a saved GameState in the db.
			}
			//Fetch GameState just to see if there is one.
			var existingGameState = await _dbContext.GameStates.FirstOrDefaultAsync(gs => gs.UserGoogleId == googleAccountId);
			if(existingGameState == null) {
				return false;//Couldn't find a GameState saved in the db for this user.
			}
			return true;
		}
		#endregion
	}
}