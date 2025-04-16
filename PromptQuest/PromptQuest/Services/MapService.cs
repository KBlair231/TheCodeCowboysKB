﻿using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IMapService {
		public void MovePlayer(GameState gameState, int mapNodeId = 0);
		public Map GetMap();
	}
	public class MapService:IMapService {
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
		private static readonly Map _map = new Map() { ListMapNodes = _mapNodes,ListMapEdges = _mapEdges };

		// Moves the player to the mapNode with the given mapNodeId, if mapNodeId isn't provided, the player's location (mapNodeId) increases by 1.
		public void MovePlayer(GameState gameState, int mapNodeId = 0) {
			// Check if the player has reached the end of the map.
			int mapNodeIdFinal = _mapNodes.Max(x => x.MapNodeId);
			gameState.IsLocationComplete = false;
			if (gameState.PlayerLocation == mapNodeIdFinal) {
				gameState.PlayerLocation = 1; // Reset the player location to the first room.
				gameState.Floor++; // Increment the floor number.
				return;
			}
			if(mapNodeId == 0) {
				//Move forward one location. Only works for now because map is linear.
				gameState.PlayerLocation++;
				return;
			}
			// Set player's location to the selected mapNode
			gameState.PlayerLocation = mapNodeId;
		}

		public Map GetMap() {
			return _map;
		}
	}
}
