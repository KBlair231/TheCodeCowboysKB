using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using PromptQuest.Services;
using System.Diagnostics;

namespace PromptQuest.Controllers {
	public class HomeController:Controller {
		private readonly ILogger<HomeController> _logger;
		private readonly IGameService _gameService;

		public HomeController(ILogger<HomeController> logger, IGameService gameService) {
			_logger = logger;
			_gameService = gameService;
		}

		public IActionResult Index() {
			// Fetch the game state for the current session.
			GameState gameState = _gameService.GetGameState();
			// They're a new player, show the main menu
			return View();
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
