//----------- CACHED ELEMENTS ---------------------------------------------------------------------------------------
//Commonly hidden/shown UI elements
let playerDisplay;
let enemyDisplay;
let actionButtonDisplay;
let backgroundImage;
let campsiteButtonDisplay;
let eventButtonDisplay;
let treasureButtonDisplay;
let shopButtonDisplay;
let dialogBox;
let abilityCooldownIcon;
let bleedingIndicator;
let burningIndicator;
let playerHealthBar;
let enemyHealthBar;
let activePopupWindow;
//Player action buttons
let attackBtn;
let healBtn;
let restBtn;
let skipRestBtn;
let acceptBtn;
let denyBtn;
let abilityBtn;
let openTreasureBtn;
let skipTreasureBtn;
let respawnBtn;

//----------- LOAD UI ELEMENTS AND ADD EVENT LISTENERS ---------------------------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", () => {
	//Grab the commonly used UI elements on load.
	playerDisplay = document.getElementById("player-display");
	enemyDisplay = document.getElementById("enemy-display");
	actionButtonDisplay = document.getElementById("action-button-display");
	backgroundImage = document.getElementById("bg-image");
	campsiteButtonDisplay = document.getElementById("campsite-button-display");
	eventButtonDisplay = document.getElementById("event-button-display");
	treasureButtonDisplay = document.getElementById("treasure-button-display");
	shopButtonDisplay = document.getElementById("shop-button-display");
	dialogBox = document.getElementById("dialog-box");
	bleedingIndicator = document.getElementById("bleeding-indicator");
	burningIndicator = document.getElementById("burning-indicator");
	abilityCooldownIcon = document.getElementById("ability-cooldown-icon");
	playerHealthBar = document.getElementById("player-health-bar");
	enemyHealthBar = document.getElementById("enemy-health-bar");
	//Grab all the player action buttons from the DOM on load.
	attackBtn = document.getElementById("attack-btn");
	healBtn = document.getElementById("health-potion-btn");
	restBtn = document.getElementById("rest-btn");
	skipRestBtn = document.getElementById("skip-rest-btn");
	acceptBtn = document.getElementById("accept-btn");
	denyBtn = document.getElementById("deny-btn");
	abilityBtn = document.getElementById("ability-btn");
	openTreasureBtn = document.getElementById("open-treasure-btn");
	skipTreasureBtn = document.getElementById("skip-treasure-btn");
	respawnBtn = document.getElementById("respawn-btn");
	//Add all player action button event listeners on load
	attackBtn.attachPlayerAction('attack');
	healBtn.attachPlayerAction('heal');
	restBtn.attachPlayerAction('rest');
	skipRestBtn.attachPlayerAction('skip-rest');
	acceptBtn.attachPlayerAction('accept');
	denyBtn.attachPlayerAction('deny');
	abilityBtn.attachPlayerAction('ability');
	openTreasureBtn.attachPlayerAction('open-treasure');
	skipTreasureBtn.attachPlayerAction('skip-treasure');
	respawnBtn.attachPlayerAction('respawn');
	//Add hover action on/offs to items
	activePopupWindow = document.getElementById("active-popup");
	abilityBtn.addEventListener("mouseover", showActivePopup);
	abilityBtn.addEventListener("mouseout", hideActivePopup);
	hideActivePopup();
});

//----------- REFRESH DISPLAY ---------------------------------------------------------------------------------------

function refreshDisplay() {
	// Refresh all dynamic displays
	refreshPlayerDisplay();
	refreshEnemyDisplay();
	refreshDialogBox();
	refreshInventory();//kick out if the menu isn't open
	refreshMap();//kick out if the menu isn't open
	refreshShop();
	refreshActionBtnDisplay();
	//Sync button states (disabled/enabled).
	restBtn.syncButtonState(gameState.inCampsite && !gameState.isLocationComplete);
	skipRestBtn.syncButtonState(gameState.inCampsite && !gameState.isLocationComplete);
	acceptBtn.syncButtonState(gameState.inEvent && !gameState.isLocationComplete);
	denyBtn.syncButtonState(gameState.inEvent && !gameState.isLocationComplete);
	openTreasureBtn.syncButtonState(gameState.inTreasure && !gameState.isLocationComplete);
	skipTreasureBtn.syncButtonState(gameState.inTreasure && !gameState.isLocationComplete);
	//Sync UI visibility (visible/hidden).
	actionButtonDisplay.syncVisibility(gameState.inCombat);
	backgroundImage.syncVisibility(gameState.inCampsite && !gameState.isLocationComplete);
	campsiteButtonDisplay.syncVisibility(gameState.inCampsite);
	eventButtonDisplay.syncVisibility(gameState.inEvent);
	treasureButtonDisplay.syncVisibility(gameState.inTreasure);
	shopButtonDisplay.syncVisibility(gameState.inShop);
	// Check for status effects and update their visibility from the enum
	bleedingIndicator.syncVisibility(gameState.enemy.statusEffects > 0 && (gameState.enemy.statusEffects == 1 || gameState.enemy.statusEffects == 3));
	burningIndicator.syncVisibility(gameState.enemy.statusEffects > 0 && (gameState.enemy.statusEffects == 2 || gameState.enemy.statusEffects == 3));
}

