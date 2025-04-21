//----------- CACHED ELEMENTS ---------------------------------------------------------------------------------------
//Commonly hidden/shown UI elements
let menu;
let map;
let inventory;
let equippedItemSlot;
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
//Cached map object that is defined server side so we grab it on load and then it never needs to be updated.
let mapDef;
//----------- LOAD UI ELEMENTS AND ADD EVENT LISTENERS ---------------------------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", async () => {
	//Grab the commonly used UI elements on load.
	menu = document.getElementById("menu");
	inventory = document.getElementById("inventory");
	equippedItemSlot = document.getElementById("equipped-item");
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
	equipBtn.addEventListener("click", equipItem);
	mapBtn.addEventListener("click", () => { tabCurrent = 'map'; refreshMenu(); });//Set tab and refresh menu
	floorBtn.attachPlayerAction('move');
	//Fetch map from server.
	mapDef = await sendGetRequest("/Game/GetMap");
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
	//Make item details visible if an item is selected.
	itemDetails.syncVisibility(selectedItemIndex != -1);
}

function refreshMap() {
	const mapContainer = document.getElementById("map-container");
	mapContainer.innerHTML = ""; // Clear previous map
	// Updates the floor counter
	const floorTracker = document.getElementById("floor-tracker");
	floorTracker.textContent = "Floor " + gameState.floor;
	// Adds a check for if the player has defeated the boss to enable next floor button
	floorBtn.syncButtonState(gameState.playerLocation == 10 && gameState.isLocationComplete);
	// Draw the map nodes and edges
	for (let i = 0; i < mapDef.listMapNodes.length; i++) {
		// node.removeEventListener("click", movePlayerToNode());
		const nodeElement = document.createElement("button");
		nodeElement.className = "map-node";
		nodeElement.setAttribute("data-node-id", mapDef.listMapNodes[i].mapNodeId);
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
		// Show player which node they are on
		if (mapDef.listMapNodes[i].mapNodeId == gameState.playerLocation) {
			nodeElement.classList.add("map-node-current");
		}
		if (mapDef.listMapNodes[i].mapNodeId < gameState.playerLocation) {
			nodeElement.classList.add("map-node-completed");
		}
		// Enable the next node after the current node
		if (mapDef.listMapNodes[i].mapNodeId == gameState.playerLocation + 1 && gameState.isLocationComplete) {
			nodeElement.classList.add("map-node-enabled");
			// Add event listener for node click on enabled nodes
			nodeElement.attachPlayerAction('move');
		}
		else {
			nodeElement.classList.add("map-node-disabled");
		}
		// Don't create a map edge for the last node
		if (i == mapDef.listMapEdges.length) {
			continue;
		}
		// Create a map edge between nodes
		const nodeEdgeElement = document.createElement("div");
		nodeEdgeElement.className = "map-edge";
		mapContainer.appendChild(nodeEdgeElement);
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
	//Update item details to match selected item.
	document.getElementById("item-name").textContent = item.name;
	document.getElementById("item-attack").textContent = item.attack;
	document.getElementById("item-defense").textContent = item.defense;
	document.getElementById("item-image").src = item.imageSrc;
	//Show item details
	itemDetails.syncVisibility(selectedItemIndex != -1);
}

async function equipItem() {
	// If there is no selected item then do nothing.
	if (selectedItemIndex == -1) {
		return;
	}
	await sendPostRequest(`/Game/EquipItem?itemIndex=${selectedItemIndex}`);
	refreshMenu();
}