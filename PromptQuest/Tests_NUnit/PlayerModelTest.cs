using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using PromptQuest.Models;

namespace PlayerModelTest.Tests {
	[TestFixture]
	public class PlayerModelTests {
		private Player _player;

		[SetUp]
		public void Setup() {
			_player = new Player {
				Name = "ValidName",
				HealthPotions = 2,
				MaxHealth = 100,
				CurrentHealth = 100,
				Defense = 1,
				Attack = 10,
				Class = "Warrior" // Set a default class for testing
			};
		}

		[Test]
		[TestCase("ValidName", true)]
		[TestCase("Valid Name", true)]
		[TestCase("Invalid123", false)]
		[TestCase("Invalid@Name", false)]
		[TestCase("", false)]
		public void PlayerModel_NameValidation(string name, bool expectedIsValid) {
			// Arrange
			_player.Name = name;

			// Act
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(_player, null, null);
			bool isValid = Validator.TryValidateObject(_player, validationContext, validationResults, true);

			// Assert
			Assert.AreEqual(expectedIsValid, isValid);
		}

		[Test]
		[TestCase("Warrior", true)]
		[TestCase("Mage", true)]
		[TestCase("Archer", true)]
		[TestCase("", false)]
		public void PlayerModel_ClassValidation(string className, bool expectedIsValid) {
			// Arrange
			_player.Class = className;

			// Act
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(_player, null, null);
			bool isValid = Validator.TryValidateObject(_player, validationContext, validationResults, true);

			// Assert
			Assert.AreEqual(expectedIsValid, isValid);
		}
	}
}