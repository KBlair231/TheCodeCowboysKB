using Moq;
using NUnit.Framework;
using PromptQuest.Models;
using PromptQuest.Services;

namespace PromptQuest.Tests.Services {
	// This service simply routes actions to their proper services and delegates updates to the session or db. So, the tests here are slim and redundant in some cases.
	// Most unit tests for services should reside in the unit test file for their respective service.
	[TestFixture]
	public class GameServiceTests {
		private GameService _gameService;
		private Mock<ISessionService> _mockSessionService;
		private Mock<IDatabaseService> _mockDatabaseService;
		private Mock<ICombatService> _mockCombatService;
		private Mock<IMapService> _mockMapService;

		[SetUp]
		public void SetUp() {
			// Initialize mocks
			_mockSessionService = new Mock<ISessionService>();
			_mockDatabaseService = new Mock<IDatabaseService>();
			_mockCombatService = new Mock<ICombatService>();
			_mockMapService = new Mock<IMapService>();

			// Initialize GameService with mocks
			_gameService = new GameService(
					_mockSessionService.Object,
					_mockDatabaseService.Object,
					_mockCombatService.Object,
					_mockMapService.Object
			);
		}

		[Test]
		public void StartNewGame_AuthenticatedUser_GameStateIsUpdatedInDatabaseAndSession() {
			// Arrange
			var userGoogleId = "user123";
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(true);
			_mockDatabaseService.Setup(db => db.GetUserGoogleId()).Returns(userGoogleId);

			// Act
			_gameService.StartNewGame();

			// Assert
			_mockDatabaseService.Verify(db => db.AddOrUpdateGameState(It.Is<GameState>(gs => gs.UserGoogleId == userGoogleId)),Times.Once);
			_mockSessionService.Verify(session => session.UpdateGameState(It.IsAny<GameState>()),Times.Once);
		}

		[Test]
		public void GetGameState_AuthenticatedUser_ReturnsGameStateFromDatabase() {
			// Arrange
			var userGoogleId = "user123";
			var gameState = new GameState { UserGoogleId = userGoogleId };
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(true);
			_mockDatabaseService.Setup(db => db.GetUserGoogleId()).Returns(userGoogleId);
			_mockDatabaseService.Setup(db => db.GetGameState(userGoogleId)).Returns(gameState);

			// Act
			var result = _gameService.GetGameState();

			// Assert
			Assert.That(result,Is.EqualTo(gameState));
		}

		[Test]
		public void GetGameState_UnauthenticatedUser_ReturnsGameStateFromSession() {
			// Arrange
			var gameState = new GameState();
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(false);
			_mockSessionService.Setup(session => session.GetGameState()).Returns(gameState);

			// Act
			var result = _gameService.GetGameState();

			// Assert
			Assert.That(result,Is.EqualTo(gameState));
		}

		[Test]
		public void DoesUserHaveSavedGame_NoAuthenticatedUser_ReturnsFalse() {
			// Arrange
			var gameState = new GameState();
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(false);
			_mockSessionService.Setup(session => session.GetGameState()).Returns(gameState);

			// Act
			var result = _gameService.DoesUserHaveSavedGame();

			// Assert
			Assert.That(result,Is.False);
		}

		[Test]
		public void DoesUserHaveSavedGame_AuthenticatedUserWithSavedGame_ReturnsTrue() {
			// Arrange
			var gameState = new GameState();
			var userGoogleId = "user123";
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(true);
			_mockDatabaseService.Setup(db => db.GetUserGoogleId()).Returns(userGoogleId);
			_mockDatabaseService.Setup(db => db.GetGameState(userGoogleId)).Returns(gameState);

			// Act
			var result = _gameService.DoesUserHaveSavedGame();

			// Assert
			Assert.That(result,Is.True);
		}

		[Test]
		public void DoesUserHaveSavedGame_AuthenticatedUserWithoutSavedGame_ReturnsFalse() {
			// Arrange
			var userGoogleId = "user123";
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(true);
			_mockDatabaseService.Setup(db => db.GetUserGoogleId()).Returns(userGoogleId);

			// Act
			var result = _gameService.DoesUserHaveSavedGame();

			// Assert
			Assert.That(result,Is.False);
		}

		[Test]
		public void CreateCharacter_AuthenticatedUser_DeletesOldCharacterAndUpdatesGameState() {
			// Arrange
			var player = new Player { PlayerId = 1,Name = "NewPlayer" };
			var gameState = new GameState { Player = new Player { PlayerId = 0 } };
			var userGoogleId = "user123";
			_mockDatabaseService.Setup(db => db.IsAuthenticatedUser()).Returns(true);
			_mockDatabaseService.Setup(db => db.GetUserGoogleId()).Returns(userGoogleId);
			_mockDatabaseService.Setup(db => db.GetGameState(userGoogleId)).Returns(gameState);

			// Act
			_gameService.CreateCharacter(player);

			// Assert
			_mockDatabaseService.Verify(db => db.DeletePlayer(0),Times.Once);
			_mockSessionService.Verify(session => session.UpdateGameState(It.Is<GameState>(gs => gs.Player == player)),Times.Once);
		}
	}
}
