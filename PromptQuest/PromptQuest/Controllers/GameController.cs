using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PromptQuest.Models;
using PromptQuest.Services;
using System.Numerics;

namespace PromptQuest.Controllers {

	public class GameController : Controller {
		private readonly ILogger<GameController> _logger;
		private readonly IGameStateService _gameStateService;
		private readonly ICombatService _combatService;
		private readonly IMapService _mapService;
		private readonly IDallEApiService _dallEApiService;
		private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };

		public GameController(ILogger<GameController> logger, IGameStateService gameStateService, ICombatService combatService, IMapService mapService, IDallEApiService dallEApiService) {
			_logger = logger;
			_gameStateService = gameStateService;
			_combatService = combatService;
			_mapService = mapService;
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
		public async Task<IActionResult> CreateCharacter(Player player) {
			// Default stats for now.
			player.MaxHealth = 75;
			player.CurrentHealth = 75;
			player.HealthPotions = 2;
			player.BaseAttack = 3;
			player.BaseDefense = 0;
			player.Image=_dallEApiService.GetImageDataFromSession();//Grab the Player's generated image data. Returns "" if there is none.
			if(ModelState.IsValid) { // Character created succesfully
				GameState gameState = await _gameStateService.StartNewGame(player); // Start a new game. If the user already has one it will be overwritten.
				_combatService.StartCombat(gameState); // Start combat right away, for now.
				await _gameStateService.SaveGameStateAsync(gameState);//Save it to the db first so that Id's are accurate in the session.
				_gameStateService.UpdateGameState(gameState);//Store new GameState in the session.
				return RedirectToAction("Game");
			}
			else {
				return View(player); //Send it back so they can try again.
			}
		}

		public async Task CreateCharacterImage(string prompt) {
			string imageData = await _dallEApiService.GenerateImageAsync(prompt);
			_dallEApiService.StoreImageDataInSession(imageData);//Store the preview image in session so that it can be retrieved when we save the character.
		}

		/// <summary> Serves up the player's image as a file instead of a base64 string. Set isPreview to true, if you want to fetch the preview image from the session instead of Player.Image the db </summary>
		public IActionResult GetCharacterImage(bool isPreview = false) {
			string imageData="";
			if(isPreview) {
				imageData = _dallEApiService.GetImageDataFromSession();//Grab the Player's generated image data.
				if(!string.IsNullOrEmpty(imageData)) {
					byte[] imageBytes = Convert.FromBase64String(imageData);
					return File(imageBytes,"image/png");
				}
			}
			else {
				imageData = _gameStateService.GetGameState().Player.Image;//Grab the Player's generated image data.
				if(!string.IsNullOrEmpty(imageData)) {
					byte[] imageBytes = Convert.FromBase64String(imageData);
					return File(imageBytes,"image/png");
				}
			}
			// Serve default image if imageData is empty or null
			string defaultImagePath = Path.Combine("wwwroot/images", "DefaultPlayerImage.png"); // Adjust path as needed
			byte[] defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
			return File(defaultImageBytes, "image/png");
		}

		[HttpGet]
		public async Task<IActionResult> Continue() {
			bool gameStateIsLoaded = await _gameStateService.LoadGameStateAsync();
			if(gameStateIsLoaded) {
				return RedirectToAction("Game");
			}
			return RedirectToAction("Index", "Home");//Couldn't load the user's GameState into the session. Send them back to the main menu.
		}

		[HttpGet]
		public IActionResult Game() {
			return View();
		}

		[HttpGet]
		public JsonResult GetGameState() {
			GameState gameState = _gameStateService.GetGameState();
			// Return the entire game state.
			return Json(gameState);
		}

		[HttpPost]
		public async Task<JsonResult> PlayerAction(string playerAction, int actionValue = -1) {
			Func<GameState, Task> action = new Func<GameState, Task>(async (gameState) => { 
				switch(playerAction.ToLower()) {
					case "attack":
						_combatService.PlayerAttack(gameState);
						break;
					case "equip":
						if(actionValue < 0) {
							break;
						}
						Item newItem = gameState.Player.Items[actionValue];
						Item? oldItem = gameState.Player.Items.FirstOrDefault(i => i.itemType == newItem.itemType && i.Equipped == true);
						if(oldItem != null) {
							oldItem.Equipped = false;
							if(oldItem.Name == newItem.Name) {
								break;
							}
						}
						//Mark new item as equipped.
						newItem.Equipped = true;
						break;
					case "heal":
						_combatService.PlayerUseHealthPotion(gameState);
						break;
					case "rest":
						_combatService.PlayerRest(gameState); // Currently in _combatService, may change later
						break;
					case "skip-rest":
						_combatService.PlayerSkipRest(gameState); // Currently in _combatService, may change later
						break;
					case "accept":
						_combatService.PlayerAccept(gameState); // Currently in _combatService, may change later
						break;
					case "deny":
						_combatService.PlayerDeny(gameState); // Currently in _combatService, may change later
						break;
					case "open-treasure":
						_combatService.PlayerOpenTreasure(gameState); // Currently in _combatService, may change later
						break;
					case "skip-treasure":
						_combatService.PlayerSkipTreasure(gameState); // Currently in _combatService, may change later
						break;
					case "purchase":
						_combatService.PlayerPurchaseItem(gameState, actionValue); // Currently in _combatService, may change later
						break;
					case "move":
						if (actionValue < 0) {
							break;
						}
						_mapService.MovePlayer(gameState, actionValue);
						if(gameState.InCombat) {//Moving the player could put the player in combat
							_combatService.StartCombat(gameState);//Server should be the one to start combat
						}
						await _gameStateService.SaveGameStateAsync(gameState); //Save Player's progress when they enter a new room.
						break;
					case "respawn":
						_combatService.RespawnPlayer(gameState);
						await _gameStateService.SaveGameStateAsync(gameState); //Save Player's progress when they die so they can't abuse the last save.
						break;
					case "ability":
						_combatService.PlayerAbility(gameState);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(playerAction), playerAction, null);
				}
			});
			return await ProcessRequest(action);
		}

