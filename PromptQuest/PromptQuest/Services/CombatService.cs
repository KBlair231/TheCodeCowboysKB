using System.ComponentModel;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PromptQuest.Models;

namespace PromptQuest.Services {

	public interface ICombatService {
		void StartCombat(GameState gameState);
		void PlayerAttack(GameState gameState, int attackMult = 1, bool decrementAbility = true);
		void PlayerUseHealthPotion(GameState gameState);
		void PlayerRest(GameState gameState);
		void PlayerSkipRest(GameState gameState);
		void PlayerAccept(GameState gameState);
		void PlayerDeny(GameState gameState);
		void PlayerAbility(GameState gameState);
		void PlayerOpenTreasure(GameState gameState);
		void PlayerSkipTreasure(GameState gameState);
		void PlayerPurchaseItem(GameState gameState, int actionValue);
		void RespawnPlayer(GameState gameState);
		void EnemyAttack(GameState gameState);
		void EnemyDied(GameState gameState);
		Enemy GetEnemy(GameState gameState);
	}

	public class CombatService : ICombatService {
		/// <summary>Initiates combat between the player and an enemy and updates the game state. </summary>
		public void StartCombat(GameState gameState) {
			gameState.InCombat = true;
			gameState.IsPlayersTurn = true; // Player attacks first
			gameState.Player.AbilityCooldown = 0; //reset player ability, may be removed down the line
			gameState.Player.DefenseBuff = 0; //reset player defense buff
			gameState.Enemy = GetEnemy(gameState);
			// Give the enemy a 5% chance to attack first
			Random random = new Random();
			int chance = random.Next(1, 101); // Generate a number between 1 and 100
			if(chance <= 5) {
				gameState.IsPlayersTurn = false; // Enemy attacks first
				gameState.AddMessage($"You were ambushed by the {gameState.Enemy.Name}!");
			}
			else {
				gameState.IsPlayersTurn = true; // Player attacks first
				gameState.AddMessage($"You have encountered the {gameState.Enemy.Name}!"); // Let the user know that combat started.
			}
		}

		#region Player Action Methods

		/// <summary> Calculates the damage that the player does to the enemy, updates the game state, then returns a message.</summary>
		public void PlayerAttack(GameState gameState, int attackMult = 1, bool decrementAbility = true) {
			Random random = new Random();//for any random rolls
			int attackBuff = 0;//for any attack buffs
												 // Get the player's equipped item
												 //beggining of attack portion
												 //Checking for the Quick Shot passive
			int numberOfAttacks = 1;
			numberOfAttacks += gameState.Player.QuickShot(random.Next(0, 100));
			for(int i = 0; i < numberOfAttacks; i++) {
				//beggining of damage calc
				// Checking for Heavy Smash passive
				attackBuff = gameState.Player.HeavySmash(random.Next(0, 100));
				// Make damage into a range of 0.8-1.30x the attack stat picking a random number between these values of the damage
				int lowerBound = (int)Math.Floor((gameState.Player.AttackStat + attackBuff) * 0.8);
				int upperBound = (int)Math.Ceiling((gameState.Player.AttackStat + attackBuff) * 1.3);
				// Generate a random number between the lower and upper bounds
				int damage = random.Next(lowerBound, upperBound + 1) * attackMult - gameState.Enemy.Defense;
				// If attack is less than one make it one.
				if(damage < 1) {
					damage = 1;
				}
				//Checking for Mana Burn passive
				damage += gameState.Player.ManaBurn(random.Next(0, 100));
				gameState.Enemy.Defense -= gameState.Player.PoisonWeapons(random.Next(0, 100));
				// Update enemy health.
				gameState.Enemy.CurrentHealth -= damage;
				//end of damage calc
				//decrement ability cooldown if ability was not used
				if(decrementAbility && gameState.Player.AbilityCooldown > 0) {
					gameState.Player.AbilityCooldown -= 1;
					gameState.Player.ArcaneRecovery(random.Next(0, 100));
				}
				// Get the player's equipped item
				Item item = gameState.Player.EquippedWeapon;
				if(item.StatusEffects != StatusEffect.None) {
					int statusEffectChance = random.Next(0, 5); // 25% chance to apply status effect
					if(statusEffectChance == 1) {
						if(!gameState.Enemy.StatusEffects.HasFlag(item.StatusEffects)) {
							gameState.AddMessage($"The {gameState.Enemy.Name} is now affected by {item.StatusEffects.ToString()}!");
							gameState.Enemy.StatusEffects = item.StatusEffects;
						}
					}
				}
			}
			// Check if enemy died.
			if(gameState.Enemy.CurrentHealth <= 0) {
				EnemyDied(gameState);
			}
			gameState.IsPlayersTurn = false;
		}

