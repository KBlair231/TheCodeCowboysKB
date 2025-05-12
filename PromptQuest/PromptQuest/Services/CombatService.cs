using System.ComponentModel;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface ICombatService {
		void StartCombat(GameState gameState);
		void PlayerAttack(GameState gameState, int attackMult = 1, bool decrementAbility=true);
		void PlayerUseHealthPotion(GameState gameState);
		void PlayerRest(GameState gameState);
		void PlayerSkipRest(GameState gameState);
		void PlayerAccept(GameState gameState);
		void PlayerDeny(GameState gameState);
		void PlayerAbility(GameState gameState);
		void PlayerOpenTreasure(GameState gameState);
		void PlayerSkipTreasure(GameState gameState);
		void RespawnPlayer(GameState gameState);
		void EnemyAttack(GameState gameState);
		Enemy GetEnemy(GameState gameState);
	}

	public class CombatService : ICombatService {
		/// <summary>Initiates combat between the player and an enemy and updates the game state. </summary>
		public void StartCombat(GameState gameState) {
			gameState.InCombat = true;
			gameState.IsPlayersTurn = true; // Player always goes first, for now.
			gameState.Player.AbilityCooldown = 0; //reset player ability, may be removed down the line
			gameState.Player.DefenseBuff = 0; //reset player defense buff
			if(gameState.PlayerLocation == 11) {
				gameState.Enemy = GetElite(gameState);
				gameState.AddMessage($"You have been attacked by the {gameState.Enemy.Name}!"); // Let the user know that combat started.
				return;
			}
			if(gameState.PlayerLocation != 18) {
				gameState.Enemy = GetEnemy(gameState);
				gameState.AddMessage($"You have been attacked by the {gameState.Enemy.Name}!"); // Let the user know that combat started.
				return;
			}
			// If the player is in the boss room, spawn a boss.
			gameState.Enemy = GetBoss(gameState);
			gameState.AddMessage($"You have encountered the {gameState.Enemy.Name}! Defeat the boss! "); // Let the user know that combat started.
		}

		#region Player Action Methods

		/// <summary> Calculates the damage that the player does to the enemy, updates the game state, then returns a message.</summary>
		public void PlayerAttack(GameState gameState, int attackMult = 1, bool decrementAbility = true) {
			// Get the player's equipped item
			Item item = gameState.Player.ItemEquipped;
			// Calculate damage as attack - defense.
			int damage = (int)Math.Floor((double)(gameState.Player.Attack + item.Attack)*attackMult) - gameState.Enemy.Defense;
			// If attack is less than one make it one.
			if (damage < 1) {
				damage = 1;
			}
			// Update enemy health.
			gameState.Enemy.CurrentHealth -= damage;
			//decrement ability cooldown if ability was not used
			if (decrementAbility && gameState.Player.AbilityCooldown>0)
			{
				gameState.Player.AbilityCooldown -= 1;
			}
			// Return the result to the user.
			gameState.AddMessage($"You attacked the {gameState.Enemy.Name} for {damage} damage");
			if (item.StatusEffects != StatusEffect.None) {
				Random random = new Random();
				int statusEffectChance = random.Next(0, 5); // 25% chance to apply status effect
				if (statusEffectChance == 1) {
					if (!gameState.Enemy.StatusEffects.HasFlag(item.StatusEffects)) {
						gameState.AddMessage($"The {gameState.Enemy.Name} is now affected by {item.StatusEffects.ToString()}!");
						gameState.Enemy.StatusEffects = item.StatusEffects;
					}
				}
			}
			// Check if enemy died.
			
			if (gameState.Enemy.CurrentHealth <= 0) {
				gameState.IsPlayersTurn = true; // Zero this field out because combat is over.
				gameState.IsLocationComplete = true; // Player has completed the current area.
				gameState.AddMessage($"You have defeated the {gameState.Enemy.Name}! Check your map to see where you're going next.");
				// Generate a random amount of gold between 6 and 15
				Random random = new Random();
				int gold = random.Next(6, 16);
				gameState.Player.Gold += gold;
				gameState.AddMessage($"You gained {gold} gold!");
				if (gameState.PlayerLocation == 18) {
					Item bossItem = GetBossItem(gameState); // Get the boss item.
					gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {bossItem.Name}!");
					gameState.Player.Items.Add(bossItem);
					return;
				} 
				if(gameState.PlayerLocation == 11) {
					Item eliteItem = GetEliteItem(gameState); // Get the elite's item.
					gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {eliteItem.Name}!");
					gameState.Player.Items.Add(eliteItem);
					return;
				}
			}
			
			gameState.IsPlayersTurn = false;
		}
		/// <summary>activates the player's ability</summary>
		public void PlayerAbility(GameState gameState)
		{
			switch (gameState.Player.Class.ToLower())
			{
				case "warrior"://attack for double power, uses the attack function
					gameState.AddMessage($"You used your ability! You power up and perform a strong attack!");
					PlayerAttack(gameState, 2, false);
					gameState.Player.AbilityCooldown = 3;
					break;
				case "mage"://gain +6 defense for the next attack
					gameState.AddMessage($"You used your ability! You cast a shield spell to gain +6 defense against the next attack!");
					gameState.Player.DefenseBuff += 6;
					gameState.Player.AbilityCooldown = 4;
					break;
				case "archer"://perform two attacks
					gameState.AddMessage($"You used your ability! You shoot two arrows at the enemy!");
					PlayerAttack(gameState, 1, false);
					PlayerAttack(gameState, 1, false);
					gameState.Player.AbilityCooldown = 3;
					break;
				default:
					gameState.AddMessage("Your class has no ability.");
					break;
			}
		}
		/// <summary>Calculates the amount healed by a Health Potion, updates the game state, then returns a message.</summary>
		public void PlayerUseHealthPotion(GameState gameState) {
			// If player has no potions, don't let them heal.
			if(gameState.Player.HealthPotions <= 0) {
				gameState.AddMessage("You have no Health Potions!");
				return;
			}
			// If player is already at max health, don't let them heal.
			if(gameState.Player.CurrentHealth == gameState.Player.MaxHealth) {
				gameState.AddMessage("You are already at max health!");
				return;
			}
			// Update player health and number of potions.
			gameState.Player.HealthPotions -= 1;
			gameState.Player.CurrentHealth += 5;
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				gameState.AddMessage($"You healed to max HP!"); // Overwrite current message.
				return;
			}
			gameState.AddMessage($"You healed to {gameState.Player.CurrentHealth} HP!");
			// Healing does not end the player's turn.
		}

		/// <summary>Calculates the amount healed by Resting, updates the game state, then returns a message.</summary>
		public void PlayerRest(GameState gameState) {
			// Set potions to 2 if they are less than 2.
			if(gameState.Player.HealthPotions < 2) {
				gameState.Player.HealthPotions = 2;
			}
			// If player is already at max health, don't let them rest.
			if(gameState.Player.CurrentHealth == gameState.Player.MaxHealth) {
				gameState.AddMessage("You are already at max health!");
			}
			// Update player health (+30% of max health)
			gameState.Player.CurrentHealth += gameState.Player.MaxHealth / 3;
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				gameState.AddMessage($"You healed to max HP!"); // Overwrite current message.
				gameState.IsLocationComplete = true;
				return;
			}
			// Ensure player can leave
			gameState.IsLocationComplete = true;
			gameState.AddMessage($"You healed to {gameState.Player.CurrentHealth} HP!");
		}

		/// <summary>Doesn't rest, updates the game state, then returns a message.</summary>
		public void PlayerSkipRest(GameState gameState) {
			gameState.AddMessage("You have guts...");
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player accepts event, updates the game state, then returns a message.</summary>
		public void PlayerAccept(GameState gameState) {
			// Player gains 3 potions
			gameState.Player.HealthPotions += 3;
			// Damage the player
			gameState.Player.CurrentHealth -= 7;
			if(gameState.Player.CurrentHealth <= 0) {
				gameState.Player.CurrentHealth = 1;
			}
			// Tell player what happened
			gameState.AddMessage("Your greed earned you 3 more potions, but at what cost?");
			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player denies event, updates the game state, then returns a message.</summary>
		public void PlayerDeny(GameState gameState) {
			gameState.AddMessage("You resist the temptation.");
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player opens treasure chest, updates the game state, then returns a message.</summary>
		public void PlayerOpenTreasure(GameState gameState) {
			int randPotions = new Random().Next(1, 4); // Randomly give 1 to 3 potions
			gameState.Player.HealthPotions += randPotions;
			int randItem = new Random().Next(1, 5); // Randomly give an item from the list below
			Item treasureItem = new Item();
			if(randItem == 1) {
				// If 1, give them a treasure specific item.
				treasureItem.Name = "Banana Crown";
				treasureItem.Attack = 2;
				treasureItem.Defense = 10;
				treasureItem.ImageSrc = "/images/BananaCrown.png";
				treasureItem.itemType = ItemType.Helm;
				gameState.Player.Items.Add(treasureItem);
			}
			else if(randItem == 2) {
				// If 2, give them a treasure specific item.
				treasureItem.Name = "Gothic T-Shirt";
				treasureItem.Attack = 7;
				treasureItem.Defense = 2;
				treasureItem.ImageSrc = "/images/GothicTShirt.png";
				treasureItem.itemType = ItemType.Chest;
				gameState.Player.Items.Add(treasureItem);
			}
			else if(randItem == 3) {
				// If 3, give them a treasure specific item.
				treasureItem.Name = "Gilded Boots";
				treasureItem.Attack = 5;
				treasureItem.Defense = 5;
				treasureItem.ImageSrc = "/images/GildedBoots.png";
				treasureItem.itemType = ItemType.Boots;
				gameState.Player.Items.Add(treasureItem);
			}
			else {
				// Else, give them a treasure specific item.
				treasureItem.Name = "Elf Hat";
				treasureItem.Attack = 0;
				treasureItem.Defense = 12;
				treasureItem.ImageSrc = "/images/ElfHat.png";
				treasureItem.itemType = ItemType.Helm;
				gameState.Player.Items.Add(treasureItem);
			}

			// Tell player what happened
			gameState.AddMessage("You gained: " + randPotions + " Health Potion(s) and " + treasureItem.Name + "!");
			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player skips treasure chest, updates the game state, then returns a message.</summary>
		public void PlayerSkipTreasure(GameState gameState) {
			gameState.AddMessage("Better safe than sorry...");
			gameState.IsLocationComplete = true;
		}

		public void RespawnPlayer(GameState gameState) {
			// Reset player health and potions back to max
			gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
			// Reset player's potions to 2
			gameState.Player.HealthPotions = 2;
			// Clear messages from previous life.
			gameState.ClearMessages();
			// Restart the player at the first location.
			gameState.PlayerLocation = 1;
			// Start a new fight.
			StartCombat(gameState);
		}

		#endregion  Player Action Methods - End

		#region Enemy Action Methods

		/// <summary>Calculates the damage that the enemy does to the player, updates the game state, then returns a message.</summary>
		public void EnemyAttack(GameState gameState) {
			// Get the player's equipped item
			Item item = gameState.Player.ItemEquipped;
			// Calculate damage as attack - defense.
			int damage = gameState.Enemy.Attack - gameState.Player.Defense - item.Defense - gameState.Player.DefenseBuff;
			if(gameState.Player.DefenseBuff>0){
				gameState.AddMessage("Your shield blocked incoming damage");
			}
			gameState.Player.DefenseBuff = 0;//reset the defense buff after blocking one hit
			// If attack is less than one make it one.
			if(damage < 1)
				damage = 1;
			// Update player health.
			gameState.Player.CurrentHealth -= damage;
			// Return an action result with a message describing what happened.
			gameState.AddMessage($"The {gameState.Enemy.Name} attacked you for {damage} damage");
			// Check if player died.
			if(gameState.Player.CurrentHealth < 1) {
				gameState.IsPlayersTurn = true; // Zero this field out because combat is over.
				gameState.AddMessage("You have been defeated.");
				gameState.ClearMapNodeIdsVisited(); // Clear visited map nodes since the player died.
			}
			// Apply status effect damage to the enemy
			if (gameState.Enemy.StatusEffects.HasFlag(StatusEffect.Burning)) { 
				gameState.Enemy.CurrentHealth -= 2;
				gameState.Enemy.StatusEffects &= ~StatusEffect.Burning; // Remove the burning effect
				gameState.AddMessage($"The {gameState.Enemy.Name} took 2 damage from burning and the flame extinguished.");
			}
			if (gameState.Enemy.StatusEffects.HasFlag(StatusEffect.Bleeding)) {
				gameState.Enemy.CurrentHealth -= 1;
				gameState.AddMessage($"The {gameState.Enemy.Name} took 1 damage from their untreated wound.");
			}
			// Player didn't die, so now it is their turn.
			gameState.IsPlayersTurn = true;
			// Check if enemy died.
			if(gameState.Enemy.CurrentHealth <= 0) {
				gameState.IsLocationComplete = true; // Player has completed the current area.
				gameState.AddMessage($"You have defeated the {gameState.Enemy.Name}! Check your map to see where you're going next.");
				if (gameState.PlayerLocation == 10) {
					Item bossItem = GetBossItem(gameState); // Get the boss item.
					gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {bossItem.Name}!");
					gameState.Player.Items.Add(bossItem);
					return;
				}
				if (gameState.PlayerLocation == 7) {
					Item eliteItem = GetEliteItem(gameState); // Get the elite's item.
					gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {eliteItem.Name}!");
					gameState.Player.Items.Add(eliteItem);
					return;
				}
			}
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
						enemy.Attack = 7;
						enemy.Defense = 3;
						break;
				}
			}
			return enemy;
		}
		#region Boss and Elite Item Generation
		public Item GetBossItem(GameState gameState) {
			// Generate a boss item for the player
			Item bossItem = new Item();
			if (gameState.Floor == 1) {
				// If the player is on the first floor, give them a boss specific item.
				bossItem.Name = "Dark Staff";
				bossItem.Attack = 5;
				bossItem.Defense = 4;
				bossItem.ImageSrc = "/images/DarkStaff.png";
			}
			else if (gameState.Floor == 2) {
				// If the player is on the second floor, give them a boss specific item.
				bossItem.Name = "Dark Elvish Sword";
				bossItem.Attack = 8;
				bossItem.Defense = 3;
				bossItem.ImageSrc = "/images/DarkElvenSword.png";
				bossItem.StatusEffects = StatusEffect.Bleeding;
			}
			else {
				// If the player is on the third floor, give them a boss specific item.
				bossItem.Name = "Shadow Spear";
				bossItem.Attack = 12;
				bossItem.Defense = 6;
				bossItem.ImageSrc = "/images/DarkSpear.png";
				bossItem.StatusEffects = StatusEffect.Bleeding;
			}
			return bossItem;
		}
		public Item GetEliteItem(GameState gameState) {
			// Generate a boss item for the player
			Item eliteItem = new Item();
			if (gameState.Floor == 1) {
				// If the player is on the first floor, give them an elite specific item.
				eliteItem.Name = "Berserker Axe";
				eliteItem.Attack = 5;
				eliteItem.Defense = 1;
				eliteItem.ImageSrc = "/images/BerserkerAxe.png";
				eliteItem.StatusEffects = StatusEffect.Bleeding;
			}
			else if (gameState.Floor == 2) {
				// If the player is on the second floor, give them an elite specific item.
				eliteItem.Name = "Spiked Leaf";
				eliteItem.Attack = 6;
				eliteItem.Defense = 2;
				eliteItem.ImageSrc = "/images/SpikedLeaf.png";
			}
			else {
				// If the player is on the third floor, give them an elite specific item.
				eliteItem.Name = "Demon Cleaver";
				eliteItem.Attack = 10;
				eliteItem.Defense = 5;
				eliteItem.ImageSrc = "/images/DemonCleaver.png";
				eliteItem.StatusEffects = StatusEffect.Bleeding;
			}
			return eliteItem;
		}
		#endregion Boss and Elite Item Generation - End
		#region Elite and Boss Enemy Generation
		/// <summary> Generates an Elite Enemy, updates the game state, then returns the Elite Enemy.</summary>
		public Enemy GetElite(GameState gameState) {
			if (gameState.Floor == 1) {
				Enemy elite = new Enemy();
				elite.Name = "Spectral Orc Berserker";
				elite.ImageUrl = "/images/SpectralOrc.png";
				elite.MaxHealth = 15;
				elite.CurrentHealth = 15;
				elite.Attack = 4;
				elite.Defense = 3;
				return elite;
			}
			else if (gameState.Floor == 2) {
				Enemy elite = new Enemy();
				elite.Name = "Corrupted Plant";
				elite.ImageUrl = "/images/EvilPlant.png";
				elite.MaxHealth = 20;
				elite.CurrentHealth = 20;
				elite.Attack = 6;
				elite.Defense = 2;
				return elite;
			}
			else {
				Enemy elite = new Enemy();
				elite.Name = "Demon Knight";
				elite.ImageUrl = "/images/DemonKnight.png";
				elite.MaxHealth = 40;
				elite.CurrentHealth = 40;
				elite.Attack = 7;
				elite.Defense = 5;
				return elite;
			}
		}
		/// <summary> Generates an Enemy, updates the game state, then returns the Enemy.</summary>
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
		#endregion Boss and Elite Enemy Generation - End
		#endregion Helper Methods - End
	}
}