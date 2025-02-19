using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PromptQuest.Controllers
{
 
    public class GameController: Controller
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

            if (ModelState.IsValid)
            {
                return RedirectToAction("Game",player);  // Change this to redirect to the Game View
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Game(PlayerModel player)
        {
			      
			      return View(player);
        }

        [HttpGet]
        public IActionResult GetEnemy()
        {
            var enemy = new EnemyModel { Name = "Ancient Orc", ImageUrl = "/images/PlaceholderAncientOrc.png", MaxHealth = 10, CurrentHealth = 10 };
            return Json(enemy);
        }
    }
}