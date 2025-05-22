using System.ComponentModel.DataAnnotations;

namespace PromptQuest.Models
{
	public class Enemy
	{
		/// <summary> Primary Key </summary>
		public int EnemyId {get; set;}
		public string Name { get; set; } = "";
		public string ImageUrl { get; set; } = "";
		public int MaxHealth { get; set; } = 30;
		public int CurrentHealth { get; set; } = 30;
		public int Attack { get; set; } = 8;
		public int Defense { get; set; } = 3;
		public StatusEffect StatusEffects { get; set; } = StatusEffect.None;
	}
}