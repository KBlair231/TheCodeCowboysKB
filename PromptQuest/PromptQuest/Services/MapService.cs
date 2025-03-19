using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IMapService {
		public void MovePlayer(GameState gameState);
		public Map GetMap();
	}
	public class MapService: IMapService {
		private static readonly List<MapNode> _mapNodes = new List<MapNode>
			{
					new MapNode { MapNodeId = 1 },
					new MapNode { MapNodeId = 2 },
					new MapNode { MapNodeId = 3 },
					new MapNode { MapNodeId = 4 },
					new MapNode { MapNodeId = 5 },
					new MapNode { MapNodeId = 6 },
					new MapNode { MapNodeId = 7 },
					new MapNode { MapNodeId = 8 },
					new MapNode { MapNodeId = 9 },
					new MapNode { MapNodeId = 10 }
			};
		private static readonly List<MapEdge> _mapEdges = new List<MapEdge>() {
					new MapEdge { MapNodeIdStart = 1, MapNodeIdEnd = 2 },
					new MapEdge { MapNodeIdStart = 2, MapNodeIdEnd = 3 },
					new MapEdge { MapNodeIdStart = 3, MapNodeIdEnd = 4 },
					new MapEdge { MapNodeIdStart = 4, MapNodeIdEnd = 5 },
					new MapEdge { MapNodeIdStart = 5, MapNodeIdEnd = 6 },
					new MapEdge { MapNodeIdStart = 6, MapNodeIdEnd = 7 },
					new MapEdge { MapNodeIdStart = 7, MapNodeIdEnd = 8 },
					new MapEdge { MapNodeIdStart = 8, MapNodeIdEnd = 9 },
					new MapEdge { MapNodeIdStart = 9, MapNodeIdEnd = 10 }
				};
		private static readonly Map _map = new Map() { ListMapNodes = _mapNodes, ListMapEdges = _mapEdges };

		public void MovePlayer(GameState gameState) {
			// Check if the player has reached the end of the map.
			int mapNodeIdFinal = _mapNodes.Max(x => x.MapNodeId);
			if(gameState.PlayerLocation == mapNodeIdFinal) {
				return;
			}
			gameState.PlayerLocation++;
			gameState.IsLocationComplete = false;
			return;
		}

		public Map GetMap() {
			return _map;
		}
	}
}
