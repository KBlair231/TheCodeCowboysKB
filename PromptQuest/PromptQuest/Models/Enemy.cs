using System.ComponentModel.DataAnnotations;

namespace PromptQuest.Models
{
	public class Enemy
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public int MaxHealth { get; set; }
		public int CurrentHealth { get; set; }
		public int Attack { get; set; }
		public int Defense { get; set; } = 1;

	}
}