		/// <summary>activates the player's ability</summary>
		public void PlayerAbility(GameState gameState) {
			switch(gameState.Player.Class.ToLower()) {
				case "warrior"://attack for double power, uses the attack function
					gameState.AddMessage($"You used your warrior ability! You power up and perform a strong attack!");
					PlayerAttack(gameState, 2, false);
					gameState.Player.AbilityCooldown = 3;
					break;
				case "mage"://gain +6 defense for the next attack
					gameState.AddMessage($"You used your mage ability! You cast a shield spell to gain +6 defense against the next attack!");
					gameState.Player.DefenseBuff += 6;
					gameState.Player.AbilityCooldown = 4;
					break;
				case "archer"://perform two attacks
					gameState.AddMessage($"You used your archer ability! You shoot two arrows at the enemy!");
					PlayerAttack(gameState, 1, false);
					PlayerAttack(gameState, 1, false);
					gameState.Player.AbilityCooldown = 3;
					break;
				default:
					gameState.AddMessage("Your class has no ability.");
					break;
			}
		}

		/// <summary>Calculates the amount healed by a Health Potion, updates the game state, then returns a message.</summary>
		public void PlayerUseHealthPotion(GameState gameState) {
			// If player has no potions, don't let them heal.
			if(gameState.Player.HealthPotions <= 0) {
				gameState.AddMessage("You have no Health Potions!");
				return;
			}
			// If player is already at max health, don't let them heal.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.AddMessage("You are already at max health!");
				return;
			}
			// Update player health and number of potions.
			gameState.Player.HealthPotions -= 1;
			gameState.Player.CurrentHealth += (int)Math.Ceiling(gameState.Player.MaxHealth * 0.2);
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				return;
			}
			// Healing does not end the player's turn.
		}

		/// <summary>Calculates the amount healed by Resting, updates the game state, then returns a message.</summary>
		public void PlayerRest(GameState gameState) {
			string message = "You feel well rested and are ready to continue on your journey.";
			// If player is already at max health, don't let them rest.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				//gameState.AddMessage("You are already at max health!"); Not sure if this is necessary or not with the new health change indicators
				gameState.IsLocationComplete = true;
				gameState.AddMessage(message);
				return;
			}
			// Update player health (+30% of max health)
			gameState.Player.CurrentHealth += (int)Math.Ceiling(gameState.Player.MaxHealth * 0.35);
			// If the potion put the player's health above maximum, set it to maximum.
			if(gameState.Player.CurrentHealth >= gameState.Player.MaxHealth) {
				gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
				gameState.IsLocationComplete = true;
				gameState.AddMessage(message);
				return;
			}
			gameState.AddMessage(message);
			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Doesn't rest, updates the game state, then returns a message.</summary>
		public void PlayerSkipRest(GameState gameState) {
			gameState.AddMessage("You have guts...");
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player accepts event, updates the game state, then returns a message.</summary>
		public void PlayerAccept(GameState gameState) {
			switch(gameState.EventNum) {
				case 1:
					// Player gains 3 potions
					gameState.Player.HealthPotions += 3;
					// Damage the player
					gameState.Player.CurrentHealth -= 7;
					if(gameState.Player.CurrentHealth <= 0) {
						gameState.Player.CurrentHealth = 1;
					}
					// Tell player what happened
					gameState.AddMessage("Your greed earned you 3 more potions, but at what cost?");
					break;
				case 2:
					int randItem = new Random().Next(1, 4); // Randomly give an item from the list below
					Item treasureItem = new Item();
					if(randItem == 1) {
						// If 1, give them a treasure specific item.
						treasureItem.Name = "Fiery Pajama Pants";
						treasureItem.Attack = 3;
						treasureItem.Defense = 3;
						treasureItem.ImageSrc = "/images/FieryPajamaPants.png";
						treasureItem.itemType = ItemType.Legs;
						treasureItem.StatusEffects = StatusEffect.Burning;
						gameState.Player.Items.Add(treasureItem);
					}
					else if(randItem == 2) {
						// If 2, give them a treasure specific item.
						treasureItem.Name = "Inflatable Hammer";
						treasureItem.Attack = 5;
						treasureItem.Defense = -2;
						treasureItem.ImageSrc = "/images/InflatableHammer.png";
						treasureItem.itemType = ItemType.Weapon;
						treasureItem.Passive = Passives.HeavySmash; // Give the player the Heavy Smash passive
						gameState.Player.Items.Add(treasureItem);
					}
					else {
						// Else, give them a treasure specific item.
						treasureItem.Name = "Rainbow Treads";
						treasureItem.Attack = -1;
						treasureItem.Defense = 8;
						treasureItem.ImageSrc = "/images/RainbowTreads.png";
						treasureItem.itemType = ItemType.Boots;
						treasureItem.Passive = Passives.QuickShot; // Give the player the Quick Shot passive
						gameState.Player.Items.Add(treasureItem);
					}
					// Tell player what happened
					gameState.AddMessage("You gained: " + treasureItem.Name + "!");
					break;
				case 3:
					Item bloodThornAxe = new Item();
					bloodThornAxe.Name = "BloodThorn Axe";
					bloodThornAxe.Attack = 9;
					bloodThornAxe.Defense = 8;
					bloodThornAxe.ImageSrc = "/images/BloodThornAxe.png";
					bloodThornAxe.itemType = ItemType.Weapon;
					bloodThornAxe.StatusEffects = StatusEffect.Bleeding;
					gameState.Player.Items.Add(bloodThornAxe);
					gameState.AddMessage("You pick up the BloodThorn Axe. It's thorns poke you.");
					gameState.Player.CurrentHealth -= 25; // Damage the player
					if(gameState.Player.CurrentHealth <= 0) {
						gameState.AddMessage("By some miracle, you live...");
						gameState.Player.CurrentHealth = 1; // Prevent death
					}
					break;
				case 4:
					Item poisonChestPlate = new Item();
					poisonChestPlate.Name = "Plate of the Poisoned";
					poisonChestPlate.Attack = 10;
					poisonChestPlate.Defense = 7;
					poisonChestPlate.ImageSrc = "/images/PoisonChestplate.png";
					poisonChestPlate.itemType = ItemType.Chest;
					poisonChestPlate.Passive = Passives.PoisonWeapons; // Give the player the Poison Weapons passive
					gameState.Player.Items.Add(poisonChestPlate);
					gameState.AddMessage("You pick up the Plate of the Poisoned. It's toxins cause you to feel incredibly sick.");
					gameState.Player.CurrentHealth -= 40; // Damage the player
					if(gameState.Player.CurrentHealth <= 0) {
						gameState.AddMessage("By some miracle, you live...");
						gameState.Player.CurrentHealth = 1; // Prevent death
					}
					break;
				case 5:
					Random random = new Random();
					int chance = random.Next(1, 101); // Generate a number between 1 and 100
					if(chance <= 50) {
						// 50%: Gain Love
						gameState.AddMessage("You feel a warm sensation. You have gained Love and heal for 10HP!");
						gameState.Player.CurrentHealth += 10; // Heal the player
						if(gameState.Player.CurrentHealth > gameState.Player.MaxHealth) {
							gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
						}
					}
					else if(chance <= 80) {
						// 30%: Suffer Hate
						gameState.AddMessage("A dark shadow looms over you. You suffer Hate and take 5 damage!");
						gameState.Player.CurrentHealth -= 5; // Damage the player
						if(gameState.Player.CurrentHealth <= 0) {
							gameState.AddMessage("By some miracle, you live...");
							gameState.Player.CurrentHealth = 1; // Prevent death
						}
					}
					else if(chance <= 97) {
						// 17%: New Arms
						gameState.AddMessage("You feel a strange transformation. You have gained a new armament!");
						Item newArms = new Item {
							Name = "Mystic Arms",
							Attack = 10,
							Defense = -5,
							ImageSrc = "/images/MysticArms.png",
							itemType = ItemType.Weapon
						};
						gameState.Player.Items.Add(newArms);
					}
					else if(chance <= 99) {
						// 2%: Unimaginable Wealth
						gameState.AddMessage("Gold rains from the sky! You have gained Unimaginable Wealth!");
						gameState.Player.Gold += 100; // Give 100 gold
					}
					else {
						// 1%: Certain Death
						gameState.AddMessage("A chilling wind passes through you. You have met Certain Death.");
						gameState.Player.CurrentHealth = 0; // Set health to 0 to indicate death
					}
					break;
				case 6:
					gameState.AddMessage("You scoop up the small pile of gold! You gained 20 gold!");
					gameState.Player.Gold += 20; // Grant gold
					break;
				case 7:
					gameState.AddMessage("You drink the red potion and feel more refreshed than ever! Your Maximum HP has increased by 5 points.");
					gameState.Player.MaxHealth += 5; // Increase max health by 5
					gameState.Player.CurrentHealth = gameState.Player.MaxHealth; // Heal the player
					break;
				//case 8:
				//	gameState.AddMessage("You drink the red potion and feel... a little disgusted. You lost 1 Maximum HP, but gained an uncapped 25 HP!");
				//	gameState.Player.MaxHealth -= 1; // Decrease max health by 1
				//	gameState.Player.CurrentHealth += 20; // Heal the player, even if it goes over max
				//	break;
				//case 9:
				//	gameState.AddMessage("You drink the grey potion and feel like vomiting. Your Maximum HP is now 1, but you gained an uncapped 75 HP!");
				//	gameState.Player.MaxHealth = 1; // Decrease max health to 1
				//	gameState.Player.CurrentHealth += 75; // Heal the player, even if it goes over max
				//	break;
				case 8:
					gameState.AddMessage("You feel stronger! Your base attack has increased by 2 points.");
					gameState.Player.BaseAttack += 2; // Increase attack by 2 permanently
					break;
				case 9:
					gameState.AddMessage("You feel like nothing can scratch you! Your base defense has increased by 2 points.");
					gameState.Player.BaseDefense += 2; // Increase defense by 2 permanently
					break;
				case 10:
					if(gameState.Player.AttackStat == gameState.Player.DefenseStat) {
						gameState.AddMessage("The scale remains perfectly centered. You are rewarded for your dedication to a balanced lifestyle.");
						gameState.AddMessage("Your base attack and defense increase by 3 points!");
						gameState.Player.BaseAttack += 3; // Increase attack by 3 permanently
						gameState.Player.BaseDefense += 3; // Increase defense by 3 permanently
					}
					else if(gameState.Player.AttackStat > gameState.Player.DefenseStat) {
						gameState.AddMessage("The scale is weighed down by the bag with the sword symbol. It is clear that you value power over cowering from your foes behind a shield.");
						gameState.AddMessage("Your base attack increases by 3 and your base defense decreases by 2!");
						gameState.Player.BaseAttack += 3; // Increase attack by 3 permanently
						gameState.Player.BaseDefense -= 2; // Decrease defense by 2 permanently
					}
					else {
						gameState.AddMessage("The scale is weighed down by the bag with the shield symbol. It is clear that you value a strategical approach over blind rage.");
						gameState.AddMessage("Your base defense increases by 3 and your base attack decreases by 2!");
						gameState.Player.BaseDefense += 3; // Increase defense by 3 permanently
						gameState.Player.BaseAttack -= 2; // Decrease attack by 2 permanently
					}
					break;
			}

			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player denies event, updates the game state, then returns a message.</summary>
		public void PlayerDeny(GameState gameState) {
			gameState.AddMessage("You figure it's not worth the risk.");
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player opens treasure chest, updates the game state, then returns a message.</summary>
		public void PlayerOpenTreasure(GameState gameState) {
			int randPotions = new Random().Next(1, 3); // Randomly give 1 to 2 potions
			gameState.Player.HealthPotions += randPotions;
			int randItem = 0; // Initialize randItem
			switch(gameState.Floor) {
				case 1:
					randItem = new Random().Next(1, 4); // Randomly give an item from floor 1
					break;
				case 2:
					randItem = new Random().Next(4, 7); // Randomly give an item from floor 2
					break;
				case 3:
					randItem = new Random().Next(7, 10); // Randomly give an item from floor 3
					break;
				case 4:
					randItem = new Random().Next(10, 13); // Randomly give an item from floor 4
					break;
				default:
					randItem = new Random().Next(1, 13); // Default case, randomly give an item from any floor
					break;
			}
			Item treasureItem = new Item();
			treasureItem = GetChestItem(randItem);
			gameState.Player.Items.Add(treasureItem);
			// Tell player what happened
			gameState.AddMessage("You gained: " + randPotions + " Health Potion(s) and " + treasureItem.Name + "!");
			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Gets a chest item and returns it.</summary>
		public Item GetChestItem(int itemValue) {
			Item treasureItem = new Item();
			switch(itemValue) {
				case 1:
					treasureItem.Name = "Elf Hat";
					treasureItem.Attack = 0;
					treasureItem.Defense = 4;
					treasureItem.ImageSrc = "/images/ElfHat.png";
					treasureItem.itemType = ItemType.Helm;
					break;
				case 2:
					treasureItem.Name = "Crystal Leggings";
					treasureItem.Attack = 3;
					treasureItem.Defense = 2;
					treasureItem.ImageSrc = "/images/CrystalLeggings.png";
					treasureItem.itemType = ItemType.Legs;
					treasureItem.Passive = Passives.SpikedBulwark; // Give the player the Spiked Bulwark passive
					break;
				case 3:
					treasureItem.Name = "Gilded Boots";
					treasureItem.Attack = 1;
					treasureItem.Defense = 3;
					treasureItem.ImageSrc = "/images/GildedBoots.png";
					treasureItem.itemType = ItemType.Boots;
					break;
				case 4:
					treasureItem.Name = "Banana Crown";
					treasureItem.Attack = 2;
					treasureItem.Defense = 10;
					treasureItem.ImageSrc = "/images/BananaCrown.png";
					treasureItem.itemType = ItemType.Helm;
					treasureItem.Passive = Passives.ArcaneRecovery; // Give the player the Arcane Recovery passive
					break;
				case 5:
					treasureItem.Name = "Gothic T-Shirt";
					treasureItem.Attack = 6;
					treasureItem.Defense = 1;
					treasureItem.ImageSrc = "/images/GothicTShirt.png";
					treasureItem.itemType = ItemType.Chest;
					break;
				case 6:
					treasureItem.Name = "Spooky Ladle";
					treasureItem.Attack = 6;
					treasureItem.Defense = 2;
					treasureItem.ImageSrc = "/images/SpookyLadle.png";
					treasureItem.itemType = ItemType.Weapon;
					break;
				case 7:
					treasureItem.Name = "Space Blaster";
					treasureItem.Attack = 9;
					treasureItem.Defense = 5;
					treasureItem.ImageSrc = "/images/SpaceBlaster.png";
					treasureItem.itemType = ItemType.Weapon;
					treasureItem.Passive = Passives.ManaBurn; // Give the player the Mana Burn passive
					break;
				case 8:
					treasureItem.Name = "Cosmic Shield";
					treasureItem.Attack = 2;
					treasureItem.Defense = 10;
					treasureItem.ImageSrc = "/images/CosmicShield.png";
					treasureItem.itemType = ItemType.Weapon;
					break;
				case 9:
					treasureItem.Name = "Space Tracers";
					treasureItem.Attack = 2;
					treasureItem.Defense = 7;
					treasureItem.ImageSrc = "/images/SpaceTracers.png";
					treasureItem.itemType = ItemType.Boots;
					break;
				case 10:
					treasureItem.Name = "Candy Cane Dagger";
					treasureItem.Attack = 10;
					treasureItem.Defense = 3;
					treasureItem.ImageSrc = "/images/CandyCaneDagger.png";
					treasureItem.itemType = ItemType.Weapon;
					treasureItem.Passive = Passives.QuickShot; // Give the player the Quick Shot passive
					break;
				case 11:
					treasureItem.Name = "Gingerbread Chestplate";
					treasureItem.Attack = 0;
					treasureItem.Defense = 9;
					treasureItem.ImageSrc = "/images/GingerbreadChestplate.png";
					treasureItem.itemType = ItemType.Chest;
					break;
				case 12:
					treasureItem.Name = "Candy Pants";
					treasureItem.Attack = 1;
					treasureItem.Defense = 8;
					treasureItem.ImageSrc = "/images/CandyPants.png";
					treasureItem.itemType = ItemType.Legs;
					break;
				default:
					treasureItem.Name = "Item.Name not found";
					treasureItem.Attack = 404;
					treasureItem.Defense = -404;
					treasureItem.ImageSrc = "/images/ItemNotFound.png";
					treasureItem.itemType = ItemType.Weapon;
					treasureItem.StatusEffects = StatusEffect.Burning;
					treasureItem.Passive = Passives.PoisonWeapons; // Give the player the Poison Weapons passive
					break;
			}
			return treasureItem;
		}

		/// <summary>Player skips treasure chest, updates the game state, then returns a message.</summary>
		public void PlayerSkipTreasure(GameState gameState) {
			gameState.AddMessage("Better safe than sorry...");
			gameState.IsLocationComplete = true;
		}

		/// <summary>Player purchases an item, updates the game state, then returns a message.</summary>
		public void PlayerPurchaseItem(GameState gameState, int actionValue) {
			Item shopItem = new Item();
			int itemPrice = 0;
			if(actionValue == 1) {
				// Set item price
				itemPrice = 25;
				if(gameState.Player.Gold < itemPrice) {
					gameState.AddMessage("You don't have enough gold!");
					return;
				}
				gameState.Player.Gold -= itemPrice; // Deduct gold
				shopItem = GetShopItem(actionValue);
				gameState.Player.Items.Add(shopItem);
			}
			else if(actionValue == 2) {
				// Set item price
				itemPrice = 65;
				if(gameState.Player.Gold < itemPrice) {
					gameState.AddMessage("You don't have enough gold!");
					return;
				}
				gameState.Player.Gold -= itemPrice; // Deduct gold
				shopItem = GetShopItem(actionValue);
				gameState.Player.Items.Add(shopItem);
			}
			else if(actionValue == 3) {
				// Set item price
				itemPrice = 75;
				if(gameState.Player.Gold < itemPrice) {
					gameState.AddMessage("You don't have enough gold!");
					return;
				}
				gameState.Player.Gold -= itemPrice; // Deduct gold
				shopItem = GetShopItem(actionValue);
				gameState.Player.Items.Add(shopItem);
			}
			else {
				// Set item price
				itemPrice = 30;
				if(gameState.Player.Gold < itemPrice) {
					gameState.AddMessage("You don't have enough gold!");
					return;
				}
				gameState.Player.Gold -= itemPrice; // Deduct gold
				shopItem.Name = "Health Potion"; // Name the item just so it can be messaged
				gameState.Player.HealthPotions += 1; // Give the player a health potion
			}
			// Tell player what happened
			gameState.AddMessage("You bought: " + shopItem.Name + " for " + itemPrice + " gold!");
			gameState.AddMessage("You have " + gameState.Player.Gold + " gold left.");
			// Ensure player can leave
			gameState.IsLocationComplete = true;
		}

		/// <summary>Gets a shop item and returns it.</summary>
		public Item GetShopItem(int actionValue) {
			Item shopItem = new Item();
			if(actionValue == 1) {
				shopItem.Name = "Darksteel Leggings";
				shopItem.Attack = -1;
				shopItem.Defense = 6;
				shopItem.ImageSrc = "/images/DarkSteelLeggings.png";
				shopItem.itemType = ItemType.Legs;
			}
			else if(actionValue == 2) {
				shopItem.Name = "Radiant Glass Helm";
				shopItem.Attack = 3;
				shopItem.Defense = 9;
				shopItem.ImageSrc = "/images/RadiantGlassHelm.png";
				shopItem.itemType = ItemType.Helm;
			}
			else if(actionValue == 3) {
				shopItem.Name = "The Pencil Blade";
				shopItem.Attack = 10;
				shopItem.Defense = -3;
				shopItem.ImageSrc = "/images/ThePencilBlade.png";
				shopItem.itemType = ItemType.Weapon;
			}
			else { // Shouldn't occur, but just in case.
				shopItem.Name = "Elf Hat";
				shopItem.Attack = 0;
				shopItem.Defense = 12;
				shopItem.ImageSrc = "/images/ElfHat.png";
				shopItem.itemType = ItemType.Helm;
			}
			return shopItem;
		}

		public void RespawnPlayer(GameState gameState) {
			// Reset player health and potions back to max
			gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
			// Reset player's potions to 2
			gameState.Player.HealthPotions = 2;
			// Clear messages from previous life.
			gameState.ClearMessages();
			// Restart the player at the first location.
			gameState.PlayerLocation = 1;
			// Start a new fight.
			StartCombat(gameState);
		}

		#endregion  Player Action Methods - End

		#region Enemy Action Methods

		/// <summary>Calculates the damage that the enemy does to the player, updates the game state, then returns a message.</summary>
		public void EnemyAttack(GameState gameState) {
			// Make the damage a range of 0.8-1.30x the attack stat picking a random number between these values of the damage
			int lowerBound = (int)Math.Floor(gameState.Enemy.Attack * 0.8);
			int upperBound = (int)Math.Ceiling(gameState.Enemy.Attack * 1.3);
			// Generate a random number between the lower and upper bounds and calculate damage
			Random random = new Random();
			int damage = random.Next(lowerBound, upperBound + 1) - gameState.Player.DefenseStat - gameState.Player.DefenseBuff; ;
			if(gameState.Player.DefenseBuff > 0) {
				gameState.AddMessage("Your shield blocked incoming damage");
			}
			gameState.Player.DefenseBuff = 0;//reset the defense buff after blocking one hit
																			 // If attack is less than one make it one.
			if(damage < 1)
				damage = 1;
			// If the player has Spiked Bulwark, deal damage to the enemy.
			if(gameState.Player.HasPassive(Passives.SpikedBulwark)) {
				int returnDamage = 1;
				if(gameState.Player.Class.ToLower() == "warrior") {
					returnDamage = 2;
				}
				gameState.Enemy.CurrentHealth -= returnDamage;
				gameState.AddMessage($"Your Spiked Bulwark passive dealt {returnDamage} damage back to the {gameState.Enemy.Name}!");
			}
			// Update player health.
			gameState.Player.CurrentHealth -= damage;
			// Check if player died.
			if(gameState.Player.CurrentHealth < 1) {
				gameState.IsPlayersTurn = true; // Zero this field out because combat is over.
				gameState.AddMessage("You have been defeated.");
				gameState.ClearMapNodeIdsVisited(); // Clear visited map nodes since the player died.
			}
			// Apply status effect damage to the enemy
			if(gameState.Enemy.StatusEffects.HasFlag(StatusEffect.Burning)) {
				gameState.Enemy.CurrentHealth -= 2;
				gameState.Enemy.StatusEffects &= ~StatusEffect.Burning; // Remove the burning effect
				gameState.AddMessage($"The {gameState.Enemy.Name} took damage from burning and the flame was extinguished.");
			}
			if(gameState.Enemy.StatusEffects.HasFlag(StatusEffect.Bleeding)) {
				gameState.Enemy.CurrentHealth -= 1;
				gameState.AddMessage($"The {gameState.Enemy.Name} took damage from their untreated wound.");
			}
			// Player didn't die, so now it is their turn.
			gameState.IsPlayersTurn = true;
			// Check if enemy died.
			if(gameState.Enemy.CurrentHealth <= 0) {
				EnemyDied(gameState);
			}
		}

		public void EnemyDied(GameState gameState) {
			gameState.IsPlayersTurn = true; // Zero this field out because combat is over.
			gameState.IsLocationComplete = true; // Player has completed the current area.
			gameState.AddMessage($"You have defeated the {gameState.Enemy.Name}! Check your map to see where you're going next.");
			// Generate a random amount of gold between 6 and 15
			Random random = new Random();
			int gold = random.Next(6, 16);
			gameState.Player.Gold += gold;
			gameState.AddMessage($"You gained {gold} gold!");
			if(gameState.PlayerLocation == 18) {
				Item bossItem = GetBossItem(gameState); // Get the boss item.
				gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {bossItem.Name}!");
				gameState.Player.Items.Add(bossItem);
				return;
			}
			if(gameState.PlayerLocation == 11) {
				Item eliteItem = GetEliteItem(gameState); // Get the elite's item.
				gameState.AddMessage($"You picked up the {gameState.Enemy.Name}'s {eliteItem.Name}!");
				gameState.Player.Items.Add(eliteItem);
				return;
			}
		}

		#endregion Enemy Action Methods - end

		#region Helper Methods

		/// <summary>Generatees an Enemy, updates the game state, then returns the Enemy.</summary>
		public Enemy GetEnemy(GameState gameState) {
			// Read the enemies from a file and deserialize them into a list of enemies.
			string filePath = "wwwroot\\data\\Enemies.txt";
			if (!File.Exists(filePath)) {
				Console.WriteLine("File not found.");
				return new Enemy {
					Name = "Unknown Enemy",
					MaxHealth = 10,
					CurrentHealth = 10,
					Attack = 1,
					Defense = 1
				};
			}
			// Read the file and deserialize the JSON into a list of enemies.
			string json = File.ReadAllText(filePath);
			// Parse JSON into JObject
			JObject gameData = JObject.Parse(json);
			// Fixing the type mismatch by converting the integer 'Floor' to a string using the ToString() method.
			string floor = gameState.Floor.ToString();
			// Loop the 4 floors with a modulo operation to get the correct floor.
			if (gameState.Floor > 4) {
				floor = (gameState.Floor % 4).ToString(); // Floors are 1-4, so we use modulo to wrap around.
				if (floor == "0") {
					floor = "4"; // If modulo is 0, it means it's the 4th floor.
				}
			} 
			string enemyType = "Enemies"; // Default enemy type
			if (gameState.PlayerLocation == 11) {
				// If the player is in the elite room, return an elite 
				enemyType = "Elite";
			} else if (gameState.PlayerLocation == 18) {
				// If the player is in the boss room, return a boss
				enemyType = "Boss";
			}
			var randomEnemy = GetRandomEnemy(gameData, floor, enemyType);
			Console.WriteLine(JsonConvert.SerializeObject(randomEnemy, Formatting.Indented));
			int multiplier = gameState.Floor - 1;
			return randomEnemy != null ? new Enemy {
				Name = randomEnemy["name"]?.ToString(),
				ImageUrl = randomEnemy["imageUrl"]?.ToString(),
				MaxHealth = (int)randomEnemy["maxHealth"] + 5 * multiplier,
				CurrentHealth = (int)randomEnemy["currentHealth"] + 5 * multiplier,
				Attack = (int)randomEnemy["attack"] + 3 * multiplier,
				Defense = (int)randomEnemy["defense"] + 1 * multiplier,
				StatusEffects = StatusEffect.None // Default, can be set later if needed
			} : new Enemy {
				Name = "Unknown Enemy",
				MaxHealth = 10,
				CurrentHealth = 10,
				Attack = 1,
				Defense = 1
			};
		}
		static JObject GetRandomEnemy(JObject jsonData, string floor, string entityType) {
			if (jsonData[floor]?[entityType] is JArray entityList && entityList.Count > 0) {
				Random random = new Random();
				return (JObject)entityList[random.Next(entityList.Count)];
			}
			return null; // Handles cases where floor or type doesn't exist
		}
		#region Boss and Elite Item Generation
		public Item GetBossItem(GameState gameState) {
			// Generate a boss item for the player
			Item bossItem = new Item();
			if(gameState.Floor == 1) {
				// If the player is on the first floor, give them a boss specific item.
				bossItem.Name = "Dark Staff";
				bossItem.Attack = 5;
				bossItem.Defense = 4;
				bossItem.ImageSrc = "/images/DarkStaff.png";
			}
			else if(gameState.Floor == 2) {
				// If the player is on the second floor, give them a boss specific item.
				bossItem.Name = "Dark Elvish Sword";
				bossItem.Attack = 8;
				bossItem.Defense = 3;
				bossItem.ImageSrc = "/images/DarkElvenSword.png";
				bossItem.StatusEffects = StatusEffect.Bleeding;
			}
			else {
				// If the player is on the third floor, give them a boss specific item.
				bossItem.Name = "Shadow Spear";
				bossItem.Attack = 12;
				bossItem.Defense = 6;
				bossItem.ImageSrc = "/images/DarkSpear.png";
				bossItem.StatusEffects = StatusEffect.Bleeding;
			}
			return bossItem;
		}
		public Item GetEliteItem(GameState gameState) {
			// Generate a boss item for the player
			Item eliteItem = new Item();
			if(gameState.Floor == 1) {
				// If the player is on the first floor, give them an elite specific item.
				eliteItem.Name = "Berserker Axe";
				eliteItem.Attack = 5;
				eliteItem.Defense = 1;
				eliteItem.ImageSrc = "/images/BerserkerAxe.png";
				eliteItem.StatusEffects = StatusEffect.Bleeding;
			}
			else if(gameState.Floor == 2) {
				// If the player is on the second floor, give them an elite specific item.
				eliteItem.Name = "Spiked Leaf";
				eliteItem.Attack = 6;
				eliteItem.Defense = 2;
				eliteItem.ImageSrc = "/images/SpikedLeaf.png";
			}
			else {

				// If the player is on the third floor, give them an elite specific item.
				eliteItem.Name = "Demon Cleaver";
				eliteItem.Attack = 10;
				eliteItem.Defense = 5;
				eliteItem.ImageSrc = "/images/DemonCleaver.png";
				eliteItem.StatusEffects = StatusEffect.Bleeding;
			}
			return eliteItem;
		}
		#endregion Boss and Elite Item Generation - End
		#region Elite and Boss Enemy Generation
		/// <summary> Generates an Elite Enemy, updates the game state, then returns the Elite Enemy.</summary>
		public Enemy GetElite(GameState gameState) {
			if(gameState.Floor == 1) {
				Enemy elite = new Enemy();
				elite.Name = "Spectral Orc Berserker";
				elite.ImageUrl = "/images/SpectralOrc.png";
				elite.MaxHealth = 35;
				elite.CurrentHealth = 35;
				elite.Attack = 12;
				elite.Defense = 5;
				return elite;
			}
			else if(gameState.Floor == 2) {
				Enemy elite = new Enemy();
				elite.Name = "Corrupted Plant";
				elite.ImageUrl = "/images/EvilPlant.png";
				elite.MaxHealth = 40;
				elite.CurrentHealth = 40;
				elite.Attack = 15;
				elite.Defense = 8;
				return elite;
			}
			else {
				Enemy elite = new Enemy();
				elite.Name = "Demon Knight";
				elite.ImageUrl = "/images/DemonKnight.png";
				elite.MaxHealth = 50;
				elite.CurrentHealth = 50;
				elite.Attack = 18;
				elite.Defense = 13;
				return elite;
			}
		}
		/// <summary> Generates an Enemy, updates the game state, then returns the Enemy.</summary>
		public Enemy GetBoss(GameState gameState) {
			if(gameState.Floor == 1) {
				Enemy boss = new Enemy();
				boss.Name = "Dark Orc Warlock";
				boss.ImageUrl = "/images/OrcWarlock.png";
				boss.MaxHealth = 40;
				boss.CurrentHealth = 40;
				boss.Attack = 12;
				boss.Defense = 3;
				return boss;
			}
			else if(gameState.Floor == 2) {
				Enemy boss = new Enemy();
				boss.Name = "Dark Elven King";
				boss.ImageUrl = "/images/DarkElfWarrior.png";
				boss.MaxHealth = 55;
				boss.CurrentHealth = 55;
				boss.Attack = 15;
				boss.Defense = 7;
				return boss;
			}
			else {
				Enemy boss = new Enemy();
				boss.Name = "Eldritch Horror";
				boss.ImageUrl = "/images/EldritchHorror.png";
				boss.MaxHealth = 70;
				boss.CurrentHealth = 70;
				boss.Attack = 20;
				boss.Defense = 14;
				return boss;
			}
	}
	#endregion Boss and Elite Enemy Generation - End
	#endregion Helper Methods - End
}
}