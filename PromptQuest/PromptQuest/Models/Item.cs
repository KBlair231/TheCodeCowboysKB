using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromptQuest.Models {
	public class Item {
		public int PlayerId { get; set; }
		public int ItemId { get; set; }
		public bool Equipped { get; set; }
		public string Name { get; set; } = "none";
		public int Attack { get; set; } = 0;
		public int Defense { get; set; } = 0;
		public string ImageSrc { get; set; } = "";
		public StatusEffect StatusEffects { get; set; } = StatusEffect.None;
		public Passives Passive { get; set; } = Passives.None;
		public ItemType itemType { get; set; } = ItemType.Weapon;
	}
	public enum ItemType {
		Weapon = 0,
		Boots = 1,
		Legs = 2,
		Chest = 3,
		Helm = 4
	}
}
