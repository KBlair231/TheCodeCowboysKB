using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;
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
	}
}