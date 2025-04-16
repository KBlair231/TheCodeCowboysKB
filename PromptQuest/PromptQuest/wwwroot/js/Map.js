﻿// Global Variables
let map;

// Get map from server on load
document.addEventListener("DOMContentLoaded", async function () {
	let response = await fetch("/Game/GetMap");
	map = await response.json();
});

// Map Functions
function updateMap() {
	const mapContainer = document.getElementById("map-container");
	mapContainer.innerHTML = ""; // Clear previous map
	// Updates the floor counter
	const floorTracker = document.getElementById("floor-tracker");
	floorTracker.textContent = "Floor " + gameState.floor;
	// Adds a check for if the player has defeated the boss to show next floor button
	if (gameState.playerLocation == 10 && gameState.isLocationComplete) {
		nextFloorButton.style.visibility = "visible";
		nextFloorButton.addEventListener("click", movePlayerToNode)
	} else {
		nextFloorButton.style.visibility = "hidden";
	}
	// Draw the map nodes and edges
	for (let i = 0; i < map.listMapNodes.length; i++) {
		// node.removeEventListener("click", movePlayerToNode());
		const nodeElement = document.createElement("div");
		nodeElement.className = "map-node";
		nodeElement.setAttribute("data-node-id", map.listMapNodes[i].mapNodeId);
		mapContainer.appendChild(nodeElement);
		// Show player which node they are on
		if (map.listMapNodes[i].mapNodeId == gameState.playerLocation) {
			nodeElement.classList.add("map-node-current");
		}
		if (map.listMapNodes[i].mapNodeId < gameState.playerLocation) {
			nodeElement.classList.add("map-node-completed");
		}
		// Enable the next node after the current node
		if (map.listMapNodes[i].mapNodeId == gameState.playerLocation + 1 && gameState.isLocationComplete) {
			nodeElement.classList.add("map-node-enabled");
			// Add event listener for node click on enabled nodes
			nodeElement.addEventListener("click", async function() {movePlayerToNode()});
		}
		else {
			nodeElement.classList.add("map-node-disabled");
		}
		// Don't create a map edge for the last node
		if (i == map.listMapEdges.length) { 
			continue;
		}
		// Create a map edge between nodes
		const nodeEdgeElement = document.createElement("div");
		nodeEdgeElement.className = "map-edge";
		mapContainer.appendChild(nodeEdgeElement);
	}
}

async function movePlayerToNode() {
	await executePlayerAction('move');
	updateMap();
	spawnNewEnemy();
}