//----------- CACHED ELEMENTS ---------------------------------------------------------------------------------------
//Commonly hidden/shown UI elements
let menu;
let map;
let inventory;
let equippedWeaponSlot;
let equippedHelmSlot;
let equippedChestSlot;
let equippedLegsSlot;
let equippedBootsSlot;
let itemDetails;
//Menu buttons
let openMenuBtn;
let closeMenuBtn;
let inventoryBtn;
let equipBtn;
let mapBtn;
let floorBtn;
//Client side state tracking variables
let tabCurrent = 'inventory'; //Set inventory as the default tab.
let selectedItemIndex = -1; //No item selected on load.
let gold;
//Cached map object that is defined server side so we grab it on load and then it never needs to be updated.
let mapDef;
//----------- LOAD UI ELEMENTS AND ADD EVENT LISTENERS ---------------------------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", async () => {
	//Grab the commonly used UI elements on load.
	menu = document.getElementById("menu");
	inventory = document.getElementById("inventory");
	equippedWeaponSlot = document.getElementById("equipped-weapon");
	equippedHelmSlot = document.getElementById("equipped-helm");
	equippedChestSlot = document.getElementById("equipped-chest");
	equippedLegsSlot = document.getElementById("equipped-legs");
	equippedBootsSlot = document.getElementById("equipped-boots");
	itemDetails = document.getElementById("item-details");
	map = document.getElementById("map");
	//Grab all menu buttons from the DOM on load.
	openMenuBtn = document.getElementById("menu-open");
	closeMenuBtn = document.getElementById("menu-close");
	inventoryBtn = document.getElementById("inventory-btn");
	equipBtn = document.getElementById("equip-btn");
	mapBtn = document.getElementById("map-btn");
	floorBtn = document.getElementById("floor-btn");
	//Add all menu buttons button event listeners on load
	openMenuBtn.addEventListener("click", () => { menu.syncVisibility(true); refreshMenu(); }); 
	closeMenuBtn.addEventListener("click", () => { menu.syncVisibility(false); });//Force menu to hide on click.
	inventoryBtn.addEventListener("click", () => { tabCurrent = 'inventory'; refreshMenu(); });//Set tab and refresh menu
	equipBtn.attachPlayerAction('equip', () => selectedItemIndex);
	mapBtn.addEventListener("click", () => { tabCurrent = 'map'; refreshMenu(); });//Set tab and refresh menu
	floorBtn.attachPlayerAction('move', () => 1);
});

//------------------------ REFRESH DISPLAY --------------------------------------------------------------------------------------------------------

//This is the only method here that gets called outside of this file. It gets called by refreshDisplay() Game.js. This means this runs everytime we refresh after an action.
function refreshMenu() {
	//Show currently selected tab. 
	inventory.syncVisibility(tabCurrent === 'inventory');
	map.syncVisibility(tabCurrent === "map");
	//item details doesn't inheirit visibility from inventory so do it manually. Find a fix for this someday.
	itemDetails.syncVisibility(selectedItemIndex != -1);
	//Clear highlight on all tab buttons.
	inventoryBtn.classList.remove('pq-tab-current');
	mapBtn.classList.remove('pq-tab-current');
	//Highlight currently selected tab and refresh its contents
	if (tabCurrent === "inventory") {
		inventoryBtn.classList.add('pq-tab-current');
		refreshInventory();
	}
	if (tabCurrent === "map") {
		mapBtn.classList.add('pq-tab-current');
		refreshMap();
	}
}

function refreshInventory() {
	const items = gameState.player.items;
	gold = document.getElementById("gold-display").textContent = gameState.player.gold;
	//Clear all inventory slots.
	for (let i = 1; i <= 20; i++) {
		const slot = document.getElementById("inventory-slot-" + i);
		while (slot.firstChild) {
			slot.removeChild(slot.firstChild);
		}
	}
	//Load items into inventory slots.
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
		if (i == selectedItemIndex) {
			updateEquipButton(items[i]);
		}
	}
	fillEquippedSlot(gameState.player.equippedHelm);
	fillEquippedSlot(gameState.player.equippedChest);
	fillEquippedSlot(gameState.player.equippedLegs);
	fillEquippedSlot(gameState.player.equippedBoots);
	fillEquippedSlot(gameState.player.equippedWeapon);
	//Make item details visible if an item is selected.
	itemDetails.syncVisibility(selectedItemIndex != -1);
	equipBtn.syncButtonState(selectedItemIndex != -1);
}

