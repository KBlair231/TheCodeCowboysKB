async function spawnNewEnemy() {
	// Tell the server to start combat and wait for it to be done
	await fetch("/Game/StartCombat", { method: "POST" });
	// Game state changed, grab it
	let response = await fetch("/Game/GetGameState");
	gameState = await response.json();
	// Update display
	updateDisplay();
	// Gets rid of last combat's messages;
	clearDialogBox()
}

// Function to disable the combat buttons and remove their event handlers.  
function disableCombatButtons() {
	// Disable Attack button.  
	const attackButton = document.getElementById("attack-btn");
	attackButton.removeEventListener("click", handleAttackClick);
	attackButton.disabled = true;
	attackButton.classList.add("PQButtonDisabled");
	// Disable the Use Health Potion button.  
	const healButton = document.getElementById("health-potion-btn");
	healButton.removeEventListener("click", handleHealClick);
	healButton.disabled = true;
	healButton.classList.add("PQButtonDisabled");
}

// Function to enable the combat buttons and add their event handlers.  
function enableCombatButtons() {
	// Enable Attack button.  
	const attackButton = document.getElementById("attack-btn");
	attackButton.addEventListener("click", handleAttackClick);
	attackButton.disabled = false;
	attackButton.classList.remove("PQButtonDisabled");
	// Enable the Use Health Potion button.  
	const healButton = document.getElementById("health-potion-btn");
	healButton.addEventListener("click", handleHealClick);
	healButton.disabled = false;
	healButton.classList.remove("PQButtonDisabled");
}

// Wrapper for Attack button click event handler
async function handleAttackClick() {
	await executePlayerAction('attack');
}

// Wrapper for Use Health Potion button click event handler
async function handleHealClick() {
	await executePlayerAction('heal');
}

function enableNextFightTrigger() {
	document.addEventListener("keydown", handleSpacePress);
}

function handleSpacePress(event) {
	if (event.code === "Space") {
		document.removeEventListener("keydown", handleSpacePress);
		spawnNewEnemy();
	}
}
// Function to hide the Combat UI.  
function hideCombatUI() {
	// Just in case.
	disableCombatButtons();
	// Hide the enemy display.  
	const enemyDisplay = document.getElementById("enemy-display");
	enemyDisplay.style.visibility = "hidden";
	// Hide the combat buttons display.  
	const combatButtonsDisplay = document.getElementById("action-button-display");
	combatButtonsDisplay.style.visibility = "hidden";
	// Check if player is still alive 
	if (gameState.player.currentHealth > 0) {
		// Allow player to start the next fight
		addLogEntry("Check your map to move to the next location!");
	}
	else {
		// Player is dead
		showRespawnModal(); // Show the respawn modal
	}
}

// Function to show the respawn modal
function showRespawnModal() {
	const respawnModal = new bootstrap.Modal(document.getElementById('respawnModal'));
	respawnModal.show();
}

// Function to hide the respawn modal
function hideRespawnModal() {
	const respawnModalElement = document.getElementById('respawnModal');
	const modalInstance = bootstrap.Modal.getInstance(respawnModalElement);
	if (modalInstance) {
		modalInstance.hide();
	}
}

// Function to respawn the player
async function respawnPlayer() {
	await fetch("/Game/Respawn", { method: "POST" });
	// Game state changed, grab it
	let response = await fetch("/Game/GetGameState");
	gameState = await response.json();
	// Update display
	updateDisplay();
	hideRespawnModal();
}

// Function to show the Combat UI.  
function showCombatUI() {
	// Enable the combat buttons
	enableCombatButtons();
	// Show the enemy display.
	const enemyDisplay = document.getElementById("enemy-display");
	enemyDisplay.style.visibility = "visible";
	// Show the combat buttons display.
	const combatButtonsDisplay = document.getElementById("action-button-display");
	combatButtonsDisplay.style.visibility = "visible";
}