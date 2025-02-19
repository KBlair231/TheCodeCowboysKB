using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using PromptQuest.Models;

namespace PlayerModelTest.Tests
{
	[TestFixture]
	public class PlayerModelTests
	{
		[Test]
		[TestCase("ValidName", true)]
		[TestCase("Valid Name", true)]
		[TestCase("Invalid123", false)]
		[TestCase("Invalid@Name", false)]
		[TestCase("", false)]
		public void PlayerModel_NameValidation(string name, bool expectedIsValid)
		{
			// Arrange
			var player = new PlayerModel
			{
				Name = name
			};

			// Act
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(player, null, null);
			bool isValid = Validator.TryValidateObject(player, validationContext, validationResults, true);

			// Assert
			Assert.AreEqual(expectedIsValid, isValid);
		}
	}
}