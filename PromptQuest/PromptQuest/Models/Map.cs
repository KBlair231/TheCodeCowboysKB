namespace PromptQuest.Models {
	public class Map {
		public List<MapNode> ListMapNodes { get; set; } = new List<MapNode>();
		public List<MapEdge> ListMapEdges { get; set; } = new List<MapEdge>();
	}
	public class MapNode {
		public int MapNodeId { get; set; }
		public bool IsLocked { get; set; } = true;
		public string NodeType { get; set; } = "Enemy";
	}
	public class MapEdge {
		public int MapNodeIdStart { get; set; }
		public int MapNodeIdEnd { get; set; }
	}
}
