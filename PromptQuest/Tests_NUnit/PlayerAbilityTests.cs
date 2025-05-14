using NUnit.Framework;
using PromptQuest.Services;
using PromptQuest.Models;
using NUnit.Framework.Internal;

namespace PlayerAbilityTests.Tests
{
	[TestFixture]
	public class PlayerAbilityTests
	{
		private GameState _gameState;
		private ICombatService _combatService;

		[SetUp]
		public void SetUp()
		{
			_combatService = new CombatService();
			_gameState = new GameState
			{
				Player = new Player
				{
					CurrentHealth = 10,
					MaxHealth = 10,
					Attack = 3,
					Defense = 2,
					Name = "TestPlayer",

				}
			};
			_combatService.StartCombat(_gameState);
			_gameState.Enemy.CurrentHealth = 10;
			_gameState.Enemy.MaxHealth = 10;
			_gameState.Player.Attack = 7;
			_gameState.Enemy.Defense = 0;
			_gameState.IsPlayersTurn = true;
		}

		[Test]
		[Category("scrum-104")]
		public void WarriorAbilityFunctionality()
		{
			// Arrange  
			_gameState.Player.Class = "Warrior";

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			int expectedDamage = (int)Math.Floor((double)(_gameState.Player.Attack + _gameState.Player.ItemEquipped.Attack) * 2) - _gameState.Enemy.Defense;
			if (expectedDamage < 1) expectedDamage = 1;
			int actualDamage = _gameState.Enemy.MaxHealth - _gameState.Enemy.CurrentHealth;
			Assert.That(actualDamage, Is.EqualTo(expectedDamage));
		}

		[Test]
		[Category("scrum-104")]
		public void WarriorAbilityDisablesAfterUse()
		{
			// Arrange  
			_gameState.Player.Class = "Warrior";

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(3));
		}

		[Test]
		[Category("scrum-104")]
		public void WarriorAbilityCooldownDecreasesOnAttack()
		{
			// Arrange  
			_gameState.Player.Class = "Warrior";
			_combatService.StartCombat(_gameState);
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.PlayerAttack(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(2));
		}

		[Test]
		[Category("scrum-104")]
		public void WarriorAbilityCooldownResetsOnCombatStarting()
		{
			// Arrange  
			_gameState.Player.Class = "Warrior";
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.StartCombat(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(0));
		}

		[Test]
		[Category("scrum-106")]
		public void MageAbilityFunctionality()
		{
			// Arrange  
			_gameState.Player.Class = "Mage";

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(4));
		}

		[Test]
		[Category("scrum-106")]
		public void MageAbilityCooldownDecreasesOnAttack()
		{
			// Arrange  
			_gameState.Player.Class = "Mage";
			_combatService.StartCombat(_gameState);
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.PlayerAttack(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(2));
		}

		[Test]
		[Category("scrum-106")]
		public void MageAbilityCooldownResetsOnCombatStarting()
		{
			// Arrange  
			_gameState.Player.Class = "Mage";
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.StartCombat(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(0));
		}

		[Test]
		[Category("defenseBuff")]
		public void DefenseBuffResetsOnCombatStarting()
		{
			// Arrange  
			_gameState.Player.DefenseBuff = 6;

			// Act  
			_combatService.StartCombat(_gameState);

			// Assert  
			Assert.That(_gameState.Player.DefenseBuff, Is.EqualTo(0));
		}

		[Test]
		[Category("scrum-106")]
		public void DefenseBuffOnMageAbility()
		{
			// Arrange  
			_gameState.Player.Class = "Mage";

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			Assert.That(_gameState.Player.DefenseBuff, Is.EqualTo(6));
		}

		[Test]
		[Category("defenseBuff")]
		public void DefenseBuffAppliesToEnemyAttack()
		{
			// Arrange  
			_combatService.StartCombat(_gameState);
			_gameState.Player.DefenseBuff = 6;

			// Act  
			_combatService.EnemyAttack(_gameState);

			// Assert  
			int expectedDamage = _gameState.Enemy.Attack - (_gameState.Player.Defense + _gameState.Player.ItemEquipped.Defense - _gameState.Player.DefenseBuff);
			if (expectedDamage < 1) expectedDamage = 1;

			int actualDamage = _gameState.Player.MaxHealth - _gameState.Player.CurrentHealth;
			Assert.That(actualDamage, Is.EqualTo(expectedDamage));
		}

		[Test]
		[Category("defenseBuff")]
		public void DefenseBuffResetsOnEnemyAttack()
		{
			// Arrange  
			_combatService.StartCombat(_gameState);
			_gameState.Player.DefenseBuff = 6;

			// Act  
			_combatService.EnemyAttack(_gameState);

			// Assert  
			Assert.That(_gameState.Player.DefenseBuff, Is.EqualTo(0));
		}

		[Test]
		[Category("scrum-105")]
		public void ArcherAbilityAttacksTwice()
		{
			// Arrange  
			_gameState.Player.Class = "Archer";
			_combatService.StartCombat(_gameState);

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			int expectedDamage = (int)Math.Floor((double)(_gameState.Player.Attack + _gameState.Player.ItemEquipped.Attack)) - _gameState.Enemy.Defense;
			if (expectedDamage < 1) expectedDamage = 1;
			expectedDamage *= 2; // Double the damage for the second attack
			int actualDamage = _gameState.Enemy.MaxHealth - _gameState.Enemy.CurrentHealth;
			Assert.That(expectedDamage, Is.EqualTo(actualDamage));
		}

		[Test]
		[Category("scrum-105")]
		public void ArcherAbilityCooldown()
		{
			// Arrange  
			_gameState.Player.Class = "Archer";
			_combatService.StartCombat(_gameState);

			// Act  
			_combatService.PlayerAbility(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(3));
		}

		[Test]
		[Category("scrum-105")]
		public void ArcherAbilityCooldownDecreasesOnAttack()
		{
			// Arrange  
			_gameState.Player.Class = "Archer";
			_combatService.StartCombat(_gameState);
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.PlayerAttack(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(2));
		}

		[Test]
		[Category("scrum-105")]
		public void ArcherAbilityCooldownResetsOnCombatStarting()
		{
			// Arrange  
			_gameState.Player.Class = "Archer";
			_gameState.Player.AbilityCooldown = 3;

			// Act  
			_combatService.StartCombat(_gameState);

			// Assert  
			Assert.That(_gameState.Player.AbilityCooldown, Is.EqualTo(0));
		}
	}
}
