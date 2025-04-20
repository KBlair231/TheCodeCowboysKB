using PromptQuest.Models;
using PromptQuest.Services;
using NUnit.Framework;

namespace PromptQuest.Tests_BDD.StepDefinitions {
	[Binding]
	public class DefenseStatSteps {
		private GameState _gameState;
		private ICombatService _combatService;
		private string _resultMessage;

		public DefenseStatSteps() {
			_combatService = new CombatService();
			_gameState = new GameState();
			_gameState.Player = new Player {
				CurrentHealth = 10,
				MaxHealth = 10,
				Attack = 5,
				Defense = 2,
				Name = "TestPlayer",
				Class = "Warrior",
			};
		}
		#region User Attacked By Enemy
		[Given("the user is on the game page")]
		public void GivenTheUserIsOnTheGamePage() {
			DefenseStatSteps steps = new DefenseStatSteps();
			_combatService.StartCombat(_gameState);
		}

		[When("the user is attacked by an enemy")]
		public void WhenTheUserIsAttackedByAnEnemy() {
			_combatService.EnemyAttack(_gameState);
		}

		[Then("the user should receive damage equal to the enemy's attack minus player defense")]
		public void ThenTheUserShouldReceiveDamageEqualToTheEnemysAttackMinusPlayerDefense() {
			int expectedDamage = _gameState.Enemy.Attack - (_gameState.Player.Defense + _gameState.Player.ItemEquipped.Defense);
			if (expectedDamage < 1) expectedDamage = 1;

			int actualDamage = _gameState.Player.MaxHealth - _gameState.Player.CurrentHealth;
			Assert.AreEqual(expectedDamage, actualDamage, _resultMessage);
		}
		#endregion
		#region User Attacks Enemy
		[Given("the user is in combat with an enemy")]
		public void GivenTheUserIsInCombatWithAnEnemy() {
			DefenseStatSteps steps = new DefenseStatSteps();
			_combatService.StartCombat(_gameState);
		}
		[When("the enemy is attacked by the user")]
		public void WhenTheEnemyIsAttackedByTheUser() {
			_combatService.PlayerAttack(_gameState);
		}
		[Then("the enemy should receive damage equal to the user's attack minus enemy defense")]
		public void ThenTheEnemyShouldReceiveDamageEqualToTheUsersAttackMinusEnemyDefense() {
			int expectedDamage = _gameState.Player.Attack + _gameState.Player.ItemEquipped.Attack - _gameState.Enemy.Defense;
			if (expectedDamage < 1) expectedDamage = 1;
			int actualDamage = _gameState.Enemy.MaxHealth - _gameState.Enemy.CurrentHealth;
			Assert.AreEqual(expectedDamage, actualDamage, _resultMessage);
		}
		#endregion
	}
}