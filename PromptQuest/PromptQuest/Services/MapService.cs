using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IMapService {
		public void MovePlayer(GameState gameState, int mapNodeId = 0);
		public Map GetMap();
	}
	public class MapService : IMapService {

		private static readonly List<MapNode> _mapNodes = new List<MapNode>
			{
					new MapNode { MapNodeId = 1 },
					new MapNode { MapNodeId = 2 },
					new MapNode { MapNodeId = 3, NodeType = "Event" },
					new MapNode { MapNodeId = 4 },
					new MapNode { MapNodeId = 5, NodeType = "Campsite" },
					new MapNode { MapNodeId = 6 },
					new MapNode { MapNodeId = 7 },
					new MapNode { MapNodeId = 8, NodeType = "Campsite" },
					new MapNode { MapNodeId = 9},
					new MapNode { MapNodeId = 10, NodeType = "Boss" }
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

		// Moves the player to the mapNode with the given mapNodeId, if mapNodeId isn't provided, the player's location (mapNodeId) increases by 1.
		public void MovePlayer(GameState gameState, int mapNodeId = 0) {
			// Check if the player has reached the end of the map.
			int mapNodeIdFinal = _mapNodes.Max(x => x.MapNodeId);
			gameState.IsLocationComplete = false;
			if(gameState.PlayerLocation == mapNodeIdFinal) {
				gameState.PlayerLocation = 1; // Reset the player location to the first room.
				gameState.Floor++; // Increment the floor number.
			}
			else if(mapNodeId == 0) {
				//Move forward one location. Only works for now because map is linear.
				gameState.PlayerLocation++;
			}
			else {
				// Set player's location to the selected mapNode
				gameState.PlayerLocation = mapNodeId;
			}
			gameState.IsLocationComplete = false;
			// Zero out GameState status
			gameState.InCombat = false;
			gameState.InCampsite = false;
			gameState.InEvent = false;
			// Check if the player is on a campsite or event node
			var currentNode = _mapNodes.FirstOrDefault(node => node.MapNodeId == gameState.PlayerLocation);
			if (currentNode != null && currentNode.NodeType == "Campsite") {
				gameState.AddMessage("You have found a campsite. Rest here to heal 30% of your maximum HP and refill your health potions.");
				gameState.InCampsite = true;
				return;
			}
			if (currentNode != null && currentNode.NodeType == "Event") {
				gameState.AddMessage("A prickly bush lies in your path. A few red objects shimmer from fairly deep inside.");
				gameState.AddMessage("Reach in and grab them?");
				gameState.InEvent = true;
				return;
			}
			if (currentNode.NodeType == "Enemy" || currentNode.NodeType == "Boss") {
				gameState.InCombat = true;
			}
		}

		public Map GetMap() {
			return _map;
		}
	}
}
