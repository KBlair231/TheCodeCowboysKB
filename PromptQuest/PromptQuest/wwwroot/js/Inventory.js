// "Global" Variables for the selected and equipped items
let selectedItemIndex = -1;

async function loadItems() {
	await loadGame(); //Refresh the local gamestate and all displays
	const items = gameState.player.items;
	console.log(items);
	// Clear existing inventory slots
	for (let i = 1; i <= 20; i++) {
		const slot = document.getElementById("inventory-slot-" + i);
		while (slot.firstChild) {
			slot.removeChild(slot.firstChild);
		}
	}
	for (let i = 0; i < items.length; i += 1) {
		//Create img tag and insert it into the slot
		const image = document.createElement("img");
		image.src = items[i].imageSrc;
		image.alt = items[i].name;
		const slot = document.getElementById("inventory-slot-" + (i + 1));
		slot.appendChild(image);
		//Set up select behavior for the slot
		image.addEventListener("click", () => {
			selectItem(items[i], i);
		});
	}
	// Fill Equipped item slot
	const equippedItem = gameState.player.itemEquipped;
	//Clear out equipped item slot
	const equippedItemSlot = document.getElementById("equipped-item");
	while (equippedItemSlot.firstChild) {
		equippedItemSlot.removeChild(equippedItemSlot.firstChild);
	}
	if (equippedItem != null) {
		const image = document.createElement("img");
		image.src = equippedItem.imageSrc;
		image.alt = equippedItem.name;
		equippedItemSlot.appendChild(image);
		//Set up the select behavior
		image.addEventListener("click", () => {
			selectItem(equippedItem, -1);
		});
	}
	// Set up the equip button
	const equipButton = document.getElementById("equip-button");
	equipButton.removeEventListener("click", equipItem);
	equipButton.addEventListener("click", equipItem);
}

async function equipItem() {
	// If there is no selected item then do nothing.
	if (selectedItemIndex == -1) {
		return;
	}
	await $.ajax({
		url: '/Game/EquipItem',
		type: 'POST',
		data: { itemIndex: selectedItemIndex },
		success: async function (response) {
			await loadItems(); //Refresh the inventory to show new changes
		},
		error: function (xhr, status, error) {
			console.error('Error equipping item: ', error);
		}
	});
}

// Function to select an item and display its stats
function selectItem(item, index) {
	//Save the items Id so we can use it later
	selectedItemIndex = index;
	// Display item stats
	document.getElementById("item-name").textContent = item.name;
	document.getElementById("item-attack").textContent = item.attack;
	document.getElementById("item-defense").textContent = item.defense;
	document.getElementById("item-image").src = item.imageSrc;
	// Show the elements
	document.getElementById("item-name").style.display = "block";
	document.getElementById("item-attack").style.display = "block";
	document.getElementById("item-defense").style.display = "block";
	document.getElementById("item-image").style.display = "block";
	document.getElementById("shield-icon").style.display = "block";
	document.getElementById("sword-icon").style.display = "block";
	//Remove the highlight on any other item slots
	for (let i = 1; i <= 20; i++) {
		document.getElementById("inventory-slot-" + i).style.borderColor = "";
	}
	document.getElementById("equipped-item").style.borderColor = "";
	// Highlight the selected inventory slot
	const selectedSlot = document.getElementById("inventory-slot-" + (selectedItemIndex+1));
	if (selectedSlot) selectedSlot.style.borderColor = "#ffdc4a";
}