// ------------------------ REFRESH DISPLAY HELPER METHODS ------------------------------------------------------------------------------------------------------

function refreshPlayerDisplay() {
	document.querySelectorAll(".player-name").forEach(el => { el.textContent = gameState.player.name; });
	document.querySelectorAll(".player-image").forEach(el => { el.src = `/Game/GetCharacterImage`; }); // Placeholder image for now.
	document.querySelectorAll(".player-image").forEach(el => { el.alt = gameState.player.name; });
	document.querySelectorAll(".player-attack").forEach(el => {
		const attackStat = gameState.player.attackStat ?? 0;
		const min = Math.floor(attackStat * 0.8);
		const max = Math.ceil(attackStat * 1.3);
		el.textContent = `${min}-${max}`;
	});
	document.querySelectorAll(".player-defense").forEach(el => { el.textContent = gameState.player.defenseStat ?? 0; });
	document.querySelectorAll(".player-hp").forEach(el => { el.textContent = gameState.player.currentHealth + "/" + gameState.player.maxHealth + " HP"; });
	document.getElementById("player-health-potions").textContent = gameState.player.healthPotions;
	document.getElementById("player-passive").textContent = "Passive: " + getPassiveDescription(gameState.player.passive);
	abilityCooldownIcon.src = "/images/" + gameState.player.abilityCooldown + "_6_Clock.png";
	playerHealthBar.style.height = ((gameState.player.currentHealth / gameState.player.maxHealth) * 100) + "%";
	let healthDifference = gameState.player.currentHealth - previousPlayerHealth;
	if (healthDifference != 0) {
		showDamageIndicator("player-damage-indicator", healthDifference);
		if (healthDifference < 0) {
			triggerShake('player-image');//Shake animation only when player takes damage (stops player image from shaking if healing)
		}
	}
	handleDeath(playerDisplay, gameState.player.currentHealth, playerHealthBar);
	respawnBtn.syncButtonState(gameState.player.currentHealth <= 0); //Enable respawn button if player is dead.
	if (gameState.player.currentHealth <= 0) {
		setTimeout(() => {
			showRespawnModal();
		}, 3000); //Wait 3 seconds for death animation to be done before showing the respawn modal.
	}
	else {
		hideRespawnModal();
	}
	previousPlayerHealth = gameState.player.currentHealth;//Update cached value for next check.
	activePopupWindow.textContent = getActiveDescription(gameState.player.class);//update active ability description. should only be needed once but whatever.
}

function refreshEnemyDisplay() {
	document.getElementById("enemy-name").textContent = gameState.enemy.name;
	document.getElementById("enemy-image").src = gameState.enemy.imageUrl;
	document.getElementById("enemy-image").alt = gameState.enemy.name;
	const enemyAttack = gameState.enemy.attack ?? 0;
	const minEnemy = Math.floor(enemyAttack * 0.8);
	const maxEnemy = Math.ceil(enemyAttack * 1.3);
	document.getElementById("enemy-attack").textContent = `${minEnemy}-${maxEnemy}`;
	document.getElementById("enemy-defense").textContent = gameState.enemy.defense;
	//document.getElementById("enemy-hp").textContent = gameState.enemy.currentHealth + "/" + gameState.enemy.maxHealth + " HP";
	enemyHealthBar.style.height = ((gameState.enemy.currentHealth / gameState.enemy.maxHealth) * 100) + "%";
	let healthDifference = gameState.enemy.currentHealth - previousEnemyHealth;
	if (healthDifference < 0) { //Only check for damage on enemy because they can't heal.
		showDamageIndicator("enemy-damage-indicator", healthDifference);
		triggerShake('enemy-image');
	}
	handleDeath(enemyDisplay, gameState.enemy.currentHealth, enemyHealthBar);
	previousEnemyHealth = gameState.enemy.currentHealth;//Update cached value for next check.
}

function refreshDialogBox() {
	dialogBox.innerHTML = "";
	gameState.listMessages.forEach((message) => {
		const logDiv = document.createElement("div");
		logDiv.textContent = message; //Instantly show cached messages
		dialogBox.appendChild(logDiv);
	});
	//Update message cache
	cachedMessages = gameState.listMessages;
	//Scroll to bottom to show new messages
	dialogBox.scrollTop = dialogBox.scrollHeight;
}

