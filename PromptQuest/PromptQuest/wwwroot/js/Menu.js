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
//Buttons
let equipBtn;
let floorBtn;
//Client side state tracking variables
let isMapOpen = false; //Keep track of whether or not the map is open so we don't refresh it constantly.
let isInventoryOpen = false; //Keep track of whether or not the inventory is open so we don't refresh it constantly.
let selectedItemIndex = 0; //No item selected on load.
//Cached map object that is defined server side so we grab it on load and then it never needs to be updated.
let mapDef;
let legendVisible = true;
//----------- LOAD UI ELEMENTS AND ADD EVENT LISTENERS ---------------------------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", async () => {
	menu = document.getElementById('menu');
	inventory = document.getElementById("inventory");
	map = document.getElementById("map");
	equippedWeaponSlot = document.getElementById("equipped-weapon");
	equippedHelmSlot = document.getElementById("equipped-helm");
	equippedChestSlot = document.getElementById("equipped-chest");
	equippedLegsSlot = document.getElementById("equipped-legs");
	equippedBootsSlot = document.getElementById("equipped-boots");
	itemDetails = document.getElementById("item-details-container");
	//Grab all the buttons that need to be cached from the DOM on load.
	equipBtn = document.getElementById("equip-btn");
	floorBtn = document.getElementById("floor-btn");
	//Add all menu buttons button event listeners on load
	document.getElementById("menu-btn").addEventListener("click", () => { overlay.syncVisibility(true); menu.syncVisibility(true); }); //Show overlay to blur background and show meny on top of it.
	document.getElementById("continue-btn").addEventListener("click", () => { overlay.syncVisibility(false); menu.syncVisibility(false); }); //Hide overlay and menu.
	document.getElementById("quit-btn").addEventListener("click", () => {window.location.replace("/Home"); }); // Redirect to /Home and clear history
	document.getElementById("open-inventory-btn").addEventListener("click", () => { overlay.syncVisibility(true); inventory.syncVisibility(true); isInventoryOpen = true; refreshInventory(); }); 
	document.getElementById("close-inventory-btn").addEventListener("click", () => { overlay.syncVisibility(false); inventory.syncVisibility(false); itemDetails.syncVisibility(false); isInventoryOpen = false; });//Force menu to hide on click.
	document.getElementById("open-map-btn").addEventListener("click", () => { overlay.syncVisibility(true); map.syncVisibility(true); isMapOpen = true;  refreshMap(); });//Set tab and refresh menu
	document.getElementById("close-map-btn").addEventListener("click", () => { overlay.syncVisibility(false); map.syncVisibility(false); isMapOpen = false; });//Set tab and refresh menu
	document.getElementById("legend").addEventListener("click", () => { showHideLegend(); });//toggle the legend display
  equipBtn.attachPlayerAction('equip', () => selectedItemIndex);
	floorBtn.attachPlayerAction('move', () => 1);
	floorBtn.addEventListener("click", async () => {
		await executePlayerAction('move', 1);
		let data = await sendGetRequest(`/Game/GetBackground?floor=${gameState.floor}`);
		document.getElementById("game-container").style.backgroundImage = data;
	});
	// Fetch map from the server
	mapDef = await sendGetRequest("/Game/GetMap");
});

//------------------------ REFRESH DISPLAY METHODS --------------------------------------------------------------------------------------------------------

