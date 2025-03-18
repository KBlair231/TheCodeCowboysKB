//----------------------------------------------------------------------- Item Creation -----------------------------------------------------------------------
const defaultItem1 = new Item("Jeweled Helmet", 0, 2, "/images/PlaceholderItem1.png");
const defaultItem2 = new Item("Fiery Sword", 4, 0, "/images/PlaceholderItem2.png");
const defaultItem3 = new Item("Frozen Shield", 1, 3, "/images/PlaceholderItem3.png");
const defaultItem4 = new Item("Warded Sword", 3, 2, "/images/PlaceholderItem4.png");
const defaultItems = [defaultItem1, defaultItem2, defaultItem3, defaultItem4];

// This function will produce an item object when called
function Item(name, attack, defense, image) {
	this.name = name;
	this.attack = attack;
	this.defense = defense;
	this.image = image;
}
// This function will call the Item function to create new items and add them to the defaultItems array for testing
function LoadItems() {
	// Clear existing items in inventory slots
	for (let i = 1; i <= 20; i++) {
		let slot = document.getElementById("inventory-slot-" + i);
		while (slot.firstChild) {
			slot.removeChild(slot.firstChild);
		}
	}
	// Add items to inventory slots
	for (let i = 0; i < defaultItems.length; i++) {
		let item = defaultItems[i];
		let image = document.createElement("img");
		image.src = item.image;
		image.alt = item.name;
		document.getElementById("inventory-slot-" + (i + 1)).appendChild(image);
		image.addEventListener("click", (function () {
			// Display item stats
			selectItem(i);
		}));
	}
}
function selectItem(id) {
	item = defaultItems[id];
	// Display item stats
	document.getElementById("item-name").innerHTML = item.name;
	document.getElementById("item-attack").innerHTML = item.attack;
	document.getElementById("item-defense").innerHTML = item.defense;
	document.getElementById("item-image").src = item.image;
	// Show the elements
	document.getElementById("item-name").style.display = "block";
	document.getElementById("item-attack").style.display = "block";
	document.getElementById("item-defense").style.display = "block";
	document.getElementById("item-image").style.display = "block";
	document.getElementById("shield-icon").style.display = "block";
	document.getElementById("sword-icon").style.display = "block";
	// Reset the border color of all slots
	for (let i = 1; i <= 20; i++) {
		document.getElementById("inventory-slot-" + i).style.borderColor = "";
	}
	// Highlight the inventory slot
	document.getElementById("inventory-slot-" + (id + 1)).style.borderColor = "#ffdc4a";
}