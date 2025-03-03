// "Global" Variable to keep track of the game state locally so that functions don't have to be passed parameters all the time.
let gameState;
document.addEventListener("DOMContentLoaded", async function () {

	// Page loaded, get current game state and store it locally.  
	let response = await fetch("/Game/GetGameState");
	gameState = await response.json();
	// Check if the user doesn't have a character
	if (gameState.player == null) {// new player or session expired
		window.location.href = "/"; // boot them to the Main Menu.
	}
	// Update display with loaded data.  
	updateDisplay();
	if (gameState.inCombat == true) {// Player is in combat .
		// Show the combat UI
		showCombatUI();
		//addLogEntry("You were attacked by an " + gameState.enemy.name + "!");  
		if (gameState.isPlayersTurn == true) {
			enableCombatButtons();// Start player's turn.
		}
		else {
			executeEnemyAction();
		}
	}
	else {// Player is not in combat.  
		// Hide combat UI.  
		hideCombatUI();
	}
});
// Function that makes an ajax call telling the game engine to process the engine, then updates the local gameState variable with the response.  
async function executePlayerAction(action) {
	await $.ajax({
		url: '/Game/PlayerAction',
		type: 'POST',
		data: { action: action },
		success: function (response) {
			let actionResult = response;
			console.log('Player action (' + action + ') executed successfully:', actionResult);
			updateLocalGameState(actionResult); // Update local gameState variable with whatever the action changed.  
			updateDisplay() // Update screen to show whatever the action changed.  
		},
		error: function (xhr, status, error) {
			console.error('Error executing player action (' + action + '):', error);
		}
	});
	// Check if player is in combat.
	if (gameState.inCombat == false) {
		hideCombatUI(); // They aren't, hide combat UI
		return;
	}
	if (gameState.isPlayersTurn == false) {
		disableCombatButtons(); // They are, but it's not the player's turn anymore
		// Add a small delay so that the enemy's turn takes time.  
		setTimeout(async () => {
			await executeEnemyAction(); // The enemy action is determined server side.
		}, 1000);
		return;
	}
	// No need to enable combat buttons here because they are already enabled to allow the player to make this action.
}

// Function that makes an ajax call telling the game engine to process the engine, then updates the local gameState variable with the response.  
async function executeEnemyAction() {
	await $.ajax({
		url: '/Game/EnemyAction',
		type: 'POST',
		success: function (response) {
			let actionResult = response;
			console.log('Enemy action executed successfully:', actionResult);
			updateLocalGameState(actionResult); // Update local gameState variable with whatever the action changed.  
			updateDisplay(); // Update screen to show whatever the action changed.  --
		},
		error: function (xhr, status, error) {
			console.error('Error executing enemy action:', error);
		}
	});

	// Check if player is in combat.
	if (gameState.inCombat == false) {
		hideCombatUI(); // They are not, hide combat UI
		return;
	}
	if (gameState.isPlayersTurn == false) {
		executeEnemyAction(); // Enemy continues its turn.
		return;
	}
	enableCombatButtons(); // Player's turn.
}

async function spawnNewEnemy() {
	// Tell the server to start combat and wait for it to be done
	await fetch("/Game/StartCombat", { method: "POST" });
	// Game state changed, grab it
	let response = await fetch("/Game/GetGameState");
	gameState = await response.json();
	// Update display
	updateDisplay();
	showCombatUI();
}

// Helper function to update the gameState variable with the results from a PQActionResult.  
function updateLocalGameState(actionResult) {
	// Update inCombat state.
	gameState.inCombat = actionResult.inCombat;
	// Update isPlayersTurn state.
	gameState.isPlayersTurn = actionResult.isPlayersTurn;
	// Update player health.
	gameState.player.currentHealth = actionResult.playerHealth;
	// Update player health potions.
	gameState.player.healthPotions = actionResult.playerHealthPotions;
	// Update enemy health.
	gameState.enemy.currentHealth = actionResult.enemyHealth;
	// Update message log
	gameState.messageLog.push(actionResult.message);
	// Log the updated gameState for debugging.
	console.log('Updated local gameState:', gameState);
}

//----------- Functions - Display Updates -----------------------------------------------------------------------------------------------------------  

// Function to update the player's display  
// Function to update the player's display  
function updateDisplay() {
	// Update Player display.
	document.querySelectorAll(".player-name").forEach(el => { el.textContent = gameState.player.name; });
	document.querySelectorAll(".player-image").forEach(el => { el.src = "/images/PlaceholderPlayerPortrait.png"; }); // Placeholder image for now.
	document.querySelectorAll(".player-image").forEach(el => { el.alt = gameState.player.name; });
	document.querySelectorAll(".player-attack").forEach(el => { el.textContent = gameState.player.attack; });
	document.querySelectorAll(".player-defense").forEach(el => { el.textContent = gameState.player.defense; });
	document.querySelectorAll(".player-hp").forEach(el => { el.textContent = gameState.player.currentHealth + "/" + gameState.player.maxHealth + " HP"; });
	document.getElementById("player-hp").textContent = gameState.player.currentHealth + "/" + gameState.player.maxHealth + " HP";
	document.getElementById("player-health-potions").textContent = gameState.player.healthPotions;
	// Update Enemy display.
	document.getElementById("enemy-name").textContent = gameState.enemy.name;
	document.getElementById("enemy-image").src = gameState.enemy.imageUrl;
	document.getElementById("enemy-image").alt = gameState.enemy.name;
	document.getElementById("enemy-attack").textContent = gameState.enemy.attack;
	document.getElementById("enemy-defense").textContent = gameState.enemy.defense;
	document.getElementById("enemy-hp").textContent = gameState.enemy.currentHealth + "/" + gameState.enemy.maxHealth + " HP";
	// Fill the dialog box with any messages
	let numMessages = gameState.messageLog.length
	for (let i = numMessages - 5; i < numMessages; i++) {// Show last 5 messages, not the first 5.
		addLogEntry(gameState.messageLog[i]);
	}
}

// Function to add log entries to the dialog box.  
function addLogEntry(message) {
	const dialogBox = document.querySelector(".DialogBox");
	const logLimit = 5;
	if (dialogBox.childElementCount >= logLimit) {
		dialogBox.innerHTML = "";
	}
	const logDiv = document.createElement("div");
	logDiv.textContent = message;
	dialogBox.appendChild(logDiv);
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
	const combatButtonsDisplay = document.getElementById("combat-buttons-display");
	combatButtonsDisplay.style.visibility = "hidden";
	// Check if player is still alive
	if (gameState.player.currentHealth > 0) {
		// Allow player to start the next fight
		addLogEntry("Player health is: " + gameState.player.currentHealth); // debug
		addLogEntry("Enemy defeated! Press [SPACE] to fight the next enemy.");
		enableNextFightTrigger(); // Wait for space key press
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
	hideCombatUI();
}

// Function to show the Combat UI.  
function showCombatUI() {
	// Enable the combat buttons
	enableCombatButtons();
	// Show the enemy display.
	const enemyDisplay = document.getElementById("enemy-display");
	enemyDisplay.style.visibility = "visible";
	// Show the combat buttons display.
	const combatButtonsDisplay = document.getElementById("combat-buttons-display");
	combatButtonsDisplay.style.visibility = "visible";
}

//----------- Helper Functions - End -----------------------------------------------------------------------------------------------------------