		[HttpPost]
		public async Task<JsonResult> EnemyAction() {
			Func<GameState, Task> action = new Func<GameState, Task>(async (gameState) => {
				//There is a delay between player and enemy actions client side. Try to find a way to get it onto the server to avoid bugs.
				//await Task.Delay(2000); // 2-second delay to simulate the enemy taking time complete their turn.
				_combatService.EnemyAttack(gameState);//Enemy only attacks for now.
			});
			return await ProcessRequest(action);
		}

		[HttpPost]
		public async Task<JsonResult> EndTutorial() {
			Func<GameState, Task> action = new Func<GameState, Task>(async (gameState) => {
				gameState.InTutorial = false;
			});
			return await ProcessRequest(action);
		}

		[HttpGet]
		public IActionResult GetMap(int floor) {
			Map map = _mapService.GetMap(floor);
			return Json(map);
		}

		[HttpGet]
		public IActionResult SkipToRoom(int targetRoom) {
			GameState gameState = _gameStateService.GetGameState();
			// Move the player to the desired room
			_mapService.MovePlayer(gameState, targetRoom, admin: true);
			if(gameState.InCombat) {//Moving the player could put the player in combat
				_combatService.StartCombat(gameState);//Server should be the one to start combat
			}
			_gameStateService.UpdateGameState(gameState); // Update the session with the new game state
			return RedirectToAction("Game");
		}

		public async Task<JsonResult> ProcessRequest(Func<GameState, Task> action) {
			//Keep track of what the GameState looked like before the action is performed.
			GameState gameStateBefore = _gameStateService.GetGameState();
			//Perform the requested action.
			GameState gameStateAfter = _gameStateService.GetGameState();
			await action.Invoke(gameStateAfter);
			//Update session to match what happened.
			_gameStateService.UpdateGameState(gameStateAfter);
			//Find the differences between the GameState before and after the action and turn it into json.
			JsonResult jsonResult = Json(GenerateDiff(gameStateBefore, gameStateAfter));
			return jsonResult;
		}

		public Dictionary<string, object> GenerateDiff(object objectBefore, object objectAfter) {
			Dictionary<string, object> dictionaryDiff = new Dictionary<string, object>();
			if(objectBefore == null || objectAfter == null || objectBefore.GetType() != objectAfter.GetType()) {
				return dictionaryDiff; // Return empty diff if types don't match
			}
			//Loop through each property in the GameState class
			foreach(var prop in objectBefore.GetType().GetProperties()) {
				if(prop.Name == "CurrentHealth") {
					Console.WriteLine("GameState.Enemy.CurrentHealth");
				}
				//Take a snapshot each property before and after changes were made
				var oldValue = prop.GetValue(objectBefore);
				var newValue = prop.GetValue(objectAfter);
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
					var nestedDiff = GenerateDiff(oldValue, newValue);
					if(nestedDiff.Any()) {
						dictionaryDiff[prop.Name] = nestedDiff; // Only store if changes exist
					}
				}
				else if(!Equals(oldValue, newValue)) {//Property doesn't derive from IEnumerable but did change so include it.
					dictionaryDiff[prop.Name] = newValue;
				}
			}
			return dictionaryDiff;
		}

		// Helper function: Detect primitive types correctly
		private static bool IsPrimitiveType(Type type) {
			return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
		}
	}
}