using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;   // Required for bitmaps

namespace PromptQuest.Models {
	public class Player {
		/// <summary> Primary Key </summary>
		public int PlayerId { get; set; }
		[Required] // Data annotation to specify that the Name property is Required
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must not contain numbers or special characters.")]
		public string Name { get; set; } = "";
		/// <summary> A picture of the player's character stored as a base 64 string. </summary>
		public string Image { get; set; } = "";
		public int HealthPotions { get; set; }
		public int MaxHealth { get; set; }
		public int CurrentHealth { get; set; }
		public int BaseDefense { get; set; }
		[NotMapped]
		public int DefenseStat => BaseDefense + EquippedWeapon.Defense + EquippedHelm.Defense + EquippedChest.Defense + EquippedLegs.Defense + EquippedBoots.Defense + DefenseBuff;
		public int DefenseBuff { get; set; } = 0; // For temporary defense increases that last only one hit, resets each combat
		public int BaseAttack { get; set; } = 0;
		[NotMapped]
		public int AttackStat => BaseAttack + EquippedWeapon.Attack + EquippedHelm.Attack + EquippedChest.Attack + EquippedLegs.Attack + EquippedBoots.Attack;
		public int AbilityCooldown { get; set; }
		[Required]
		public string Class { get; set; } = "";
		public int Gold { get; set; } = 0;
		public Passives Passive { get; set; } = Passives.None;
		/// <summary>The Player's equipped item. Readonly</summary>
		public Item EquippedWeapon => Items.FirstOrDefault(i => i.Equipped && i.itemType == ItemType.Weapon) ?? new Item(){ itemType = ItemType.Weapon };
		public Item EquippedHelm => Items.FirstOrDefault(i => i.Equipped && i.itemType == ItemType.Helm) ?? new Item() { itemType = ItemType.Helm };
		public Item EquippedChest => Items.FirstOrDefault(i => i.Equipped && i.itemType == ItemType.Chest) ?? new Item() { itemType = ItemType.Chest };
		public Item EquippedLegs => Items.FirstOrDefault(i => i.Equipped && i.itemType == ItemType.Legs) ?? new Item() { itemType = ItemType.Legs };
		public Item EquippedBoots => Items.FirstOrDefault(i => i.Equipped && i.itemType == ItemType.Boots) ?? new Item() { itemType = ItemType.Boots };
		public List<Item> Items { get; set; } = new List<Item>();
		public StatusEffect StatusEffects { get; set; } = StatusEffect.None;

		//functions that should not be added to the database
		public bool HasPassive(Passives passive)//multiple of the same passive do not stack
		{
			if(Passive== passive) {
				return true;
			}
			if(EquippedWeapon.Passive==passive) {
				return true;
			}
			if (EquippedHelm.Passive == passive)
			{
				return true;
			}
			if (EquippedChest.Passive == passive)
			{
				return true;
			}
			if (EquippedLegs.Passive == passive)
			{
				return true;
			}
			if (EquippedBoots.Passive == passive)
			{
				return true;
			}
			return false;
		}
		public int QuickShot(int roll){
			if(!HasPassive(Passives.QuickShot)){
				return 0;
			}
			if((roll < 10 && Class.ToLower() == "archer") || roll < 5){
				return 1;
			}
			return 0;
		}
		public int HeavySmash(int roll){
			if(!HasPassive(Passives.HeavySmash)){
				return 0;
			}
			if((roll < 20 && Class.ToLower() == "warrior") || roll < 10){
				return 3;
			}
			return 0;
		}
		public int ManaBurn(int roll){
			if(!HasPassive(Passives.ManaBurn)){
				return 0;
			}
			if ((roll < 20 && Class.ToLower() == "mage") || roll < 10)
			{
				return 2+AbilityCooldown;
			}
			return 0;
		}
		public int PoisonWeapons(int roll){
			if(!HasPassive(Passives.PoisonWeapons)){
				return 0;
			}
			if ((roll < 20 && Class.ToLower() == "archer") || roll < 10)
			{
				return 1;
			}
			return 0;
		}
		public void ArcaneRecovery(int roll){
			if(!HasPassive(Passives.ArcaneRecovery)){
				return;
			}
			if (((roll < 40 && Class.ToLower() == "mage") || roll < 20)&& AbilityCooldown>0){
				AbilityCooldown -= 1;
			}
		}
	}
}