async function refreshMap() {
	//Fetch map from server.
	mapDef = await sendGetRequest("/Game/GetMap");
	const mapContainer = document.getElementById("map-container");
	mapContainer.innerHTML = ""; // Clear previous map
	// Updates the floor counter
	const floorTracker = document.getElementById("floor-tracker");
	floorTracker.textContent = "Floor " + gameState.floor;
	// Adds a check for if the player has defeated the boss to enable next floor button
	floorBtn.syncButtonState(gameState.playerLocation == 18 && gameState.isLocationComplete);
	// Draw the map nodes and edges
	for (let i = 0; i < mapDef.listMapNodes.length; i++) {
		// node.removeEventListener("click", movePlayerToNode());
		const nodeElement = document.createElement("button");
		nodeElement.className = "map-node";
		nodeElement.setAttribute("data-node-id", mapDef.listMapNodes[i].mapNodeId);
		// Adjust the position of the node based on the nodeHeight value
		if (mapDef.listMapNodes[i].nodeHeight) {
			nodeElement.style.position = "absolute"; // Ensure the node is positioned absolutely
			nodeElement.style.top = `${mapDef.listMapNodes[i].nodeHeight * -100 + 200}px`; // Adjust the vertical position
		}
		if (mapDef.listMapNodes[i].nodeDistance) {
			nodeElement.style.left = `${mapDef.listMapNodes[i].nodeDistance * 100}px`; // Adjust the horizontal position
		}
		mapContainer.appendChild(nodeElement);
		// Check for NodeType and add image if it is "Boss"
		if (mapDef.listMapNodes[i].nodeType === "Boss") {
			const imgElement = document.createElement("img");
			imgElement.src = "/images/boss.png";
			imgElement.className = "map-image";
			nodeElement.appendChild(imgElement);
		}
		// Check for NodeType and add image if it is "Campsite"
		if (mapDef.listMapNodes[i].nodeType === "Campsite") {
			const imgElement = document.createElement("img");
			imgElement.src = "/images/campsite.png";
			imgElement.className = "map-image";
			nodeElement.appendChild(imgElement);
		}
		// Check for NodeType and add image if it is "Event"
		if (mapDef.listMapNodes[i].nodeType === "Event") {
			const imgElement = document.createElement("img");
			imgElement.src = "/images/event.png";
			imgElement.className = "map-image";
			nodeElement.appendChild(imgElement);
		}
		// Check for NodeType and add image if it is "Treasure"
		if (mapDef.listMapNodes[i].nodeType === "Treasure") {
			const imgElement = document.createElement("img");
			imgElement.src = "/images/treasure.png";
			imgElement.className = "map-image";
			nodeElement.appendChild(imgElement);
		}
		// Show player which node they are on
		if ((i + 1) == gameState.playerLocation) {
			nodeElement.classList.add("map-node-current");
		}
		// gameState.isLocationComplete
		if (gameState.listMapNodeIdsVisited.includes((i + 1).toString())) {
			nodeElement.classList.add("map-node-completed");
		}
		// Enable the nodes in connectedNodes
		if (mapDef.listMapNodes[gameState.playerLocation - 1].connectedNodes.includes(i + 1) && gameState.isLocationComplete) {
			const connectedNode = mapContainer.querySelector(`[data-node-id="${i + 1}"]`);
			if (connectedNode) {
				connectedNode.classList.add("map-node-enabled");
				// Add event listener for node click on enabled nodes
				connectedNode.attachPlayerAction('move', () => (i + 1));
			}
		}
		else {
			nodeElement.classList.add("map-node-disabled");
		}
		// Don't create a map edge for the last node
		//if (i == mapDef.listMapEdges.length) {
		//	continue;
		//}
		// Create a map edge between nodes
		//const nodeEdgeElement = document.createElement("div");
		//nodeEdgeElement.className = "map-edge";
		//mapContainer.appendChild(nodeEdgeElement);
	}
}

