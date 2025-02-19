using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PromptQuest.Controllers
{

	public class GameController : Controller
	{
		private readonly ILogger<GameController> _logger;
		public GameController(ILogger<GameController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult CreateCharacter()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateCharacter(PlayerModel player)
		{
			player.MaxHealth = 10;
			player.CurrentHealth = 10;
			player.HealthPotions = 2;
			player.Attack = 1;

			if (ModelState.IsValid)
			{
				CurrentPlayer.SetPlayer(player);
				return RedirectToAction("Game");  // Change this to redirect to the Game View
			}
			else
			{
				return View();
			}
		}

		[HttpGet]
		public IActionResult Game()
		{

			return View(CurrentPlayer.GetPlayer());
		}

		[HttpGet]
		public IActionResult GetEnemy()
		{
			var enemy = new EnemyModel { Name = "Ancient Orc", ImageUrl = "/images/PlaceholderAncientOrc.png", MaxHealth = 10, CurrentHealth = 10, Attack = 3 };
			return Json(enemy);
		}
	}
}