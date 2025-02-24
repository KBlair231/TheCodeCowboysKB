using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using PromptQuest.Services;
using PromptQuest.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace PromptQuest.Tests.Services {
	[TestFixture]
	public class GameServiceTests {
		private GameService _gameService;
		private ISession _session;
		private HttpContext _httpContext;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			// Set up in-memory session
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddDistributedMemoryCache();
			var serviceProvider = serviceCollection.BuildServiceProvider();
			var memoryCache = serviceProvider.GetService<IDistributedCache>();
			var loggerFactory = new Mock<ILoggerFactory>().Object;
			var tryEstablishSession = new Func<bool>(() => true);

			_session = new DistributedSession(
					memoryCache,
					"SessionId",
					TimeSpan.FromMinutes(5),
					TimeSpan.FromMinutes(5),
					tryEstablishSession,
					loggerFactory,
					true
			);

			_httpContext = new DefaultHttpContext();
			_httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = _session });
			var httpContextAccessor = new HttpContextAccessor { HttpContext = _httpContext };

			_gameService = new GameService(httpContextAccessor);
		}

		[SetUp]
		public void SetUp() {

			// Test GameState so that values aren't null when ations are executed.
			GameState gameState = new GameState {
				Player = new Player { Name = "TestPlayer",MaxHealth = 10,CurrentHealth = 10,Attack = 1,Defense = 1 }
			};

			// Initialize the session
			_session.SetString("GameState",JsonSerializer.Serialize(gameState));

		}


		[Test]
		public void GetGameState_ShouldReturnGameState() {
			// Arrange
			string expectedPlayerName = "John the Deadly Knight";
			GameState gameState = new GameState { Player = new Player { Name = expectedPlayerName } };
			_session.SetString("GameState",JsonSerializer.Serialize(gameState));

			// Act
			GameState gameStateFromSession = _gameService.GetGameState();

			// Assert
			Assert.AreEqual(expectedPlayerName,gameStateFromSession.Player.Name);
		}

		[Test]
		public void UpdatePlayer_ShouldUpdatePlayerInGameState() {
			// Arrange
			var expectedPlayerName = "UpdatedPlayer";
			var player = new Player { Name = expectedPlayerName };

			// Act
			_gameService.UpdatePlayer(player);
			var result = _gameService.GetGameState();

			// Assert
			Assert.AreEqual(expectedPlayerName,result.Player.Name);
		}

		[Test]
		public void StartCombat_ShouldInitializeCombatState() {
			// Act
			_gameService.StartCombat();
			GameState gameState = _gameService.GetGameState();

			// Assert
			// Check that StartCombat generates an enemy
			Assert.IsNotNull(gameState.Enemy);
			// Check that StartCombat sets InCombat flag to true
			Assert.IsTrue(gameState.InCombat);
			// Check that StartCombat sets IsPlayersTurn flag to true
			Assert.IsTrue(gameState.InCombat);
		}

		/// <summary> This just tests that ExecutePlayerAction updates the gamestate in the session because there are separate tests for the actions themselves in their own services.</summary>
		[Test]
		public void ExecutePlayerAction_ShouldExecuteActionAndUpdateGameState() {
			// Arrange
			GameState gameStateBefore = _gameService.GetGameState(); // get game state before action
			_gameService.StartCombat(); // Start combat so there is an enemy

			// Act
			var result = _gameService.ExecutePlayerAction("attack");
			GameState gameStateAfter = _gameService.GetGameState();// get game state before action

			// Assert
			// Check that we at least returned something
			Assert.That(result,Is.Not.Null);
			// Make sure game state retrieved from the session changed
			Assert.That(gameStateBefore,Is.Not.EqualTo(gameStateAfter)); // Make sure game state changed 
		}

		/// <summary> This just tests that ExecuteEnemyAction updates the gamestate in the session because there are separate tests for the actions themselves in their own services</summary>
		[Test]
		public void ExecuteEnemyAction_ShouldExecuteActionAndUpdateGameState() {
			// Arrange
			GameState gameStateBefore = _gameService.GetGameState(); // get game state before action
			_gameService.StartCombat(); // Start combat so there is an enemy

			// Act
			var result = _gameService.ExecuteEnemyAction();
			GameState gameStateAfter = _gameService.GetGameState(); // get game state before action

			// Assert
			// Check that we at least returned something
			Assert.That(result,Is.Not.Null);
			// Make sure game state retrieved from the session changed
			Assert.That(gameStateBefore,Is.Not.EqualTo(gameStateAfter)); // Make sure game state changed 
		}
	}

	public class SessionFeature:ISessionFeature {
		public ISession Session { get; set; }
	}
}
