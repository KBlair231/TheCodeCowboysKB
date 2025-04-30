using PromptQuest.Models;
using PromptQuest.Services;
using NUnit.Framework;

namespace PromptQuest.Tests_BDD.StepDefinitions
{
	[Binding]
	public class PlayerAbilitySteps
	{
		private GameState _gameState;
		private ICombatService _combatService;
		private string _resultMessage;

		public PlayerAbilitySteps()
		{
			_combatService = new CombatService();
			_gameState = new GameState();
			_gameState.Player = new Player
			{
				CurrentHealth = 10,
				MaxHealth = 10,
				Attack = 3,
				Defense = 2,
				Name = "TestPlayer",
			};
		}
		[Given("the user is in combat")]
		public void GivenTheUserIsInCombat()
		{
			_combatService.StartCombat(_gameState);
		}
		[Given("the user has the {string} class")]
		public void GivenTheUserHasTheClass(string className)
		{
			_gameState.Player.Class = className;
		}
		[Given("the ability has a cooldown of {int}")]
		public void GivenTheAbilityHasACooldownOf(int cooldown)
		{
			_gameState.Player.AbilityCooldown = cooldown;
		}
		[Given("the user has a defense buff of {int}")]
		public void GivenTheUserHasADefenseBuffOf(int defenseBuff)
		{
			_gameState.Player.DefenseBuff = defenseBuff;
		}

		[When("the user performs an {string}")]
		public void WhenTheUserPerforms(string action)
		{
			if (action == "attack")
			{
				_combatService.PlayerAttack(_gameState);
			}
			else if (action == "ability")
			{
				_combatService.PlayerAbility(_gameState);
			}
			else
			{
				throw new ArgumentException($"Unknown action: {action}");
			}
		}
		[When("the user enters combat")]
		public void WhenTheUserEntersCombat()
		{
			_combatService.StartCombat(_gameState);
		}
		[When("the user is attacked")]
		public void WhenTheUserIsAttacked()
		{
			_combatService.EnemyAttack(_gameState);
		}

		[Then("the enemy should receive damage equal to the user's attack times {int} minus enemy defense")]
		public void ThenTheEnemyShouldReceiveDamageEqualToTheUsersAttackTimesMinusEnemyDefense(int mult)
		{
			int expectedDamage = (int)Math.Floor((double)(_gameState.Player.Attack + _gameState.Player.ItemEquipped.Attack) * mult) - _gameState.Enemy.Defense;
			if (expectedDamage < 1) expectedDamage = 1;
			int actualDamage = _gameState.Enemy.MaxHealth - _gameState.Enemy.CurrentHealth;
			Assert.AreEqual(expectedDamage, actualDamage, _resultMessage);
		}

		[Then("the ability cooldown should be set to {int}")]
		public void ThenTheAbilityCooldownShouldBeSetTo(int cooldown)
		{
			Assert.AreEqual(cooldown, _gameState.Player.AbilityCooldown, _resultMessage);
		}
		[Then("the defense buff should be set to {int}")]
		public void ThenTheDefenseBuffShouldBeSetTo(int defenseBuff)
		{
			Assert.AreEqual(defenseBuff, _gameState.Player.DefenseBuff, _resultMessage);
		}
		[Then("the damage received should be reduced by the defense buff")]
		public void ThenTheUserShouldReceiveDamageEqualToTheEnemysAttackMinusPlayerDefense()
		{
			int expectedDamage = _gameState.Enemy.Attack - (_gameState.Player.Defense + _gameState.Player.ItemEquipped.Defense-_gameState.Player.DefenseBuff);
			if (expectedDamage < 1) expectedDamage = 1;

			int actualDamage = _gameState.Player.MaxHealth - _gameState.Player.CurrentHealth;
			Assert.AreEqual(expectedDamage, actualDamage, _resultMessage);
		}
		[Then("the enemy should receive damage equal to the user's attack minus enemy defense, twice")]
		public void ThenTheEnemyShouldReceiveDamageEqualToTheUsersAttackTimesMinusEnemyDefenseTwice()
		{
			int expectedDamage = (int)Math.Floor((double)(_gameState.Player.Attack + _gameState.Player.ItemEquipped.Attack)) - _gameState.Enemy.Defense;
			if (expectedDamage < 1) expectedDamage = 1;
			expectedDamage *= 2; // Double the damage for the second attack
			int actualDamage = _gameState.Enemy.MaxHealth - _gameState.Enemy.CurrentHealth;
			Assert.AreEqual(expectedDamage, actualDamage, _resultMessage);
		}
	}
}