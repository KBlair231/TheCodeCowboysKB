using Microsoft.EntityFrameworkCore;
using PromptQuest.Models;
using System;
using System.Numerics;

namespace PromptQuest.Services {
	public interface IDatabaseService {
		GameState GetGameState(string userGoogleId);
		void AddOrUpdateGameState(GameState gameState);
		void DeletePlayer(int playerId);
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

		/// <summary> Retrieves the first gamestate in the database with a matching UserGoogleId. </summary>
		public GameState GetGameState(string userGoogleId) {
			GameState gameState = _dbContext.GameStates
					.Include(g => g.Player)
					.Include(g => g.Enemy)
					.FirstOrDefault(g => g.UserGoogleId == userGoogleId);
			return gameState;
		}

		/// <summary> Adds a GameState to the database, or updates it if the gameState does not exist . </summary>
		public void AddOrUpdateGameState(GameState gameState) {
			// Check if the GameState exists; if not, throw, else update it
			var existingGameState = _dbContext.GameStates
					.Include(gs => gs.Player)
					.Include(gs => gs.Enemy)
					.FirstOrDefault(gs => gs.UserGoogleId == gameState.UserGoogleId);
			if(existingGameState == null) {
				_dbContext.GameStates.Add(gameState);
			}
			else {
				// Update or Add Player if there is one.
				if(gameState.Player != null) {
					var existingPlayer = _dbContext.Players
							.FirstOrDefault(p => p.PlayerId == gameState.Player.PlayerId);
					if(existingPlayer == null) {
						_dbContext.Players.Add(gameState.Player);
					}
				}
				if(gameState.Enemy != null) {
					// Update or Add Enemy if there is one.
					var existingEnemy = _dbContext.Enemies
							.FirstOrDefault(e => e.EnemyId == gameState.Enemy.EnemyId);
					if(existingEnemy == null) {
						_dbContext.Enemies.Add(gameState.Enemy);
					}
				}
			}
			_dbContext.SaveChanges();
		}

		/// <summary> Deletes a Player from the database using the playerId. </summary>
		public void DeletePlayer(int playerId) {
			var player = _dbContext.Players.FirstOrDefault(p => p.PlayerId == playerId);
			if(player != null) {
				_dbContext.Players.Remove(player);
				_dbContext.SaveChanges();
			}
		}

		/// <summary> Deletes an Enemy from the database using the enemyId. </summary>
		public void DeleteEnemy(int enemyId) {
			var enemy = _dbContext.Enemies.FirstOrDefault(e => e.EnemyId == enemyId);
			if(enemy != null) {
				_dbContext.Enemies.Remove(enemy);
				_dbContext.SaveChanges();
			}
		}

		/// <summary> Returns true if the user is authenticated. Otherwise false. </summary>
		public bool IsAuthenticatedUser() {
			var user = _httpContextAccessor.HttpContext.User;
			if(user.Identity.IsAuthenticated) {
				return true;
			}
			return false;
		}

		/// <summary> Get the current User's Id </summary>
		public string GetUserGoogleId() {
			var user = _httpContextAccessor.HttpContext.User;
			var userId = user.FindFirst("GoogleAccountId")?.Value;
			if(userId == null) {
				return "";
			}
			return userId;
		}
	}
}
