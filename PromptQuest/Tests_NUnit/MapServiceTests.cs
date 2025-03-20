using NUnit.Framework;
using PromptQuest.Models;
using PromptQuest.Services;

namespace Tests_NUnit {
	[TestFixture]
	public class MapServiceTests {
		private IMapService _mapService;

		[SetUp]
		public void Setup() {
			_mapService = new MapService();
		}

		[Test]
		public void GetMap_ShouldReturnMap() {
			// Act
			var map = _mapService.GetMap();

			// Assert
			Assert.IsNotNull(map);
			Assert.That(map.ListMapNodes.Count, Is.EqualTo(10));
			Assert.That(map.ListMapEdges.Count, Is.EqualTo(9));
		}

		[Test]
		public void MovePlayer_ShouldIncrementPlayerLocation() {
			// Arrange
			var gameState = new GameState { PlayerLocation = 1 };

			// Act
			_mapService.MovePlayer(gameState);

			// Assert
			Assert.That(gameState.PlayerLocation, Is.EqualTo(2));
			Assert.IsFalse(gameState.IsLocationComplete);
		}

		[Test]
		public void MovePlayer_ShouldNotIncrementPlayerLocation_WhenAtEndOfMap() {
			// Arrange
			var gameState = new GameState { PlayerLocation = 10 };

			// Act
			_mapService.MovePlayer(gameState);

			// Assert
			Assert.That(gameState.PlayerLocation, Is.EqualTo(10));
		}

	}
}