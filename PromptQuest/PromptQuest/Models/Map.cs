namespace PromptQuest.Models {
	public class Map {
		public List<MapNode> ListMapNodes { get; set; } = new List<MapNode>();
		public List<int> ListMapNodeIdsVisited { get; set; } = new List<int>();
		public string BackgroundImage { get; set; } = "~/images/BackgroundCandyNightmare.png";
	}
	public class MapNode {
		public int MapNodeId { get; set; }
		public bool IsLocked { get; set; } = true;
		public string NodeType { get; set; } = "Enemy";
		public List<int> ConnectedNodes { get; set; } = new List<int>();
		public int NodeHeight { get; set; } = 0;
		public int NodeDistance { get; set; } = 0;
	}
}
