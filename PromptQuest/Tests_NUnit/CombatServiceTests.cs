using NUnit.Framework;
using PromptQuest.Services;
using PromptQuest.Models;
using NUnit.Framework.Internal;

namespace PromptQuest.Tests.Services {
	[TestFixture]
	public class CombatServiceTests {
		private ICombatService _combatService;
		private GameState _gameState;

		[SetUp]
		public void SetUp() {
			// Initialize CombatService and GameState.
			_combatService = new CombatService();
			// Test game state just in case, so that player or enemy can't throw null reference exceptions.
			_gameState = new GameState {
				Player = new Player { Name = "TestPlayer" },
				Enemy = new Enemy { Name = "TestEnemy" }
			};
		}

		#region Player Attack Tests

		[Test]
		public void PlayerAttack_ShouldDealDamageToEnemy() {
			// Arrange
			_combatService.StartCombat(_gameState);
			_gameState.Enemy.CurrentHealth = 10;
			_gameState.Enemy.MaxHealth = 10;
			_gameState.Player.Attack = 3;
			_gameState.Enemy.Defense = 0;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Enemy should take 1 damage -> 10 - 3 = 7
			Assert.That(_gameState.Enemy.CurrentHealth, Is.EqualTo(7));
			// Check that it is Enemy's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.False);
		}

		[Test]
		public void PlayerAttack_ShouldDealDamageToEnemyMinusDefense() {
			// Arrange
			_combatService.StartCombat(_gameState);
			_gameState.Enemy.CurrentHealth = 10;
			_gameState.Enemy.MaxHealth = 10;
			_gameState.Player.Attack = 5;
			_gameState.Enemy.Defense = 3;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Enemy should take 1 damage because 5 Attack - 3 Defense = 2 damage.
			Assert.That(_gameState.Enemy.CurrentHealth, Is.EqualTo(8));
			// Check that it is Enemy's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.False);
		}

		[Test]
		public void PlayerAttack_ShouldDealMinimumOfOneDamageToEnemy() {
			// Arrange
			_combatService.StartCombat(_gameState);
			_gameState.Enemy.CurrentHealth = 10;
			_gameState.Enemy.MaxHealth = 10;
			_gameState.Player.Attack = 1;
			_gameState.Enemy.Defense = 5;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Enemy should take 1 damage even though 1 Attack - 5 Defense = -4 damage because the minimum damage is 1.
			Assert.That(_gameState.Enemy.CurrentHealth, Is.EqualTo(9));
			// Check that it is Enemy's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.False);
		}

		#endregion Player Attack Tests - End

		#region Player Health Potions Tests 

		[Test]
		public void PlayerUseHealthPotion_ShouldHealPlayerByFive() {
			// Arrange
			_gameState.Player.HealthPotions = 2;
			_gameState.Player.CurrentHealth = 3;
			_gameState.Player.MaxHealth = 10;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerUseHealthPotion(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Health should go up 5 from 3 to a total of 8
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(8));
			// Number of potions should go down by one because it was used
			Assert.That(_gameState.Player.HealthPotions, Is.EqualTo(1));
			// Should still be player's turn.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void PlayerUseHealthPotion_ShouldNotHealPlayerPastMax() {
			// Arrange
			_gameState.Player.HealthPotions = 2;
			_gameState.Player.CurrentHealth = 7;
			_gameState.Player.MaxHealth = 10;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerUseHealthPotion(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Health should go up from 7 to 10 because health cannot be above max.
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(10));
			// Number of potions should go down by one because it was used
			Assert.That(_gameState.Player.HealthPotions, Is.EqualTo(1));
			// Should still be player's turn.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void PlayerUseHealthPotion_ShouldNotHealPlayerAtMax() {
			// Arrange
			_gameState.Player.HealthPotions = 2;
			_gameState.Player.CurrentHealth = 10;
			_gameState.Player.MaxHealth = 10;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerUseHealthPotion(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Player was already at max, so health should not change.
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(10));
			// Player is already at max, so number of potions shouldn't change.
			Assert.That(_gameState.Player.HealthPotions, Is.EqualTo(2));
			// Should still be player's turn.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void PlayerUseHealthPotion_ShouldNotHealPlayerWithZeroPotions() {
			// Arrange
			_gameState.Player.HealthPotions = 0;
			_gameState.Player.CurrentHealth = 5;
			_gameState.Player.MaxHealth = 10;
			_gameState.IsPlayersTurn = true;

			// Act
			var result = _combatService.PlayerUseHealthPotion(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Player has no potions, so health should not change.
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(5));
			// Player has no potions, so number of potions shouldn't change.
			Assert.That(_gameState.Player.HealthPotions, Is.EqualTo(0));
			// Should still be player's turn.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		#endregion Player Health Potions Tests - End

		#region Enemy Attack Tests - End

		[Test]
		public void EnemyAttack_ShouldDealDamageToPlayer() {
			// Arrange
			_gameState.Player.CurrentHealth = 10;
			_gameState.Player.MaxHealth = 10;
			_gameState.Enemy.Attack = 3;
			_gameState.Player.Defense = 0;
			_gameState.IsPlayersTurn = false;

			// Act
			var result = _combatService.EnemyAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Player should take 3 damage -> 10 - 3 = 7
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(7));
			// Check that it is Player's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void EnemyAttack_ShouldDealDamageToPlayerMinusDefense() {
			// Arrange
			_combatService.StartCombat(_gameState);
			_gameState.Player.CurrentHealth = 10;
			_gameState.Player.MaxHealth = 10;
			_gameState.Enemy.Attack = 5;
			_gameState.Player.Defense = 3;
			_gameState.IsPlayersTurn = false;

			// Act
			var result = _combatService.EnemyAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Player should take 2 damage because 5 Attack - 3 Defense = 2 damage.
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(8));
			// Check that it is Player's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void EnemyAttack_ShouldDealMinimumOfOneDamageToPlayer() {
			// Arrange
			_combatService.StartCombat(_gameState);
			_gameState.Player.CurrentHealth = 10;
			_gameState.Player.MaxHealth = 10;
			_gameState.Enemy.Attack = 1;
			_gameState.Player.Defense = 5;
			_gameState.IsPlayersTurn = false;

			// Act
			var result = _combatService.EnemyAttack(_gameState);

			// Assert
			// Check that we at least returned something
			Assert.That(result, Is.Not.Null);
			// test gameState not result because that is what actually gets saved to the session, the result is just presentation logic.
			// Player should take 1 damage even though 1 Attack - 5 Defense = -4 damage because the minimum damage is 1.
			Assert.That(_gameState.Player.CurrentHealth, Is.EqualTo(9));
			// Check that it is Player's turn now.
			Assert.That(_gameState.IsPlayersTurn, Is.True);
		}

		[Test]
		public void GetEnemy_ShouldReturnDifferentEnemiesAfterMultipleCalls() {
			// Arrange
			HashSet<string> enemyNames = new HashSet<string>();

			// Act
			for (int i = 0; i < 10; i++) {
				Enemy enemy = _combatService.GetEnemy();
				enemyNames.Add(enemy.Name);
			}

			// Assert
			// Check that for 10 enemies there are at least two different enemies within the bunch
			Assert.Greater(enemyNames.Count, 1, "GetEnemy should return different enemies after multiple calls.");
		}

		#endregion Enemy Attack Tests - End
	}
}
