using Microsoft.AspNetCore.Mvc;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IMapService {
		public void MovePlayer(GameState gameState, int mapNodeId = 0, bool admin = false);
		public Map GetMap(int floor);
	}
	public class MapService : IMapService {

		#region GetMapNodes
		// Funtion to get the correct map nodes for the current floor
		private List<MapNode> GetMapNodesForFloor(int floor) {
			switch(floor) {
				case 1:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3}, NodeHeight = 2, NodeDistance = 1},
						new MapNode { MapNodeId = 2, NodeType = "Enemy", ConnectedNodes = {4, 5}, NodeHeight = 1, NodeDistance = 2},
						new MapNode { MapNodeId = 3, NodeType = "Event", ConnectedNodes = {5, 6}, NodeHeight = 3, NodeDistance = 2},
						new MapNode { MapNodeId = 4, NodeType = "Campsite", ConnectedNodes = {7}, NodeHeight = 1, NodeDistance = 3},
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {7}, NodeHeight = 2, NodeDistance = 3},
						new MapNode { MapNodeId = 6, NodeType = "Treasure", ConnectedNodes = {8}, NodeHeight = 4, NodeDistance = 3},
						new MapNode { MapNodeId = 7, NodeType = "Enemy", ConnectedNodes = {9}, NodeHeight = 1, NodeDistance = 4},
						new MapNode { MapNodeId = 8, NodeType = "Campsite", ConnectedNodes = {9}, NodeHeight = 3, NodeDistance = 4},
						new MapNode { MapNodeId = 9, NodeType = "Enemy", ConnectedNodes = {10, 11}, NodeHeight = 2, NodeDistance = 5},
						new MapNode { MapNodeId = 10, NodeType = "Shop", ConnectedNodes = {12}, NodeHeight = 1, NodeDistance = 6},
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {12}, NodeHeight = 3, NodeDistance = 6},
						new MapNode { MapNodeId = 12, NodeType = "Enemy", ConnectedNodes = {13, 14, 15}, NodeHeight = 2, NodeDistance = 7},
						new MapNode { MapNodeId = 13, NodeType = "Treasure", ConnectedNodes = {16}, NodeHeight = 1, NodeDistance = 8},
						new MapNode { MapNodeId = 14, NodeType = "Campsite", ConnectedNodes = {17}, NodeHeight = 2, NodeDistance = 8},
						new MapNode { MapNodeId = 15, NodeType = "Event", ConnectedNodes = {17}, NodeHeight = 3, NodeDistance = 8},
						new MapNode { MapNodeId = 16, NodeType = "Shop", ConnectedNodes = {18}, NodeHeight = 1, NodeDistance = 9},
						new MapNode { MapNodeId = 17, NodeType = "Enemy", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 9},
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 2, NodeDistance = 10}
					};
				case 2:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3}, NodeHeight = 3, NodeDistance = 1},
						new MapNode { MapNodeId = 2, NodeType = "Enemy", ConnectedNodes = {4}, NodeHeight = 2, NodeDistance = 2},
						new MapNode { MapNodeId = 3, NodeType = "Event", ConnectedNodes = {5}, NodeHeight = 4, NodeDistance = 2},
						new MapNode { MapNodeId = 4, NodeType = "Enemy", ConnectedNodes = {6}, NodeHeight = 2, NodeDistance = 3},
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {7}, NodeHeight = 4, NodeDistance = 3},
						new MapNode { MapNodeId = 6, NodeType = "Treasure", ConnectedNodes = {8, 9}, NodeHeight = 2, NodeDistance = 4},
						new MapNode { MapNodeId = 7, NodeType = "Enemy", ConnectedNodes = {9, 10}, NodeHeight = 4, NodeDistance = 4},
						new MapNode { MapNodeId = 8, NodeType = "Event", ConnectedNodes = {11}, NodeHeight = 1, NodeDistance = 5},
						new MapNode { MapNodeId = 9, NodeType = "Campsite", ConnectedNodes = {11}, NodeHeight = 3, NodeDistance = 5},
						new MapNode { MapNodeId = 10, NodeType = "Treasure", ConnectedNodes = {11}, NodeHeight = 5, NodeDistance = 5},
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {12, 13}, NodeHeight = 3, NodeDistance = 6},
						new MapNode { MapNodeId = 12, NodeType = "Enemy", ConnectedNodes = {15}, NodeHeight = 2, NodeDistance = 7},
						new MapNode { MapNodeId = 13, NodeType = "Enemy", ConnectedNodes = {14}, NodeHeight = 4, NodeDistance = 7},
						new MapNode { MapNodeId = 14, NodeType = "Event", ConnectedNodes = {17}, NodeHeight = 2, NodeDistance = 8},
						new MapNode { MapNodeId = 15, NodeType = "Shop", ConnectedNodes = {16}, NodeHeight = 4, NodeDistance = 8},
						new MapNode { MapNodeId = 16, NodeType = "Enemy", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 9},
						new MapNode { MapNodeId = 17, NodeType = "Enemy", ConnectedNodes = {18}, NodeHeight = 4, NodeDistance = 9},
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 3, NodeDistance = 10}
					};
				case 3:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3}, NodeHeight = 2, NodeDistance = 1 },
						new MapNode { MapNodeId = 2, NodeType = "Event", ConnectedNodes = {4}, NodeHeight = 1, NodeDistance = 2 },
						new MapNode { MapNodeId = 3, NodeType = "Enemy", ConnectedNodes = {5, 6}, NodeHeight = 3, NodeDistance = 2 },
						new MapNode { MapNodeId = 4, NodeType = "Enemy", ConnectedNodes = {7}, NodeHeight = 1, NodeDistance = 3 },
						new MapNode { MapNodeId = 5, NodeType = "Treasure", ConnectedNodes = {7}, NodeHeight = 2, NodeDistance = 3 },
						new MapNode { MapNodeId = 6, NodeType = "Campsite", ConnectedNodes = {8}, NodeHeight = 4, NodeDistance = 3 },
						new MapNode { MapNodeId = 7, NodeType = "Enemy", ConnectedNodes = {9}, NodeHeight = 1, NodeDistance = 4 },
						new MapNode { MapNodeId = 8, NodeType = "Enemy", ConnectedNodes = {9, 10}, NodeHeight = 3, NodeDistance = 4 },
						new MapNode { MapNodeId = 9, NodeType = "Enemy", ConnectedNodes = {11}, NodeHeight = 2, NodeDistance = 5 },
						new MapNode { MapNodeId = 10, NodeType = "Treasure", ConnectedNodes = {11, 12}, NodeHeight = 4, NodeDistance = 5 },
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {13}, NodeHeight = 1, NodeDistance = 6 },
						new MapNode { MapNodeId = 12, NodeType = "Enemy", ConnectedNodes = {13}, NodeHeight = 4, NodeDistance = 6 },
						new MapNode { MapNodeId = 13, NodeType = "Campsite", ConnectedNodes = {14, 15}, NodeHeight = 2, NodeDistance = 7 },
						new MapNode { MapNodeId = 14, NodeType = "Event", ConnectedNodes = {16}, NodeHeight = 1, NodeDistance = 8 },
						new MapNode { MapNodeId = 15, NodeType = "Enemy", ConnectedNodes = {16, 17}, NodeHeight = 3, NodeDistance = 8 },
						new MapNode { MapNodeId = 16, NodeType = "Shop", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 9 },
						new MapNode { MapNodeId = 17, NodeType = "Treasure", ConnectedNodes = {18}, NodeHeight = 4, NodeDistance = 9 },
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 2, NodeDistance = 10 }
					};
				case 4:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3}, NodeHeight = 2, NodeDistance = 1 },
						new MapNode { MapNodeId = 2, NodeType = "Event", ConnectedNodes = {4, 5}, NodeHeight = 1, NodeDistance = 2 },
						new MapNode { MapNodeId = 3, NodeType = "Campsite", ConnectedNodes = {5, 6}, NodeHeight = 3, NodeDistance = 2 },
						new MapNode { MapNodeId = 4, NodeType = "Treasure", ConnectedNodes = {7}, NodeHeight = 1, NodeDistance = 3 },
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {7}, NodeHeight = 2, NodeDistance = 3 },
						new MapNode { MapNodeId = 6, NodeType = "Enemy", ConnectedNodes = {8}, NodeHeight = 4, NodeDistance = 3 },
						new MapNode { MapNodeId = 7, NodeType = "Enemy", ConnectedNodes = {9}, NodeHeight = 1, NodeDistance = 4 },
						new MapNode { MapNodeId = 8, NodeType = "Event", ConnectedNodes = {9, 10}, NodeHeight = 3, NodeDistance = 4 },
						new MapNode { MapNodeId = 9, NodeType = "Enemy", ConnectedNodes = {11}, NodeHeight = 2, NodeDistance = 5 },
						new MapNode { MapNodeId = 10, NodeType = "Treasure", ConnectedNodes = {12}, NodeHeight = 4, NodeDistance = 5 },
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {13}, NodeHeight = 2, NodeDistance = 6 },
						new MapNode { MapNodeId = 12, NodeType = "Campsite", ConnectedNodes = {13}, NodeHeight = 4, NodeDistance = 6 },
						new MapNode { MapNodeId = 13, NodeType = "Enemy", ConnectedNodes = {14, 15}, NodeHeight = 3, NodeDistance = 7 },
						new MapNode { MapNodeId = 14, NodeType = "Shop", ConnectedNodes = {16}, NodeHeight = 2, NodeDistance = 8 },
						new MapNode { MapNodeId = 15, NodeType = "Event", ConnectedNodes = {16, 17}, NodeHeight = 4, NodeDistance = 8 },
						new MapNode { MapNodeId = 16, NodeType = "Enemy", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 9 },
						new MapNode { MapNodeId = 17, NodeType = "Treasure", ConnectedNodes = {18}, NodeHeight = 4, NodeDistance = 9 },
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 3, NodeDistance = 10 }
					};
				case 5:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3}, NodeHeight = 2, NodeDistance = 1 },
						new MapNode { MapNodeId = 2, NodeType = "Treasure", ConnectedNodes = {4, 5}, NodeHeight = 1, NodeDistance = 2 },
						new MapNode { MapNodeId = 3, NodeType = "Event", ConnectedNodes = {6}, NodeHeight = 3, NodeDistance = 2 },
						new MapNode { MapNodeId = 4, NodeType = "Enemy", ConnectedNodes = {7}, NodeHeight = 1, NodeDistance = 3 },
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {8}, NodeHeight = 2, NodeDistance = 3 },
						new MapNode { MapNodeId = 6, NodeType = "Campsite", ConnectedNodes = {8}, NodeHeight = 4, NodeDistance = 3 },
						new MapNode { MapNodeId = 7, NodeType = "Event", ConnectedNodes = {9}, NodeHeight = 1, NodeDistance = 4 },
						new MapNode { MapNodeId = 8, NodeType = "Enemy", ConnectedNodes = {9, 10}, NodeHeight = 3, NodeDistance = 4 },
						new MapNode { MapNodeId = 9, NodeType = "Enemy", ConnectedNodes = {11}, NodeHeight = 2, NodeDistance = 5 },
						new MapNode { MapNodeId = 10, NodeType = "Shop", ConnectedNodes = {12}, NodeHeight = 4, NodeDistance = 5 },
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {13}, NodeHeight = 1, NodeDistance = 6 },
						new MapNode { MapNodeId = 12, NodeType = "Enemy", ConnectedNodes = {13}, NodeHeight = 4, NodeDistance = 6 },
						new MapNode { MapNodeId = 13, NodeType = "Enemy", ConnectedNodes = {14, 15}, NodeHeight = 2, NodeDistance = 7 },
						new MapNode { MapNodeId = 14, NodeType = "Event", ConnectedNodes = {16}, NodeHeight = 1, NodeDistance = 8 },
						new MapNode { MapNodeId = 15, NodeType = "Enemy", ConnectedNodes = {16, 17}, NodeHeight = 3, NodeDistance = 8 },
						new MapNode { MapNodeId = 16, NodeType = "Treasure", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 9 },
						new MapNode { MapNodeId = 17, NodeType = "Campsite", ConnectedNodes = {18}, NodeHeight = 4, NodeDistance = 9 },
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 3, NodeDistance = 10 }
					};
				case 6:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3, 4}, NodeHeight = 2, NodeDistance = 1 },
						new MapNode { MapNodeId = 2, NodeType = "Event", ConnectedNodes = {5}, NodeHeight = 1, NodeDistance = 2 },
						new MapNode { MapNodeId = 3, NodeType = "Enemy", ConnectedNodes = {5, 6}, NodeHeight = 2, NodeDistance = 2 },
						new MapNode { MapNodeId = 4, NodeType = "Enemy", ConnectedNodes = {6}, NodeHeight = 4, NodeDistance = 2 },
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {7, 8}, NodeHeight = 1, NodeDistance = 3 },
						new MapNode { MapNodeId = 6, NodeType = "Campsite", ConnectedNodes = {8, 9}, NodeHeight = 3, NodeDistance = 3 },
						new MapNode { MapNodeId = 7, NodeType = "Shop", ConnectedNodes = {10}, NodeHeight = 1, NodeDistance = 4 },
						new MapNode { MapNodeId = 8, NodeType = "Enemy", ConnectedNodes = {10, 11}, NodeHeight = 2, NodeDistance = 4 },
						new MapNode { MapNodeId = 9, NodeType = "Event", ConnectedNodes = {11}, NodeHeight = 4, NodeDistance = 4 },
						new MapNode { MapNodeId = 10, NodeType = "Enemy", ConnectedNodes = {12}, NodeHeight = 1, NodeDistance = 5 },
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {12, 13}, NodeHeight = 3, NodeDistance = 5 },
						new MapNode { MapNodeId = 12, NodeType = "Enemy", ConnectedNodes = {14}, NodeHeight = 2, NodeDistance = 6 },
						new MapNode { MapNodeId = 13, NodeType = "Enemy", ConnectedNodes = {14, 15}, NodeHeight = 4, NodeDistance = 6 },
						new MapNode { MapNodeId = 14, NodeType = "Campsite", ConnectedNodes = {16}, NodeHeight = 1, NodeDistance = 7 },
						new MapNode { MapNodeId = 15, NodeType = "Event", ConnectedNodes = {16, 17}, NodeHeight = 3, NodeDistance = 7 },
						new MapNode { MapNodeId = 16, NodeType = "Enemy", ConnectedNodes = {18}, NodeHeight = 2, NodeDistance = 8 },
						new MapNode { MapNodeId = 17, NodeType = "Treasure", ConnectedNodes = {18}, NodeHeight = 4, NodeDistance = 8 },
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 1, NodeDistance = 9 }
					};
				case 7:
					return new List<MapNode>
					{
						new MapNode { MapNodeId = 1, NodeType = "Enemy", ConnectedNodes = {2, 3, 4}, NodeHeight = 3, NodeDistance = 1 },
						new MapNode { MapNodeId = 2, NodeType = "Event", ConnectedNodes = {5, 6}, NodeHeight = 1, NodeDistance = 2 },
						new MapNode { MapNodeId = 3, NodeType = "Enemy", ConnectedNodes = {6, 7}, NodeHeight = 3, NodeDistance = 2 },
						new MapNode { MapNodeId = 4, NodeType = "Enemy", ConnectedNodes = {8}, NodeHeight = 4, NodeDistance = 2 },
						new MapNode { MapNodeId = 5, NodeType = "Enemy", ConnectedNodes = {9}, NodeHeight = 1, NodeDistance = 3 },
						new MapNode { MapNodeId = 6, NodeType = "Campsite", ConnectedNodes = {9, 10}, NodeHeight = 2, NodeDistance = 3 },
						new MapNode { MapNodeId = 7, NodeType = "Shop", ConnectedNodes = {10, 11}, NodeHeight = 4, NodeDistance = 3 },
						new MapNode { MapNodeId = 8, NodeType = "Event", ConnectedNodes = {11}, NodeHeight = 3, NodeDistance = 3 },
						new MapNode { MapNodeId = 9, NodeType = "Enemy", ConnectedNodes = {12}, NodeHeight = 1, NodeDistance = 4 },
						new MapNode { MapNodeId = 10, NodeType = "Enemy", ConnectedNodes = {12, 13}, NodeHeight = 3, NodeDistance = 4 },
						new MapNode { MapNodeId = 11, NodeType = "Elite", ConnectedNodes = {13, 14}, NodeHeight = 4, NodeDistance = 4 },
						new MapNode { MapNodeId = 12, NodeType = "Treasure", ConnectedNodes = {15}, NodeHeight = 2, NodeDistance = 5 },
						new MapNode { MapNodeId = 13, NodeType = "Event", ConnectedNodes = {15}, NodeHeight = 3, NodeDistance = 5 },
						new MapNode { MapNodeId = 14, NodeType = "Campsite", ConnectedNodes = {16}, NodeHeight = 4, NodeDistance = 5 },
						new MapNode { MapNodeId = 15, NodeType = "Enemy", ConnectedNodes = {17}, NodeHeight = 2, NodeDistance = 6 },
						new MapNode { MapNodeId = 16, NodeType = "Treasure", ConnectedNodes = {17, 18}, NodeHeight = 4, NodeDistance = 7 },
						new MapNode { MapNodeId = 17, NodeType = "Event", ConnectedNodes = {18}, NodeHeight = 3, NodeDistance = 8 },
						new MapNode { MapNodeId = 18, NodeType = "Boss", ConnectedNodes = {1}, NodeHeight = 4, NodeDistance = 10 }
					};
				default:
					int nextFloor = (floor % 7) + 1; // Cycles 1-7
					return GetMapNodesForFloor(nextFloor);
			}
		}
		#endregion GetMapNodes - End

		// Moves the player to the mapNode with the given mapNodeId, if mapNodeId isn't provided, the player's location (mapNodeId) increases by 1.
		public void MovePlayer(GameState gameState, int mapNodeId = 0, bool admin = false) {
			var _mapNodes = GetMapNodesForFloor(gameState.Floor);
			MapNode? mapNodeCur = _mapNodes.Find(mn => mn.MapNodeId == gameState.PlayerLocation);
			if(mapNodeCur == null) {
				gameState.AddMessage("You are lost in the void.");
				return;
			}
			if(!mapNodeCur.ConnectedNodes.Contains(mapNodeId) && !admin) {
				return;
			}
			// Just for now to keep track of visited nodes.
			gameState.AddMapNodeIdVisited(gameState.PlayerLocation);
			// Check if the player has reached the end of the map.
			int mapNodeIdFinal = _mapNodes.Max(x => x.MapNodeId);
			gameState.IsLocationComplete = false;
			if(gameState.PlayerLocation == mapNodeIdFinal) {
				gameState.PlayerLocation = 1; // Reset the player location to the first room.
				gameState.Floor++; // Increment the floor number.
				gameState.ClearMapNodeIdsVisited();
			}
			else if(mapNodeId == 0) {
				//Move forward one location. Only works for now because map is linear.
				gameState.PlayerLocation++;
			}
			else {
				// Set player's location to the selected mapNode
				gameState.PlayerLocation = mapNodeId;
			}
			gameState.ClearMessages();//New room = blank dialog box.
			gameState.IsLocationComplete = false;
			// Zero out GameState status
			gameState.InCombat = false;
			gameState.InCampsite = false;
			gameState.InEvent = false;
			gameState.InTreasure = false;
			gameState.InShop = false;
			// Check if the player is on a campsite or event node
			var currentNode = _mapNodes.FirstOrDefault(node => node.MapNodeId == gameState.PlayerLocation);
			if(currentNode != null && currentNode.NodeType == "Campsite") {
				gameState.AddMessage("You have found a campsite. Rest here to heal 30% of your maximum HP and refill your health potions.");
				gameState.InCampsite = true;
				return;
			}
			if(currentNode != null && currentNode.NodeType == "Event") {
				gameState.EventNum = new Random().Next(1, 9); // Randomly select an event number
				switch(gameState.EventNum) {
					case 1:
						gameState.AddMessage("A prickly bush lies in your path. A few red objects shimmer from fairly deep inside.");
						gameState.AddMessage("Reach in and grab them?");
						break;
					case 2:
						gameState.AddMessage("You find a small chest.");
						gameState.AddMessage("Open it?");
						break;
					case 3:
						gameState.AddMessage("You find an odd-looking weapon on the ground.");
						gameState.AddMessage("Take it?");
						break;
					case 4:
						gameState.AddMessage("You find a piece of armor on a decaying corpse.");
						gameState.AddMessage("Take it off the corpse?");
						break;
					case 5:
						gameState.AddMessage("A large statue of a skeletal soldier stands before you.");
						gameState.AddMessage("It has a button and an engraving that states:");
						gameState.AddMessage("Press the button to test your luck.");
						gameState.AddMessage("50%: Gain Love. 30%: Suffer Hate. 17%: New Arms. 2%: Unimaginable Wealth. 1%: Certain Death.");
						gameState.AddMessage("Press the button?");
						break;
					case 6:
						gameState.AddMessage("You find a small pile of gold.");
						gameState.AddMessage("Take it?");
						break;
					case 7:
						gameState.AddMessage("You find a large red potion that looks especially tasty.");
						gameState.AddMessage("Drink it?");
						break;
					// The following events cause helth beyond the maximum. I don't know how the health bar should show that, so we're putting it aside for now.
					//case 8:
					//	gameState.AddMessage("You find a large red potion that looks... peculiar.");
					//	gameState.AddMessage("Drink it?");
					//	break;
					//case 9:
					//	gameState.AddMessage("You find an odd grey potion that looks kind of gross.");
					//	gameState.AddMessage("Drink it?");
					//	break;
					case 8:
						gameState.AddMessage("You find a shiny, magical stone on the floor.");
						gameState.AddMessage("Crush it to absorb its power?");
						break;
				}
				gameState.InEvent = true;
				return;
			}
			if(currentNode != null && currentNode.NodeType == "Treasure") {
				gameState.AddMessage("You find a chest during your travels!");
				gameState.AddMessage("Open the chest?");
				gameState.InTreasure = true;
				return;
			}
			if(currentNode != null && currentNode.NodeType == "Shop") {
				gameState.AddMessage("You find yourself at an ominous shop.");
				gameState.AddMessage("What would you like to buy?");
				gameState.InShop = true;
				// Allow player to leave immediately.
				gameState.IsLocationComplete = true;
				return;
			}
			if(currentNode.NodeType == "Enemy" || currentNode.NodeType == "Boss" || currentNode.NodeType == "Elite") {
				gameState.InCombat = true;
			}
		}

		public Map GetMap(int floor) {
			return new Map() { ListMapNodes = GetMapNodesForFloor(floor) };
		}
	}
}
