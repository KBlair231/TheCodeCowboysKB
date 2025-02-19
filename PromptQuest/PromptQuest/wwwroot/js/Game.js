document.addEventListener("DOMContentLoaded", async function () {
	// Declare the global variable
	let combatState;

	// Start combat immediately on load for now
	await startCombat();

	// Function to start the combat loop
	async function startCombat() {
		let enemy = await fetchEnemy();

		combatState = {
			playerHealth: parseInt(document.querySelector("[data-player-current-health]").getAttribute("data-player-current-health")),
			playerMaxHealth: parseInt(document.querySelector("[data-player-current-health]").getAttribute("data-player-max-health")),
			enemyName: enemy.name,
			enemyHealth: enemy.currentHealth,
			enemyMaxHealth: enemy.maxHealth
		};

		// Tell player combat started
		addLogEntry("You were attacked by an " + enemy.name + "!");

		// Player starts first
		playerTurn();
	}

	// Function to handle the player's turn
	function playerTurn() {
		// Enable all combat buttons
		enableAttackButton();
	}

	// Function to handle the player's attack
	function playerAttack() {
		// Reduce enemy health
		combatState.enemyHealth -= 1;
		// Reflect damage on the screen
		updateHealthDisplay();
		// Tell the user what happened
		addLogEntry("You attacked the " + combatState.enemyName + " for 1 damage");

		// Check if combat is over
		if (combatState.enemyHealth <= 0) { // Combat is over
			// Let the user know
			addLogEntry("The enemy has been defeated!");
			// Disable combat buttons
			disableAttackButton();
			// Make the enemy disappear
			hideEnemyDisplay();
			return;
		}
		// Combat is not over
		enemyTurn(); // Start enemy's turn
	}

	// Function to handle the enemy's turn
	function enemyTurn() {
		// Disable all combat buttons
		disableAttackButton();
		// Enemy attacks by default
		enemyAttack();
	}

	function enemyAttack() {
		setTimeout(() => {// Add a nice delay to the enemy's turn
			// Reduce player health
			combatState.playerHealth -= 1;
			// Reflect damage on the screen
			updateHealthDisplay();
			// Tell the user what happened
			addLogEntry("The " + combatState.enemyName + " attacked you for 1 damage");

			// Check if combat is over
			if (combatState.playerHealth <= 0) { // Combat is over
				// Let the user know
				addLogEntry("You have been defeated!");
				// Make the enemy disappear
				hideEnemyDisplay();
				return;
			}
			// Combat is not over
			playerTurn(); // Start player's turn
		}, 1000); // Delay in milliseconds (1000ms = 1 second)
	}

	// Function to update health displays
	function updateHealthDisplay() {
		document.querySelector("[data-player-current-health]").textContent = combatState.playerHealth + "/" + combatState.playerMaxHealth + " HP";
		document.getElementById("enemy-hp").textContent = combatState.enemyHealth + "/" + combatState.enemyMaxHealth + " HP";
	}

	// Function to fetch enemy data from the server
	async function fetchEnemy() {
		try {
			const response = await fetch("/Game/GetEnemy");
			const enemy = await response.json();
			console.log("Enemy Loaded:", enemy);

			// Initialize enemy health values
			document.getElementById("enemy-name").textContent = enemy.name;
			document.getElementById("enemy-image").src = enemy.imageUrl;
			document.getElementById("enemy-image").alt = enemy.name;
			document.getElementById("enemy-hp").textContent = enemy.currentHealth + "/" + enemy.maxHealth + " HP";

			return enemy;
		} catch (error) {
			console.error("Error loading enemy:", error);
		}
	}

	// Helper functions
	// ----------------


	// Function to add log entries to the dialog box
	function addLogEntry(message) {
		const dialogBox = document.querySelector(".DialogBox");
		const logLimit = 5;

		// Clear dialog box if the number of log entries exceeds the limit
		if (dialogBox.childElementCount >= logLimit) {
			dialogBox.innerHTML = "";
		}
		const logDiv = document.createElement("div");
		logDiv.textContent = message;
		dialogBox.appendChild(logDiv);
	}

	// Function to disable the attack button and apply the disabled style
	function disableAttackButton() {
		const attackButton = document.getElementById("attack-btn");
		attackButton.removeEventListener("click", playerAttack);
		attackButton.disabled = true;
		attackButton.classList.add("PQButtonDisabled");
	}

	// Function to enable the attack button and remove the disabled style
	function enableAttackButton() {
		const attackButton = document.getElementById("attack-btn");
		attackButton.addEventListener("click", playerAttack);
		attackButton.disabled = false;
		attackButton.classList.remove("PQButtonDisabled");
	}

	// Function to hide the enemy display and attack button
	function hideEnemyDisplay() {
		const enemyDisplay = document.getElementById("enemy-display");
		const combatButtonsDisplay = document.getElementById("combat-buttons-display");
		enemyDisplay.style.display = "none";
		//Hide all combat buttons becuase we're not in combat
		combatButtonsDisplay.style.display = "none";
	}
});
