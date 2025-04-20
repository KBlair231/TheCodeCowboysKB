using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IDatabaseService {
		GameState GetGameState(string userGoogleId);
		void SaveGameState(GameState gameState);
		void DeleteGameState(string userGoogleId);
		void DeleteEnemy(int enemyId);
		bool IsAuthenticatedUser();
		string GetUserGoogleId();
	}

	public class DatabaseService:IDatabaseService {
		private readonly GameStateDbContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DatabaseService(GameStateDbContext dbContext,IHttpContextAccessor httpContextAccessor) {
			_dbContext = dbContext;
			_httpContextAccessor = httpContextAccessor;
		}

		public GameState GetGameState(string userGoogleId) {
			return _dbContext.GameStates
					.Include(gs => gs.Player)
					.ThenInclude(p => p.Items)
					.Include(gs => gs.Enemy)
					.FirstOrDefault(gs => gs.UserGoogleId == userGoogleId);
		}

		public void SaveGameState(GameState gameState) {
			//Check if game state already exists for this user
			var existingGameState = _dbContext.GameStates.FirstOrDefault(gs => gs.UserGoogleId == gameState.UserGoogleId);
			if(existingGameState == null) {
				//GameState hasn't been saved yet, add it to the db
				_dbContext.GameStates.Add(gameState);
			}
			//GameState already exists or was added for this user, save these changes made to the db context.
			_dbContext.SaveChanges();
		}

		/// <summary> Deletes the GameState with the given GoogleUserId. If it isn't found, nothing happens. </summary>
		public void DeleteGameState(string userGoogleId) {
			var gameState = GetGameState(userGoogleId);
			if(gameState != null) {
				_dbContext.Items.RemoveRange(gameState.Player.Items); // Manually delete related items because cascade delete be buggin'
				_dbContext.Players.Remove(gameState.Player);
				_dbContext.Enemies.Remove(gameState.Enemy);
				_dbContext.GameStates.Remove(gameState);
				_dbContext.SaveChanges();
			}
		}

		/// <summary> Deletes the Enemy with the given EnemyId. If it isn't found, nothing happens. </summary>
		public void DeleteEnemy(int enemyId) {
			var enemy = _dbContext.Enemies.Find(enemyId);
			if(enemy != null) {
				_dbContext.Enemies.Remove(enemy);
				_dbContext.SaveChanges();
			}
		}

		/// <summary> Returns true if the current user is authenticated. </summary>
		public bool IsAuthenticatedUser() {
			return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
		}

		/// <summary> Returns the current users GoogleAccountId if they are authenticated. If the user isn't authenticated, returns a blank string </summary>
		public string GetUserGoogleId() {
			return _httpContextAccessor.HttpContext?.User?.FindFirst("GoogleAccountId")?.Value ?? string.Empty;
		}
	}
}
