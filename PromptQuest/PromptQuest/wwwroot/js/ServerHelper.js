// Cached GameState. Each server request returns a diff that is merged into this. Only get completely refreshed on page relod.
let gameState;

document.addEventListener("DOMContentLoaded", function () {
	// Page loaded, get current game state and cache it. 
	loadGame();
});

async function loadGame() {
	gameState = await sendGetRequest("/Game/GetGameState");
	// Check if the user doesn't have a character
	if (gameState.player.maxHealth == 0) {// new player or session expired (this should be changed at some point)
		window.location.href = "/"; // boot them to the Main Menu.
	}
	// Update display with loaded data.  
	refreshDisplay();
}

// ----------------------------------- SERVER INTERACTION METHODS ----------------------------------------------------------------------

async function executePlayerAction(playerAction) {
	await sendPostRequest(`/Game/PlayerAction?playerAction=${playerAction}`);
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