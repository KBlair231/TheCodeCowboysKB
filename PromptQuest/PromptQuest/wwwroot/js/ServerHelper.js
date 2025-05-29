// Cached GameState. Each server request returns a diff that is merged into this. Only get completely refreshed on page relod.
let gameState;
//Keep track of the player and enemy's previous health so we can use it to calculate damage indicators.
let previousPlayerHealth;
let previousEnemyHealth;

document.addEventListener("DOMContentLoaded", async function () {
	// Page loaded, get current game state and cache it. 
	await loadGame();
});

async function loadGame() {
	gameState = await sendGetRequest("/Game/GetGameState");
	if (gameState == undefined) {//new player or session expired
		window.location.href = "/"; //boot them to the Main Menu.
	}
	if (gameState.inTutorial) {
			StartTutorial();
	}
	//Cache player and enemy health values locally so we can use it to calculate damage indicator values.
	previousPlayerHealth = gameState.player.currentHealth;
	previousEnemyHealth = gameState.enemy.currentHealth;
	// Update display with loaded data.
	refreshDisplay();
	initializeHealthPopovers();
	let data = await sendGetRequest(`/Game/GetBackground?floor=${gameState.floor}`);
	document.getElementById("main-background-image").src = data;
}

// ----------------------------------- SERVER INTERACTION METHODS ----------------------------------------------------------------------

async function executePlayerAction(playerAction, actionValue = -1) {
	let url = `/Game/PlayerAction?playerAction=${playerAction}`;
	url += `&actionValue=${actionValue}`; // Append action value if provided.
	await sendPostRequest(url);
	//Enemy's turn now if it isn't the players turn and they are in combat and the enemy is still alive.
	if (gameState.isPlayersTurn == false && gameState.inCombat && gameState.enemy.currentHealth > 0) {
		//I don't like that this doesn't happen on the server... but it's the easisest way for the UI to show the turns separately (time and UI updates in between).
		//Maybe one day I'll find a way to move this over there.
		setTimeout(async () => {
			await sendPostRequest('/Game/EnemyAction');
		}, 2000); // 2000 milliseconds = 2 seconds
	}
}

async function sendGetRequest(endpoint) {
	try {
		const response = await fetch(endpoint);
		//Make sure response is valid.
		if (!response.ok) {
			throw new Error(`Server error: ${response.status} ${response.statusText}`);
		}
		const data = await response.json();
		//Log the result from the server for debugging.
		console.log(data);
		//Return the result as Json.
		return data;
	}
	catch (error) { //Something went wrong.
		console.error("GET request error:", error);
		return null;
	}
}

async function sendPostRequest(endpoint, payload = {}) {
	try {
		const response = await fetch(endpoint, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify(payload)
		});
		//Make sure response is valid.
		if (!response.ok) {
			throw new Error(`Server error: ${response.status} ${response.statusText}`);
		}
		const responseData = await response.json();
		//Log diff returned from server for debugging.
		console.log(responseData);
		//Merge response into local cache.
		mergeDiffIntoCache(gameState, responseData);
		//Refresh the UI to reflect the new changes.
		refreshDisplay();
	}
	catch (error) { //Something went wrong.
		console.error("Error in sendPostRequest:", error);
	}
}


//This method essentially does the opposite of what GameController.GenerateDiff does then merges it with the cached gameState
function mergeDiffIntoCache(target, json) {
	Object.keys(json).forEach(key => {
		if (key === "PlayerLocation") {
			let breakpoint = "here";
		}
		let newValue = json[key];
		let normalizedKey = key.charAt(0).toLowerCase() + key.slice(1);

		// Check if target has this key
		if (target.hasOwnProperty(normalizedKey)) {
			let existingValue = target[normalizedKey];

			// Handle collections: Replace entire list
			if (Array.isArray(newValue)) {
				target[normalizedKey] = newValue;
			}
			// Handle nested objects recursively
			else if (typeof newValue === "object" && newValue !== null) {
				if (typeof existingValue === "object" && existingValue !== null) {
					mergeDiffIntoCache(existingValue, newValue);
				} else {
					target[normalizedKey] = newValue;
				}
			}
			// Handle primitive values
			else {
				target[normalizedKey] = newValue;
			}
		} else {
			target[normalizedKey] = newValue;
		}
	});
}