// Function to dynamically add shop purchase buttons
function refreshShop() {
	// Items currently in shop
	const shopItems = [
		{ name: "Darksteel Leggings", id: 1, price: 25 },
		{ name: "Radiant Glass Helm", id: 2, price: 65 },
		{ name: "The Pencil Blade", id: 3, price: 75 }
	];
	// Clear any existing buttons
	shopButtonDisplay.innerHTML = "";
	for (let i = 0; i < shopItems.length; i++) {
		const item = shopItems[i];
		const btn = document.createElement("button");
		btn.textContent = "Buy " + item.name + " for " + item.price + " gold?";
		btn.className = "pq-button";
		btn.style = "margin-bottom:2vmin; width: 100%;";
		// Attach the player action "purchase" with the item id as the value
		btn.attachPlayerAction("purchase", () => item.id);
		shopButtonDisplay.appendChild(btn);
		// Check if player already owns item by name. If so, disable its button in the shop
		if (gameState.player.items.find(i => i.name === item.name) != null) {
			btn.disabled = true;
		}
	}
}

function refreshActionBtnDisplay() {
	let combatEnableDelay = 1500;//Delayed to allow time for enemy turn/damage animations.
	if (gameState.inCombat && gameState.enemy.currentHealth == gameState.enemy.maxHealth) {
		combatEnableDelay = 0;//Combat just started so there is no animation to wait for.
	}
	attackBtn.syncButtonState(gameState.inCombat && gameState.isPlayersTurn && !gameState.isLocationComplete && gameState.player.currentHealth > 0, combatEnableDelay);
	healBtn.syncButtonState(gameState.inCombat && gameState.isPlayersTurn && !gameState.isLocationComplete && gameState.player.currentHealth > 0, combatEnableDelay);
	abilityBtn.syncButtonState(gameState.inCombat && gameState.isPlayersTurn && !gameState.isLocationComplete && gameState.player.currentHealth > 0 && gameState.player.abilityCooldown == 0, combatEnableDelay);
}

//------------------------ HELPER METHODS --------------------------------------------------------------------------------------------------------------

function handleDeath(characterDisplay, currentHealth, characterHealthBar) {
	if (!characterDisplay) {
		return;
	}
	if (currentHealth > 0) {
		characterDisplay.style.opacity = "1"; // Fade in
	}
	else {
		characterHealthBar.style.height = "0%";//Ensure health bar reaches zero before fading out
		characterDisplay.style.transition = "opacity 3.0s ease-out";
		characterDisplay.style.opacity = "0";
	}
}

function triggerShake(elementId) {
	let element = document.getElementById(elementId);
	if (!element) return;
	element.style.animation = "shake 0.5s ease-in-out";
	// Remove animation after it completes
	setTimeout(() => {
		element.style.animation = "";
	}, 500);
}

function showDamageIndicator(elementId, damageAmount) {
	const damageElement = document.getElementById(elementId);
	if (!damageElement) return;
	// Update text and make it visible
	if (damageAmount < 0) {
		damageElement.textContent = `${damageAmount}`;
		damageElement.style.color = 'red';
	}
	else {
		damageElement.textContent = `+${damageAmount}`;
		damageElement.style.color = '#39FF14';
	}
	damageElement.style.opacity = "1"; // Fully visible
	damageElement.style.transform = "translateY(0px)"; // Reset movement
	// Gradually fade out after a short delay
	setTimeout(() => {
		damageElement.style.opacity = "0"; // Fade out
		damageElement.style.transform = "translateY(-20px)"; // Move upwards
	}, 800); // Adjust time as needed
}

// Function to show the respawn modal
function showRespawnModal() {
	const respawnModal = document.getElementById('respawnModal');
	let modalInstance = bootstrap.Modal.getInstance(respawnModal);
	if (!modalInstance) {
		modalInstance = new bootstrap.Modal(document.getElementById('respawnModal'));
	}
	modalInstance.show();
}

// Function to hide the respawn modal
function hideRespawnModal() {
	const respawnModalElement = document.getElementById('respawnModal');
	const modalInstance = bootstrap.Modal.getInstance(respawnModalElement);
	if (modalInstance) {
		modalInstance.hide();
	}
}
// Function to show the Active Description Popup window
function showActivePopup() {
	activePopupWindow.style.visibility="visible"
}
//Function to hide the Active Description Popup window
function hideActivePopup() {
	activePopupWindow.style.visibility = "hidden"
}

//------------------------ EXTENSION METHODS --------------------------------------------------------------------------------------------------------------

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
HTMLButtonElement.prototype.attachPlayerAction = function (action, getActionValue = () => 0) {
	this.addEventListener("click", async () => {
		this.disabled = true;//Disable on click so it can't be spammed, refresh logic will enable or disable it as needed.
		await executePlayerAction(action, getActionValue());
	});
};

//Enables/disables a button according to the given condition. If condition is true, button is enabled, if not, it is disabled. Only updates if necessary to avoid UI flicker.
HTMLButtonElement.prototype.syncButtonState = function (condition, delay = 0) {
	if (condition && this.disabled) {
		//Button should be enabled but is currently disabled. Enable it.
		setTimeout(() => {
			this.disabled = false;
		}, delay);//Wait for delay number of miliseconds before button is renabled.
	}
	if (!condition && this.disabled == false) {
		//Button should not be enabled but is currently enabled. Disable it.
		this.disabled = true;
	}
};

