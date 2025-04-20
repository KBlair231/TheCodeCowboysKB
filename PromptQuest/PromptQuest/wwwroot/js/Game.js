//----------- CACHED ELEMENTS ---------------------------------------------------------------------------------------
//Commonly hidden/shown UI elements
let playerDisplay;
let enemyDisplay;
let actionButtonDisplay;
let backgroundImage;
let campsiteButtonDisplay;
let eventButtonDisplay;
let dialogBox;
//Player action buttons
let attackBtn;
let healBtn;
let restBtn;
let skipRestBtn;
let acceptBtn;
let denyBtn;

//----------- LOAD UI ELEMENTS AND ADD EVENT LISTENERS ---------------------------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", () => {
	//Grab the commonly used UI elements on load.
	playerDisplay = document.getElementById("player-display");
	enemyDisplay = document.getElementById("enemy-display");
	actionButtonDisplay = document.getElementById("action-button-display");
	backgroundImage = document.getElementById("bg-image");
	campsiteButtonDisplay = document.getElementById("campsite-button-display");
	eventButtonDisplay = document.getElementById("event-button-display");
	dialogBox = document.getElementById("dialog-box");
	//Grab all the player action buttons from the DOM on load.
	attackBtn = document.getElementById("attack-btn");
	healBtn = document.getElementById("health-potion-btn");
	restBtn = document.getElementById("rest-btn");
	skipRestBtn = document.getElementById("skip-rest-btn");
	acceptBtn = document.getElementById("accept-btn");
	denyBtn = document.getElementById("deny-btn");
	//Add all player action button event listeners on load
	attackBtn.attachPlayerAction('attack');
	healBtn.attachPlayerAction('heal');
	restBtn.attachPlayerAction('rest');
	skipRestBtn.attachPlayerAction('skip-rest');
	acceptBtn.attachPlayerAction('accept');
	denyBtn.attachPlayerAction('deny');
});

//----------- REFRESH DISPLAY ---------------------------------------------------------------------------------------

function refreshDisplay() {
	// Refresh all dynamic displays
	refreshPlayerDisplay();
	refreshEnemyDisplay();
	refreshDialogBox();
	refreshMenu();
	//Sync button states (disabled/enabled).
	attackBtn.syncButtonState(gameState.inCombat && gameState.isPlayersTurn && !gameState.isLocationComplete && gameState.player.currentHealth > 0);
	healBtn.syncButtonState(gameState.inCombat && gameState.isPlayersTurn && !gameState.isLocationComplete && gameState.player.currentHealth > 0);
	restBtn.syncButtonState(gameState.inCampsite && !gameState.isLocationComplete);
	skipRestBtn.syncButtonState(gameState.inCampsite && !gameState.isLocationComplete);
	acceptBtn.syncButtonState(gameState.inEvent && !gameState.isLocationComplete);
	denyBtn.syncButtonState(gameState.inEvent && !gameState.isLocationComplete);
	//Sync UI visibility (visible/hidden).
	playerDisplay.syncVisibility(gameState.player.currentHealth > 0);
	enemyDisplay.syncVisibility(gameState.inCombat && gameState.enemy.currentHealth > 0);
	actionButtonDisplay.syncVisibility(gameState.inCombat && !gameState.isLocationComplete && gameState.player.currentHealth > 0);
	backgroundImage.syncVisibility(gameState.inCampsite && !gameState.isLocationComplete);
	campsiteButtonDisplay.syncVisibility(gameState.inCampsite);
	eventButtonDisplay.syncVisibility(gameState.inEvent);
	//This will be merged into the sync pattern above at some point.
	hideRespawnModal();
	if (gameState.player.currentHealth <= 0) { 
		showRespawnModal();
	}
}

// This respawn modal code will be refactored soon to fit into the above patterns.

// Function to show the respawn modal
function showRespawnModal() {
	const respawnModal = new bootstrap.Modal(document.getElementById('respawnModal'));
	const respawnButton = document.getElementById("respawn-btn");
	respawnButton.attachPlayerAction('respawn');
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

// ------------------------ REFRESH DISPLAY HELPER METHODS ------------------------------------------------------------------------------------------------------

function refreshDialogBox() {
	//Clear old messages and load in the new list
	dialogBox.innerHTML = "";
	gameState.listMessages.forEach((message) => {
		const logDiv = document.createElement("div");
		logDiv.textContent = message;
		dialogBox.appendChild(logDiv);
		//logDiv.scrollIntoView({ behavior: "smooth" });
	});
	//Scroll to bottom to show new messages
	dialogBox.scrollTop = dialogBox.scrollHeight;
}

function refreshPlayerDisplay() {
	document.querySelectorAll(".player-name").forEach(el => { el.textContent = gameState.player.name; });
	document.querySelectorAll(".player-image").forEach(el => { el.src = "/images/" + gameState.player.class + ".png"; }); // Placeholder image for now.
	document.querySelectorAll(".player-image").forEach(el => { el.alt = gameState.player.name; });
	const equippedItem = gameState.player.itemEquipped
	document.querySelectorAll(".player-attack").forEach(el => { el.textContent = gameState.player.attack + equippedItem?.attack ?? 0; });
	document.querySelectorAll(".player-defense").forEach(el => { el.textContent = gameState.player.defense + equippedItem?.defense ?? 0; });
	document.querySelectorAll(".player-hp").forEach(el => { el.textContent = gameState.player.currentHealth + "/" + gameState.player.maxHealth + " HP"; });
	document.getElementById("player-health-potions").textContent = gameState.player.healthPotions;
}

function refreshEnemyDisplay() {
	document.getElementById("enemy-name").textContent = gameState.enemy.name;
	document.getElementById("enemy-image").src = gameState.enemy.imageUrl;
	document.getElementById("enemy-image").alt = gameState.enemy.name;
	document.getElementById("enemy-attack").textContent = gameState.enemy.attack;
	document.getElementById("enemy-defense").textContent = gameState.enemy.defense;
	document.getElementById("enemy-hp").textContent = gameState.enemy.currentHealth + "/" + gameState.enemy.maxHealth + " HP";
}

//------------------------ OVERLOADS --------------------------------------------------------------------------------------------------------------

//Shows/Hides an html element according to the given condition. If the condition is true, the element is shown, if not, it is hidden. Only updates if necessary to avoid UI flicker.
HTMLElement.prototype.syncVisibility = function (condition) {
	if (condition && this.style.display === "none") {
		//Element should be visible but is currently hidden. Show it.
		this.style.display = "block";
	}
	if (!condition && this.style.display !== "none") {
		//Element should be hidden but is currently visible. Hide it.
		this.style.display = "none";
	}
};

//Attaches a player action to a button as an eventlistener. Does not need to be removed because event will only fire if button is enabled.
HTMLButtonElement.prototype.attachPlayerAction = function (action) {
	this.addEventListener("click", async () => {
		if (this.disabled) {
			return; //Button is disabled, so let's bounce.
		}
		await executePlayerAction(action);
	});
};

//Enables/disables a button according to the given condition. If condition is true, button is enabled, if not, it is disabled. Only updates if necessary to avoid UI flicker.
HTMLButtonElement.prototype.syncButtonState = function (condition) {
	if (condition && this.disabled) {
		//Button should be enabled but is currently disabled. Enable it.
		this.disabled = false;
	}
	if (!condition && this.disabled == false) {
		//Button should not be enabled but is currently enabled. Disable it.
		this.disabled = true;
	}
};

