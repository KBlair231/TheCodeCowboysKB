using Microsoft.EntityFrameworkCore;
using PromptQuest.Models;
using System.Security.Claims;
using System.Text.Json;

namespace PromptQuest.Services {

	public interface ISessionService {
		public GameState GetGameState();
		public void UpdateGameState(GameState gameState);
		public bool GetTutorialFlag();
		public void SetTutorialFlag(bool TutorialFlag);
	}

	public class SessionService:ISessionService {
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string GameStateSessionKey = "GameState";
		private const string TutorialFlagSessionKey = "TutorialFlag";

		public SessionService(IHttpContextAccessor httpContextAccessor) {
			_httpContextAccessor = httpContextAccessor;
		}

		/// <summary>Returns the current gamestate. Used for loading the game view.</summary>
		public GameState GetGameState() {
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = session.GetString(GameStateSessionKey);
			if(gameStateJson != null) {
				return JsonSerializer.Deserialize<GameState>(gameStateJson);
			}
			return new GameState();
		}
		public bool GetTutorialFlag(){
			var session = _httpContextAccessor.HttpContext.Session;
			var tutorialFlagJson = session.GetString(TutorialFlagSessionKey);
			if (tutorialFlagJson != null)
			{
				return JsonSerializer.Deserialize<bool>(tutorialFlagJson);
			}
			return false;
		}
		/// <summary> Update the game state stored inside current session. </summary>
		public void UpdateGameState(GameState gameState) {
			var session = _httpContextAccessor.HttpContext.Session;
			var gameStateJson = JsonSerializer.Serialize(gameState);
			session.SetString(GameStateSessionKey,gameStateJson);
		}
		public void SetTutorialFlag(bool TutorialFlag)
		{
			var session = _httpContextAccessor.HttpContext.Session;
			var tutorialFlagJson = JsonSerializer.Serialize(TutorialFlag);
			session.SetString(TutorialFlagSessionKey, tutorialFlagJson);
		}
	}
}