function refreshInventory() {
	if (!isInventoryOpen) {
		return; // Inventory isn't open, so don't refresh
	}
	itemDetails.syncVisibility(selectedItemIndex != -1);
	document.getElementById("gold-display").textContent = gameState.player.gold;
	const items = gameState.player.items;
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
			selectItem(items[i], i);
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
	if (!isMapOpen) {
		return; // Map isn't open, so don't refresh
	}
	// Fetch map from the server
	mapDef = await sendGetRequest(`/Game/GetMap?floor=${gameState.floor}`);
	const mapContainer = document.getElementById("map-container");
	mapContainer.innerHTML = ""; // Clear previous map
	// Update floor counter & background
	document.getElementById("floor-tracker").textContent = "Floor " + gameState.floor;
	// Enable next floor button if the boss is defeated
	floorBtn.syncButtonState(gameState.playerLocation == 18 && gameState.isLocationComplete);
	// Store node elements for edge calculations
	let nodeElements = [];
	//Check if we are in landscape mode or portrait mode
	let isPortraitMode = document.querySelectorAll('.portrait-layout').length > 0;
	//Get max node height so we can scale the map properly.
	let maxNodeHeight = Math.max(...mapDef.listMapNodes.map(mapNode => mapNode.nodeHeight)) + 1;
	//Get max node distance so we can scale the map properly.
	let maxNodeDistance = Math.max(...mapDef.listMapNodes.map(mapNode => mapNode.nodeDistance));
	//Create some units based on the max values.
	let heightUnit = 100 / maxNodeHeight;
	let distanceUnit = 100 / maxNodeDistance;
	// Draw map nodes
	mapDef.listMapNodes.forEach((node, index) => {
		const nodeElement = document.createElement("button");
		nodeElement.className = "map-node";
		nodeElement.setAttribute("data-node-id", node.mapNodeId);
		nodeElement.style.position = "absolute";

		// Set position dynamically
		let y = (node.nodeHeight) * heightUnit;
		let x = (node.nodeDistance-1) * distanceUnit;
		if (isPortraitMode) {
			//We're in portrait mode so x is top and y is left
			nodeElement.style.bottom = x + '%';
			nodeElement.style.left = y + '%';
		}
		else {
			//We're in landscape mode so y is top and x is left
			nodeElement.style.top = y + '%';
			nodeElement.style.left = x + '%';
		}
		// Append node and store it for edge calculations
		mapContainer.appendChild(nodeElement);
		nodeElements.push({ id: node.mapNodeId, element: nodeElement });
		// Add appropriate images/icons
		const iconMap = {
			"Boss": "/images/BossIcon.png",
			"Elite": "/images/EliteEnemyIcon.png",
			"Campsite": "/images/campsite.png",
			"Event": "/images/event.png",
			"Treasure": "/images/treasure.png",
			"Shop": "/images/shop.png"
		};
		if (iconMap[node.nodeType]) {
			const imgElement = document.createElement("img");
			imgElement.src = iconMap[node.nodeType];
			imgElement.className = "map-image";
			nodeElement.appendChild(imgElement);
		}
		// Player's current location
		if ((index + 1) === gameState.playerLocation) {
			const playerIcon = document.createElement("img");
			playerIcon.src = "/images/PlayerIcon.png";
			playerIcon.className = "player-icon";
			nodeElement.appendChild(playerIcon);
			nodeElement.classList.add(gameState.isLocationComplete ? "map-node-completed" : "map-node-current");
		}
		// Mark visited nodes
		if (gameState.listMapNodeIdsVisited.includes((index + 1).toString())) {
			nodeElement.classList.add("map-node-completed");
		}
		// Enable nodes the player can move to
		if (mapDef.listMapNodes[gameState.playerLocation - 1].connectedNodes.includes(index + 1) && gameState.isLocationComplete) {
			const connectedNode = mapContainer.querySelector(`[data-node-id="${index + 1}"]`);
			if (connectedNode && node.mapNodeId != 1) {
				connectedNode.classList.add("map-node-enabled");
				connectedNode.attachPlayerAction('move', () => index + 1);
			}
		} else {
			nodeElement.classList.add("map-node-disabled");
		}
	});
	//Add all the edges and between the nodes.
	calculateMapEdges(mapDef, nodeElements, mapContainer);
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

//------------------------ HELPER METHODS --------------------------------------------------------------------------------------------------------

function calculateMapEdges(mapDef, nodeElements, mapContainer) {
	// Draw edges between connected nodes
	mapDef.listMapNodes.forEach((node) => {
		const currentNode = nodeElements.find(n => n.id === node.mapNodeId);
		if (!currentNode) return;
		node.connectedNodes.forEach((connectedId) => {
			const connectedNode = nodeElements.find(n => n.id === connectedId);
			if (!connectedNode) return;
			if (connectedId == 1) return; // Prevent drawing an edge from the last node to the first
			// Create edge
			const edgeElement = document.createElement("div");
			edgeElement.className = "map-edge";
			//Check if this is the last visited node -> Apply amber color
			if (connectedId == gameState.playerLocation && gameState.listMapNodeIdsVisited.includes(node.mapNodeId.toString())) {
				edgeElement.style.borderColor = "#EAAB11"; // Amber color
			}
			//Check if the connected node is completed
			if (currentNode.element.classList.contains("map-node-completed") && connectedNode.element.classList.contains("map-node-completed")) {
				edgeElement.style.borderColor = "darkgreen"; // Apply green styling
			}
			//Get center positions
			const startX = currentNode.element.offsetLeft + currentNode.element.offsetWidth / 2;
			const startY = currentNode.element.offsetTop + currentNode.element.offsetHeight / 2;
			const endX = connectedNode.element.offsetLeft + connectedNode.element.offsetWidth / 2;
			const endY = connectedNode.element.offsetTop + connectedNode.element.offsetHeight / 2;
			//Calculate angle and distance
			const deltaX = endX - startX;
			const deltaY = endY - startY;
			const distance = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
			const angleRad = Math.atan2(deltaY, deltaX);
			//Account for elliptical node borders
			const startWidthOffset = currentNode.element.offsetWidth / 2 * Math.cos(angleRad);
			const startHeightOffset = currentNode.element.offsetHeight / 2 * Math.sin(angleRad);
			const endWidthOffset = connectedNode.element.offsetWidth / 2 * Math.cos(angleRad);
			const endHeightOffset = connectedNode.element.offsetHeight / 2 * Math.sin(angleRad);
			//Adjust start & end positions to stop at the ellipse borders
			const adjustedStartX = startX + startWidthOffset;
			const adjustedStartY = startY + startHeightOffset;
			const adjustedEndX = endX - endWidthOffset;
			const adjustedEndY = endY - endHeightOffset;
			//Adjust final edge width
			const adjustedDistance = Math.sqrt(
				(adjustedEndX - adjustedStartX) ** 2 + (adjustedEndY - adjustedStartY) ** 2
			);
			//Apply styles
			edgeElement.style.width = `${adjustedDistance}px`;
			edgeElement.style.left = `${adjustedStartX}px`;
			edgeElement.style.top = `${adjustedStartY}px`;
			edgeElement.style.transform = `rotate(${angleRad * (180 / Math.PI)}deg)`;
			//Append edge
			mapContainer.appendChild(edgeElement);
		});
	});
} 
function showHideLegend() {
	legendVisible = !legendVisible;

	visAttribute = "visible";
	if (!legendVisible) {
		visAttribute = "hidden";
	}
	document.querySelectorAll(".legend-item").forEach(el => { el.style.visibility=visAttribute });
	
}
