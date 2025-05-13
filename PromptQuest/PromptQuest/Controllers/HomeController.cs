using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using PromptQuest.Services;
using System.Diagnostics;

namespace PromptQuest.Controllers {
	public class HomeController:Controller {
		private readonly ILogger<HomeController> _logger;
		private readonly IGameStateService _gameStateService;

		public HomeController(ILogger<HomeController> logger, IGameStateService gameStateService) {
			_logger = logger;
			_gameStateService = gameStateService;
		}

		public async Task<IActionResult> Index() {
			bool userHasSavedGame=await _gameStateService.DoesUserHaveSavedGame();
			ViewBag.UserHasSavedGame = userHasSavedGame;
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
