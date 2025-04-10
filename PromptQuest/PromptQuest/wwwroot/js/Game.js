// "Global" Variable to keep track of the game state locally so that functions don't have to be passed parameters all the time.
let gameState;
let tutorialflag;

document.addEventListener("DOMContentLoaded", function () {
	// Page loaded, get current game state and store it locally.  
	loadGame();
});

async function loadGame() {
	let response = await fetch("/Game/GetGameState");
	gameState = await response.json();
	// Check if the user doesn't have a character
	if (gameState.player == null) {// new player or session expired
		window.location.href = "/"; // boot them to the Main Menu.
	}
	let flagresponse = await fetch("/Game/IsTutorial"); // get the tutorial flag
	tutorialflag = await flagresponse.json()//if in the tutorial: start the tutorial
	if (tutorialflag) {
		startTutorial()
	}
	// Update display with loaded data.  
	updateDisplay(); 
	updateMap();
	if (gameState.isPlayersTurn == false) {
		executeEnemyAction();
	}
}

// Function that makes an ajax call telling the game engine to process the player's action, then updates the local gameState variable with the response.  
async function executePlayerAction(action) {
	await $.ajax({
		url: '/Game/PlayerAction',
		type: 'POST',
		data: { action: action },
		success: function (response) {
			let actionResult = response;
			console.log('Player action (' + action + ') executed successfully:', actionResult);
			updateLocalGameState(actionResult); // Update local gameState variable with whatever the action changed.  
			addLogEntry(actionResult.message);
			updateDisplay() // Update screen to show whatever the action changed.
		},
		error: function (xhr, status, error) {
			console.error('Error executing player action (' + action + '):', error);
		}
	});
	if (gameState.isPlayersTurn == false && gameState.inCombat) {
		disableCombatButtons(); // They are, but it's not the player's turn anymore
		// Add a small delay so that the enemy's turn takes time.  
		setTimeout(async () => {
			await executeEnemyAction(); // The enemy action is determined server side.
		}, 1000);
		return;
	}
	// No need to enable combat buttons here because they are already enabled to allow the player to make this action.
}

// Function that makes an ajax call telling the game engine to process the enemy's action, then updates the local gameState variable with the response.  
async function executeEnemyAction() {
	await $.ajax({
		url: '/Game/EnemyAction',
		type: 'POST',
		success: function (response) {
			let actionResult = response;
			console.log('Enemy action executed successfully:', actionResult);
			updateLocalGameState(actionResult); // Update local gameState variable with whatever the action changed.  
			addLogEntry(actionResult.message);
			updateDisplay(); // Update screen to show whatever the action changed.
		},
		error: function (xhr, status, error) {
			console.error('Error executing enemy action:', error);
		}
	});
	if (gameState.isPlayersTurn == false && gameState.inCombat) {
		executeEnemyAction(); // Enemy continues its turn.
		return;
	}
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
	// Update player location
	gameState.playerLocation = actionResult.playerLocation;
	// Update isLocationComplete
	gameState.isLocationComplete = actionResult.isLocationComplete;
	//Update player ATK with item
	gameState.player.item.atk = actionResult.playerItemATK;
	//Update player DEF with item
	gameState.player.item.def = actionResult.playerItemDEF;
	// Log the updated gameState for debugging.
	console.log('Updated local gameState:', gameState);

}

// Function to update the player's display  
function updateDisplay() {
	// Update Player display.
	document.querySelectorAll(".player-name").forEach(el => { el.textContent = gameState.player.name; });
	document.querySelectorAll(".player-image").forEach(el => { el.src = "/images/" + gameState.player.class + ".png"; }); // Placeholder image for now.
	document.querySelectorAll(".player-image").forEach(el => { el.alt = gameState.player.name; });
	document.querySelectorAll(".player-attack").forEach(el => { el.textContent = gameState.player.attack+gameState.player.item.atk; });
	document.querySelectorAll(".player-defense").forEach(el => { el.textContent = gameState.player.defense + gameState.player.item.def; });
	document.querySelectorAll(".player-hp").forEach(el => { el.textContent = gameState.player.currentHealth + "/" + gameState.player.maxHealth + " HP"; });
	document.getElementById("player-health-potions").textContent = gameState.player.healthPotions;
	if (gameState.inCombat) {
		showCombatUI();
		// Update Enemy display.
		document.getElementById("enemy-name").textContent = gameState.enemy.name;
		document.getElementById("enemy-image").src = gameState.enemy.imageUrl;
		document.getElementById("enemy-image").alt = gameState.enemy.name;
		document.getElementById("enemy-attack").textContent = gameState.enemy.attack;
		document.getElementById("enemy-defense").textContent = gameState.enemy.defense;
		document.getElementById("enemy-hp").textContent = gameState.enemy.currentHealth + "/" + gameState.enemy.maxHealth + " HP";
	}
	else {
		hideCombatUI();
	}
	if (gameState.isPlayersTurn) {
		enableCombatButtons();
	}
	else {
		disableCombatButtons();
	}
}

// Function to add log entries to the dialog box.  
function addLogEntry(message) {
	const dialogBox = document.querySelector(".dialog-box");
	const logLimit = 5;
	if (dialogBox.childElementCount >= logLimit) {
		dialogBox.innerHTML = "";
	}
	const logDiv = document.createElement("div");
	logDiv.textContent = message;
	dialogBox.appendChild(logDiv);
}

function clearDialogBox() {
	const dialogBox = document.querySelector(".dialog-box");
	dialogBox.innerHTML = "";
}
