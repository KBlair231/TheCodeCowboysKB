using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface ICombatService {
		string StartCombat(GameState gameState);
		string PlayerAttack(GameState gameState);
		string PlayerUseHealthPotion(GameState gameState);
		string PlayerRest(GameState gameState);
		string PlayerSkipRest(GameState gameState);
		string PlayerAccept(GameState gameState);
		string PlayerDeny(GameState gameState);
		string EnemyAttack(GameState gameState);
		void RespawnPlayer(GameState gameState);
		Enemy GetEnemy(GameState gameState);
	}

	public class CombatService : ICombatService {

		/// <summary>Initiates combat between the player and an enemy and updates the game state. </summary>
		public string StartCombat(GameState gameState) {
			gameState.InCombat = true;
			gameState.IsPlayersTurn = true; // Player always goes first, for now.
			if(gameState.PlayerLocation != 10) {
				gameState.Enemy = GetEnemy(gameState);
				string message = $"The {gameState.Enemy.Name} attacked!"; // Let the user know that combat started.
				return message;
			}
			else {
				// If the player is in the boss room, spawn a boss.
				gameState.Enemy = GetBoss(gameState);
				string message = $"You have encounterd the {gameState.Enemy.Name}! Defeat the boss! "; // Let the user know that combat started.
				return message;
			}
		}

		/// <summary>Respawns the player by resetting their health and potions, and updates the game state.</summary>
		public void RespawnPlayer(GameState gameState) {
			gameState.Player.CurrentHealth = gameState.Player.MaxHealth; // Reset health to max
			gameState.InCombat = false; // Player is no longer in combat
			gameState.IsPlayersTurn = false; // It is not the player's turn
		}
		#region Player Action Methods

		/// <summary> Calculates the damage that the player does to the enemy, updates the game state, then returns a message.</summary>
		public string PlayerAttack(GameState gameState) {
			// Get the player's equipped item
			Item item = gameState.Player.ItemEquipped;
			// Calculate damage as attack - defense.
			int damage = gameState.Player.Attack + item.Attack - gameState.Enemy.Defense;
			// If attack is less than one make it one.
			if(damage < 1)
				damage = 1;
			// Update enemy health.
			gameState.Enemy.CurrentHealth -= damage;
			// Return the result to the user.
			string message = $"You attacked the {gameState.Enemy.Name} for {damage} damage";
			// Check if enemy died.
			if(gameState.Enemy.CurrentHealth <= 0) {
				gameState.InCombat = false; // Enemy is dead, combat has ended.
				gameState.IsPlayersTurn = false; // Zero this field out because combat is over.
				gameState.IsLocationComplete = true; // Player has completed the current area.
				message += $", you have defeated the {gameState.Enemy.Name}."; // Let them know in the same message.
				if(gameState.PlayerLocation == 10) {
					// Generate a boss item for the player
					if(gameState.Floor == 1) {
						// If the player is on the first floor, give them a boss specific item.
						Item bossItem = new Item();
						bossItem.Name = "Orc Warlock's Staff";
						bossItem.Attack = 5;
						bossItem.Defense = 4;
						bossItem.ImageSrc = "/images/DarkStaff.png";
						gameState.Player.Items.Add(bossItem);
					}
					else if(gameState.Floor == 2) {
						// If the player is on the second floor, give them a boss specific item.
						Item bossItem = new Item();
						bossItem.Name = "Dark Elvish Sword";
						bossItem.Attack = 8;
						bossItem.Defense = 3;
						bossItem.ImageSrc = "/images/DarkElvenSword.png";
						gameState.Player.Items.Add(bossItem);
					}
					else {
						// If the player is on the third floor, give them a boss specific item.
						Item bossItem = new Item();
						bossItem.Name = "Shadow Spear";
						bossItem.Attack = 12;
						bossItem.Defense = 6;
						bossItem.ImageSrc = "/images/DarkSpear.png";
						gameState.Player.Items.Add(bossItem);
					}
				}
				return message;
			}
			// Enemy didn't die, so now it is their turn.
			gameState.IsPlayersTurn = false;
			return message;
		}

		/// <summary>Calculates the amount healed by a Health Potion, updates the game state, then returns a message.</summary>
		public string PlayerUseHealthPotion(GameState gameState) {
			string message;
			// If player has no potions, don't let them heal.
			if(gameState.Player.HealthPotions <= 0) {
				message = "You have no Health Potions!";
				return message;
			}
			// If player is already at max health, don't let them heal.
			if(gameState.Player.CurrentHealth == gameState.Player.MaxHealth) {
				message = "You are already at max health!";
				return message;
			}
			// Update player health and number of potions.
			gameState.Player.HealthPotions -= 1;
			gameState.Player.CurrentHealth += 5;
			message = $"You healed to {gameState.Player.CurrentHealth} HP!";
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				message = $"You healed to max HP!"; // Overwrite current message.
			}
			// Healing cannot possibly end combat, so no reason to check if combat has ended.
			// Healing does not end the player's turn.
			return message;
		}

		/// <summary>Calculates the amount healed by Resting, updates the game state, then returns a message.</summary>
		public string PlayerRest(GameState gameState) {
			string message;
			// Set potions to 2 if they are less than 2.
			if (gameState.Player.HealthPotions < 2)
			{
				gameState.Player.HealthPotions = 2;
			}
			// If player is already at max health, don't let them rest.
			if(gameState.Player.CurrentHealth == gameState.Player.MaxHealth) {
				message = "You are already at max health!";
				return message;
			}
			// Update player health (+30% of max health)
			gameState.Player.CurrentHealth += gameState.Player.MaxHealth / 3;
			message = $"You healed to {gameState.Player.CurrentHealth} HP!";
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				message = $"You healed to max HP!"; // Overwrite current message.
			}
			// Ensure player can leave
			gameState.IsLocationComplete = true;
			return message;
		}

		/// <summary>Doesn't rest, updates the game state, then returns a message.</summary>
		public string PlayerSkipRest(GameState gameState) {
			string message;
			message = "You have guts...";
			gameState.IsLocationComplete = true;
			return message;
		}

		/// <summary>Player accepts event, updates the game state, then returns a message.</summary>
		public string PlayerAccept(GameState gameState) {
			string message;
			// Player gains 3 potions
			gameState.Player.HealthPotions += 3;
			// Damage the player
			gameState.Player.CurrentHealth -= 7;
			if (gameState.Player.CurrentHealth <= 0) {
				gameState.Player.CurrentHealth = 1;
			}
			// Tell player what happened
			message = "Your greed earned you 3 more potions, but at what cost?";
			// Ensure player can leave
			gameState.IsLocationComplete = true;
			return message;
		}

		/// <summary>Player denies event, updates the game state, then returns a message.</summary>
		public string PlayerDeny(GameState gameState) {
			string message;
			message = "You resist the temptation.";
			gameState.IsLocationComplete = true;
			return message;
		}

		#endregion  Player Action Methods - End

		#region Enemy Action Methods

		/// <summary>Calculates the damage that the enemy does to the player, updates the game state, then returns a message.</summary>
		public string EnemyAttack(GameState gameState) {
			// Get the player's equipped item
			Item item = gameState.Player.ItemEquipped;
			// Calculate damage as attack - defense.
			int damage = gameState.Enemy.Attack - gameState.Player.Defense - item.Defense;
			// If attack is less than one make it one.
			if(damage < 1)
				damage = 1;
			// Update player health.
			gameState.Player.CurrentHealth -= damage;
			// Return an action result with a message describing what happened.
			string message = $"The {gameState.Enemy.Name} attacked you for {damage} damage";
			// Check if player died.
			if(gameState.Player.CurrentHealth < 1) {
				gameState.InCombat = false; // Player is dead, combat has ended.
				gameState.IsPlayersTurn = false; // Zero this field out because combat is over.
				message += ", you have been defeated."; // Let them know in the same message.
			}
			// Player didn't die, so now it is their turn.
			gameState.IsPlayersTurn = true;
			return message;
		}

		#endregion Enemy Action Methods - end

		#region Helper Methods

		/// <summary>Generatees an Enemy, updates the game state, then returns the Enemy.</summary>
		public Enemy GetEnemy(GameState gameState) {
			Enemy enemy = new Enemy();
			Random random = new Random();
			int enemyType = random.Next(1, 4); // Generates a number between 1 and 3
			if(gameState.Floor == 1) {
				switch(enemyType) {
					case 1:
						enemy.Name = "Ancient Orc";
						enemy.ImageUrl = "/images/PlaceholderAncientOrc.png";
						enemy.MaxHealth = 10;
						enemy.CurrentHealth = 10;
						enemy.Attack = 2;
						enemy.Defense = 1;
						break;
					case 2:
						enemy.Name = "Decrepit Centaur";
						enemy.ImageUrl = "/images/PlaceholderDecrepitCentaur.png";
						enemy.MaxHealth = 10;
						enemy.CurrentHealth = 10;
						enemy.Attack = 3;
						enemy.Defense = 0;
						break;
					case 3:
						enemy.Name = "Rotting Zombie";
						enemy.ImageUrl = "/images/PlaceholderRottingZombie.png";
						enemy.MaxHealth = 8;
						enemy.CurrentHealth = 8;
						enemy.Attack = 2;
						enemy.Defense = 2;
						break;
				}
			}
			else if(gameState.Floor == 2) {
				switch(enemyType) {
					case 1:
						enemy.Name = "Evil Elven Mage";
						enemy.ImageUrl = "/images/EvilElvenMage.png";
						enemy.MaxHealth = 15;
						enemy.CurrentHealth = 15;
						enemy.Attack = 6;
						enemy.Defense = 1;
						break;
					case 2:
						enemy.Name = "Treant Guard";
						enemy.ImageUrl = "/images/TreantGuard.png";
						enemy.MaxHealth = 20;
						enemy.CurrentHealth = 20;
						enemy.Attack = 3;
						enemy.Defense = 5;
						break;
					case 3:
						enemy.Name = "Forest Wisp";
						enemy.ImageUrl = "/images/MysticalWisp.png";
						enemy.MaxHealth = 25;
						enemy.CurrentHealth = 25;
						enemy.Attack = 2;
						enemy.Defense = 0;
						break;
				}
			}
			else {
				// Floor 3
				switch(enemyType) {
					case 1:
						enemy.Name = "Goblin Archer";
						enemy.ImageUrl = "/images/GobArcher.png";
						enemy.MaxHealth = 30;
						enemy.CurrentHealth = 30;
						enemy.Attack = 8;
						enemy.Defense = 3;
						break;
					case 2:
						enemy.Name = "Goblin Assassin";
						enemy.ImageUrl = "/images/GobAssassin.png";
						enemy.MaxHealth = 25;
						enemy.CurrentHealth = 25;
						enemy.Attack = 7;
						enemy.Defense = 2;
						break;
					case 3:
						enemy.Name = "Drunk Orc";
						enemy.ImageUrl = "/images/LazyDrunkOrc.png";
						enemy.MaxHealth = 35;
						enemy.CurrentHealth = 35;
						enemy.Attack = 10;
						enemy.Defense = 5;
						break;
				}
			}
			return enemy;
		}

		/// <summary>Generatees an Enemy, updates the game state, then returns the Enemy.</summary>
		public Enemy GetBoss(GameState gameState) {
			if(gameState.Floor == 1) {
				Enemy boss = new Enemy();
				boss.Name = "Dark Orc Warlock";
				boss.ImageUrl = "/images/OrcWarlock.png";
				boss.MaxHealth = 20;
				boss.CurrentHealth = 20;
				boss.Attack = 5;
				boss.Defense = 4;
				return boss;
			}
			else if(gameState.Floor == 2) {
				Enemy boss = new Enemy();
				boss.Name = "Dark Elven King";
				boss.ImageUrl = "/images/DarkElfWarrior.png";
				boss.MaxHealth = 25;
				boss.CurrentHealth = 25;
				boss.Attack = 6;
				boss.Defense = 6;
				return boss;
			}
			else {
				Enemy boss = new Enemy();
				boss.Name = "Eldritch Horror";
				boss.ImageUrl = "/images/EldritchHorror.png";
				boss.MaxHealth = 50;
				boss.CurrentHealth = 50;
				boss.Attack = 8;
				boss.Defense = 8;
				return boss;
			}
		}
		#endregion Helper Methods - End
	}
}
