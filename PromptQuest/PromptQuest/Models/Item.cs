namespace PromptQuest.Models
{
	public class Item
	{

		public int ItemId { get; set; }
		public string name { get; set; } = "none";
		public int ATK { get; set; } = 0;
		public int DEF { get; set; } = 0;
		public string IMG { get; set; } = "/images/PromptQuestLogo.png";
	}
}
