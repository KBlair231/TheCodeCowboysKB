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
			Assert.That(map.ListMapNodes.Count, Is.EqualTo(18));
		}

	}
}