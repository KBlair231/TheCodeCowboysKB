﻿using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface ICombatService {
		string StartCombat(GameState gameState);
		string PlayerAttack(GameState gameState);
		string PlayerUseHealthPotion(GameState gameState);
		string EnemyAttack(GameState gameState);
		void RespawnPlayer(GameState gameState);
		Enemy GetEnemy();
	}

	public class CombatService : ICombatService {

		/// <summary>Initiates combat between the player and an enemy and updates the game state. </summary>
		public string StartCombat(GameState gameState) {
			gameState.InCombat = true;
			gameState.IsPlayersTurn = true; // Player always goes first, for now.
			gameState.Enemy = GetEnemy();
			gameState.Player.HealthPotions = 2; // Set player's health potions to 2 when combat starts (Temporary)
			string message = $"The {gameState.Enemy.Name} attacked!"; // Let the user know that combat started.
		  return message;
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
			// Calculate damage as attack - defense.
			int damage = gameState.Player.Attack - gameState.Enemy.Defense;
			// If attack is less than one make it one.
			if (damage < 1)
				damage = 1;
			// Update enemy health.
			gameState.Enemy.CurrentHealth -= damage;
			// Return the result to the user.
			string message = $"You attacked the {gameState.Enemy.Name} for {damage} damage";
			// Check if enemy died.
			if (gameState.Enemy.CurrentHealth <= 0) {
				gameState.InCombat = false; // Enemy is dead, combat has ended.
				gameState.IsPlayersTurn = false; // Zero this field out because combat is over.
				gameState.IsLocationComplete = true; // Player has completed the current area.
				message += $", you have defeated the {gameState.Enemy.Name}."; // Let them know in the same message.
			}
			// Enemy didn't die, so now it is their turn.
			gameState.IsPlayersTurn = false;
			return message;
		}

		/// <summary>Calculates the amount healed by a Health Potion, updates the game state, then returns a message.</summary>
		public string PlayerUseHealthPotion(GameState gameState) {
			string message;
			// If player has no potions, don't let them heal.
			if (gameState.Player.HealthPotions <= 0) {
				message = "You have no Health Potions!";
				return message;
			}
			// If player is already at max health, don't let them heal.
			if (gameState.Player.CurrentHealth == gameState.Player.MaxHealth) {
				message = "You are already at max health!";
				return message;
			}
			// Update player health and number of potions.
			gameState.Player.HealthPotions -= 1;
			gameState.Player.CurrentHealth += 5;
			message = $"You healed to {gameState.Player.CurrentHealth} HP!";
			// If the potion put the player's health above maximum, set it to maximum.
			if (gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				message = $"You healed to max HP!"; // Overwrite current message.
			}
			// Healing cannot possibly end combat, so no reason to check if combat has ended.
			// Healing does not end the player's turn.
			return message;
		}

		#endregion  Player Action Methods - End

		#region Enemy Action Methods

		/// <summary>Calculates the damage that the enemy does to the player, updates the game state, then returns a message.</summary>
		public string EnemyAttack(GameState gameState) {
			// Calculate damage as attack - defense.
			int damage = gameState.Enemy.Attack - gameState.Player.Defense;
			// If attack is less than one make it one.
			if (damage < 1)
				damage = 1;
			// Update player health.
			gameState.Player.CurrentHealth -= damage;
			// Return an action result with a message describing what happened.
			string message = $"The {gameState.Enemy.Name} attacked you for {damage} damage";
			// Check if player died.
			if (gameState.Player.CurrentHealth < 1) {
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
		public Enemy GetEnemy() {
			Enemy enemy = new Enemy();
			Random random = new Random();
			int enemyType = random.Next(1, 4); // Generates a number between 1 and 3
			switch (enemyType) {
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
			return enemy;
		}

		#endregion Helper Methods - End
	}
}
