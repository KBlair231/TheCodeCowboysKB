using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using PromptQuest.Services;

namespace PromptQuest.Controllers {

	public class GameController : Controller {
		private readonly ILogger<GameController> _logger;
		private readonly IGameService _gameService;
		public GameController(ILogger<GameController> logger, IGameService gameService) {
			_logger = logger;
			_gameService = gameService;
		}
		public IActionResult About() {
			return View();
		}
		[HttpGet]
		public IActionResult CreateCharacter() {
			return View();
		}

		[HttpPost]
		public IActionResult CreateCharacter(Player player) {
			// Default stats for now.
			player.MaxHealth = 15;
			player.CurrentHealth = 15;
			player.HealthPotions = 2;
			player.Attack = 3;
			player.Defense = 1;
			player.Class = player.Class;
			if(ModelState.IsValid) { // Character created succesfully
				_gameService.StartNewGame(player); // Start a new game. If the user already has one it will be overwritten.
				_gameService.StartCombat(); // Start combat right away, for now.
				_gameService.SetTutorialFlag(true); // New game, so start the tutorial.
				return RedirectToAction("Game");
			}
			else {
				return View();
			}
		}

		[HttpGet]
		public IActionResult Continue() {
			//Assume the user has already completed the tutorial, if they haven't then sucks to suck.
			_gameService.SetTutorialFlag(false);
			return RedirectToAction("Game");
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
		[HttpGet]

		public JsonResult IsTutorial()
		{
			bool flag = _gameService.IsTutorial();
			// Return the entire game state.
			return Json(flag);
		}

		[HttpGet]
		public JsonResult GetGameSaveStatus() {
			// This method feels misplaced here. Just used to check if the continue button needs to be enabled or not.
			if(_gameService.DoesUserHaveSavedGame()) {
				return Json(true);
			}
			return Json(false);
		}

		[HttpPost]
		public IActionResult PlayerAction(string action) {
			PQActionResult ActionResult = _gameService.ExecutePlayerAction(action);
			return Json(ActionResult);
		}

		[HttpPost]
		public void EquipItem(int itemIndex)
		{
			_gameService.EquipItem(itemIndex);
		}

		[HttpPost]
		public IActionResult EnemyAction() {
			PQActionResult ActionResult = _gameService.ExecuteEnemyAction();
			return Json(ActionResult);
		}

		[HttpPost]
		public void StartCombat() {
			_gameService.StartCombat();
		}

		[HttpPost]
		public IActionResult EndTutorial()
		{
			_gameService.SetTutorialFlag(false);
			return Ok();
		}

		[HttpPost]
		public IActionResult Respawn()
		{
			_gameService.RespawnPlayer();
			return Ok();
		}

		[HttpPost]
		public IActionResult MovePlayerToNextLocation()
		{
			PQActionResult ActionResult = _gameService.ExecutePlayerAction("move");
			return Json(ActionResult);
		}

		[HttpGet]
		public IActionResult GetMap()
		{
			Map map = _gameService.GetMap();
			return Json(map);
		}

		[HttpGet]
		public IActionResult SkipToBoss()
		{
			_gameService.SkipToBoss();
			return RedirectToAction("Game");
		}
	}
}