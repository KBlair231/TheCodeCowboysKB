using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PromptQuest.Models;
using PromptQuest.Services;

namespace PromptQuest.Controllers {

	public class GameController:Controller {
		private readonly ILogger<GameController> _logger;
		private readonly IGameService _gameService;
		private readonly IDallEApiService _dallEApiService;
		private readonly JsonSerializerSettings _jsonSerializerSettings=new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
		public GameController(ILogger<GameController> logger,IGameService gameService,IDallEApiService dallEApiService) {
			_logger = logger;
			_gameService = gameService;
			_dallEApiService = dallEApiService;
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

		public async Task<JsonResult> CreateCharacterImage(string prompt) {
			string image = await _dallEApiService.GenerateImageAsync(prompt);
			return Json(image);
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
		public JsonResult IsTutorial() {
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
		public JsonResult PlayerAction(string playerAction, int actionValue = 0) {
			Action action = new Action(() => { _gameService.ExecutePlayerAction(playerAction, actionValue); });
			return ProcessRequest(action);
		}

		[HttpPost]
		public JsonResult EnemyAction() {
			Action action = new Action(() => { 
				//Add a delay so there is some time between player and enemy actions.
				Thread.Sleep(2000); // Waits for 2 seconds. Blocks current thread (not great, should improve later)
				_gameService.ExecuteEnemyAction();
			});
			return ProcessRequest(action);
		}

		[HttpPost]
		public JsonResult EndTutorial() {
			Action action = new Action(() => { _gameService.SetTutorialFlag(false); });
			return ProcessRequest(action);
		}

		[HttpGet]
		public JsonResult MovePlayerToNextLocation() {
			Action action = new Action(() => { _gameService.ExecutePlayerAction("move"); });
			return ProcessRequest(action);
		}

		[HttpGet]
		public IActionResult GetMap() {
			Map map = _gameService.GetMap();
			return Json(map);
		}

		[HttpGet]
		public IActionResult SkipToBoss() {
			_gameService.SkipToBoss();
			return RedirectToAction("Game");
		}

		[HttpGet]
		public IActionResult SkipToRoom(int targetRoom) {
			_gameService.SkipToRoom(targetRoom);
			return RedirectToAction("Game");
		}

		public JsonResult ProcessRequest(Action action) {
			GameState gameStateBefore = _gameService.GetGameState().CreateDeepCopy(); //Take a snapshot of the current GameState before any changes. Deep copy so it doesn't respond to outside updates
			action.Invoke(); //Perform the requested action
			GameState gameStateAfter = _gameService.GetGameState().CreateDeepCopy(); //Take a snapshot of the current GameState after any changes. Deep copy so it doesn't respond to outside updates
			JsonResult jsonResult = Json(GenerateDiff(gameStateBefore,gameStateAfter));//Generate a diff and convert into json
			return jsonResult;
		}

		public Dictionary<string,object> GenerateDiff(object gameStateBefore,object GameStateAfter) {
			Dictionary<string,object> dictionaryDiff = new Dictionary<string,object>();
			if(gameStateBefore == null || GameStateAfter == null || gameStateBefore.GetType() != GameStateAfter.GetType()) {
				return dictionaryDiff; // Return empty diff if types don't match
			}
			//Loop through each property in the GameState class
			foreach(var prop in gameStateBefore.GetType().GetProperties()) {
				if(prop.Name=="Floor") {
					Console.WriteLine("GameState.Player.Items");
				}
				//Take a snapshot each property before and after changes were made
				var oldValue = prop.GetValue(gameStateBefore);
				var newValue = prop.GetValue(GameStateAfter);
				// Handle collections: Always include the full list if any changed
				if(oldValue is IEnumerable<object> oldCollection && newValue is IEnumerable<object> newCollection) {
					//Change this later to not use serialization, instead compare objects using zip in a helper meathod.
					string oldJson = JsonConvert.SerializeObject(oldCollection);
					string newJson = JsonConvert.SerializeObject(newCollection);
					if(oldJson != newJson) {
						dictionaryDiff[prop.Name] = newCollection; // Include full list only if contents or order changed
					}
				}
				// Handle complex objects (like Player, Enemy)
				else if(oldValue != null && newValue != null && !IsPrimitiveType(prop.PropertyType)) {
					var nestedDiff = GenerateDiff(oldValue,newValue);
					if(nestedDiff.Any()) {
						dictionaryDiff[prop.Name] = nestedDiff; // Only store if changes exist
					}
				}
				else if(!Equals(oldValue,newValue)) {//Property doesn't derive from IEnumerable but did change so include it (if it's a complex object, it still includes all properties not just ones that changed).
					dictionaryDiff[prop.Name] = newValue;
				}
			}
			return dictionaryDiff;
		}

		// Helper function: Detect primitive types correctly
		private static bool IsPrimitiveType(Type type) {
			return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
		}

		/*
			//If the property derives from IEnumerable than include any objects that were changed (includes all properties not just ones that changed).
			if(oldValue is IEnumerable<object> oldCollection && newValue is IEnumerable<object> newCollection) {
				var changedElements = newCollection.Where(newItem =>
						oldCollection.Any(oldItem => !oldItem.Equals(newItem))
				).ToList();
				if(changedElements.Any()) {
					dictionaryDiff[prop.Name] = changedElements;
				}
			}
		*/

	}
}