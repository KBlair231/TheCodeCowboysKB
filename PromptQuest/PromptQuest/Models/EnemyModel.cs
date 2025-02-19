using System.ComponentModel.DataAnnotations;

namespace PromptQuest.Models
{
	public class EnemyModel
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public int MaxHealth { get; set; }
		public int CurrentHealth { get; set; }
	}
}