using System.ComponentModel.DataAnnotations.Schema;

namespace PromptQuest.Models {
	public class Item {
		public int PlayerId { get; set; }
		public int ItemId { get; set; }
		public bool Equipped { get; set; }
		public string Name { get; set; } = "none";
		public int Attack { get; set; } = 0;
		public int Defense { get; set; } = 0;
		public string ImageSrc { get; set; } = "/images/PromptQuestLogo.png";
	}
}
