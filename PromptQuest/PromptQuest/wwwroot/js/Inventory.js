//----------------------------------------------------------------------- Item Creation -----------------------------------------------------------------------
const defaultItem1 = new Item("Jeweled Helmet", 0, 2, "/images/PlaceholderItem1.png");
const defaultItem2 = new Item("Fiery Sword", 4, 0, "/images/PlaceholderItem2.png");
const defaultItem3 = new Item("Frozen Shield", 1, 3, "/images/PlaceholderItem3.png");
const defaultItem4 = new Item("Warded Sword", 3, 2, "/images/PlaceholderItem4.png");
const defaultItems = [defaultItem1, defaultItem2, defaultItem3, defaultItem4];
let selectedItem = new Item("None", 0, 0, "/images/PromptQuestLogo.png");
let equippedItem = new Item("None", 0, 0, "/images/PromptQuestLogo.png");
// This function will produce an item object when called
function Item(name, attack, defense, image) {
	this.name = name;
	this.attack = attack;
	this.defense = defense;
	this.image = image;
}
// This function will call the Item function to create new items and add them to the defaultItems array for testing
async function LoadItems() {
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
	let response = await fetch("/Game/GetEquippedItem");
	itemmdl = await response.json();
	
	equippedItem=new Item(itemmdl.name, itemmdl.atk, itemmdl.def, itemmdl.img)
	displayEquipped()
	document.getElementById("equip-button").removeEventListener("click",equipSelectedItem);// just in case, had issues of stacking
	document.getElementById("equip-button").addEventListener("click",equipSelectedItem);
}
function displayEquipped() {
	let equipped = document.getElementById("equipped-item");
	while (equipped.firstChild) {
		equipped.removeChild(equipped.firstChild);
	}
	let image = document.createElement("img");
	image.src = equippedItem.image;
	image.alt = equippedItem.name;
	document.getElementById("equipped-item").appendChild(image);
	image.addEventListener("click", (function () {
		selectItem(-1);
	}));
}

async function equipSelectedItem() {
	equippedItem = selectedItem;
	displayEquipped()
	await $.ajax({
		url: '/Game/EquipItem',
		type: 'POST',
		data: {
			itemName: equippedItem.name,
			itemATK: equippedItem.attack,
			itemDEF: equippedItem.defense,
			itemIMG: equippedItem.image,
		},
		success: function (response) {
			let actionResult = response;
			console.log('Player equipped (' + equippedItem.name + ')successfully:', actionResult);
			updateLocalGameState(actionResult); // Update local gameState variable with whatever the action changed.  
			updateDisplay() // Update screen to show whatever the action changed.
			addLogEntry(actionResult.message);
		},
		error: function (xhr, status, error) {
			console.error('Error equipping new item', error);
		}
	});
}

function selectItem(id) {
	if (id == -1) {
		selectedItem = equippedItem;
	}
	else {
		selectedItem = defaultItems[id];
	}
	
	// Display item stats
	document.getElementById("item-name").innerHTML = selectedItem.name;
	document.getElementById("item-attack").innerHTML = selectedItem.attack;
	document.getElementById("item-defense").innerHTML = selectedItem.defense;
	document.getElementById("item-image").src = selectedItem.image;
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
	document.getElementById("equipped-item").style.borderColor = ""
	// Highlight the inventory slot
	if (id == -1) {
		document.getElementById("equipped-item").style.borderColor = "#ffdc4a"
	}
	else {
		document.getElementById("inventory-slot-" + (id + 1)).style.borderColor = "#ffdc4a";
	}
	
}