//------------------------ MORE COMPLEX CLICK HANDLERS --------------------------------------------------------------------------------------------------------

function selectItem(item, index) {
	//Check if there was a previous slot selected.
	const oldInventorySlot = document.getElementById("inventory-slot-" + (selectedItemIndex + 1));
	if (oldInventorySlot != null) {
		oldInventorySlot.style.borderColor = "";//Clear old highlight
	}
	selectedItemIndex = index;
	//Check if there a new slot selected (always is unless they click the equipped slot)
	const newInventorySlot = document.getElementById("inventory-slot-" + (selectedItemIndex + 1));
	if (newInventorySlot != null) {
		newInventorySlot.style.borderColor = "#ffdc4a";//Highlight newly selected slot.
	}
	updateEquipButton(item);
	//Update item details to match selected item.
	document.getElementById("item-name").textContent = item.name;
	document.getElementById("item-attack").textContent = item.attack;
	document.getElementById("item-defense").textContent = item.defense;
	document.getElementById("item-image").src = item.imageSrc;
	document.getElementById("item-status-effects").textContent = statusEffectCheck(item.statusEffects);
	document.getElementById("item-passive").textContent = getPassiveDescription(item.passive);
	document.getElementById("item-type").textContent = 'Type: ' + itemTypeCheck(item.itemType);
	//Show item details
	itemDetails.syncVisibility(selectedItemIndex != -1);
}
function statusEffectCheck(item) {
	textReturn = 'Chance on Hit: ';
	if (item == 1) {
		return textReturn + 'Bleeding';	// Can't have 'Chance on Hit: ' + on line 202 because items without a status effect will
	} else if (item == 2) {					// display 'Chance on Hit: undefined'
		return textReturn + 'Burning';
	}
}
function itemTypeCheck(item) {
	if (item == 0) {
		return 'Weapon';
	} else if (item == 1) {
		return 'Boots';
	} else if (item == 2) {
		return 'Legs';
	} else if (item == 3) {
		return 'Chest';
	} else if (item == 4) {
		return 'Helm';
	}
}
function fillEquippedSlot(item) {
	if ( item == null) {
		return;
	}
	//Clear out equipped item slot
	const equippedSlot = document.getElementById("equipped-" + itemTypeCheck(item.itemType).toLowerCase());
	while (equippedSlot.firstChild) {
		equippedSlot.removeChild(equippedSlot.firstChild);
	}
	if (item.imageSrc == "") {
		return;
	}
	const image = document.createElement("img");
	image.src = item.imageSrc;
	image.alt = item.name;
	image.id = item.name;
	equippedSlot.appendChild(image);
}
function updateEquipButton(item) {
	equipBtn.disabled = false;
	equipBtn.textContent = "Equip Item";
	if (gameState.player.equippedWeapon.name == item.name) {
		equipBtn.textContent = "Unequip";
	}
	if (gameState.player.equippedBoots.name == item.name) {
		equipBtn.textContent = "Unequip";
	}
	if (gameState.player.equippedLegs.name == item.name) {
		equipBtn.textContent = "Unequip";
	}
	if (gameState.player.equippedChest.name == item.name) {
		equipBtn.textContent = "Unequip";
	}
	if (gameState.player.equippedHelm.name == item.name) {
		equipBtn.textContent = "Unequip";
	}
}

async function equipItem() {
	// If there is no selected item then do nothing.
	if (selectedItemIndex == -1) {
		return;
	}
	await sendPostRequest(`/Game/EquipItem?itemIndex=${selectedItemIndex}`);
	refreshMenu();
}