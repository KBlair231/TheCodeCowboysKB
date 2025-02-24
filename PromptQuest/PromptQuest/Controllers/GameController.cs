using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using PromptQuest.Services;

namespace PromptQuest.Controllers {

	public class GameController:Controller {
		private readonly ILogger<GameController> _logger;
		private readonly IGameService _gameService;
		public GameController(ILogger<GameController> logger, IGameService gameService) {
			_logger = logger;
			_gameService = gameService;
		}

		[HttpGet]
		public IActionResult CreateCharacter() {
			return View();
		}

		[HttpPost]
		public IActionResult CreateCharacter(Player player) {
			// Default stats for now.
			player.MaxHealth = 10;
			player.CurrentHealth = 10;
			player.HealthPotions = 2;
			player.Attack = 1;
			if(ModelState.IsValid) { // Character created succesfully
				_gameService.ResetGameState(); // Wipe any session data becuase they are starting a new character
				_gameService.UpdatePlayer(player); // Add player to the game state.
				_gameService.StartCombat(); // Start combat right away, for now.
				return RedirectToAction("Game");
			}
			else {
				return View();
			}
		}

		[HttpGet]
		public IActionResult Game() {
			return View();
		}

		[HttpGet]
		public JsonResult GetGameState() {
			GameState gameState = _gameService.GetGameState();
			// Return the entire game state.
			return Json(gameState);
		}

		[HttpPost]
		public IActionResult PlayerAction(string action) {
			PQActionResult ActionResult = _gameService.ExecutePlayerAction(action);
			return Json(ActionResult);
		}

		[HttpPost]
		public IActionResult EnemyAction() {
			PQActionResult ActionResult = _gameService.ExecuteEnemyAction();
			return Json(ActionResult);
		}
	}
}