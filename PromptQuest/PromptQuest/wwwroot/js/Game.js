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
			playerDefense: parseInt(document.querySelector("[data-player-defense]").getAttribute("data-player-defense")),
			playerHealthPotions: parseInt(document.querySelector("[data-player-health-potions]").getAttribute("data-player-health-potions")),
			playerAttack: parseInt(document.querySelector("[data-player-attack]").getAttribute("data-player-attack")),
			enemyName: enemy.name,
			enemyHealth: enemy.currentHealth,
			enemyAttack: enemy.attack,
			enemyMaxHealth: enemy.maxHealth,
			enemyDefense: enemy.defense
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
		enableHealthPotionButton();
	}

	// Function to handle the player's attack
	function playerAttack() {
		// Reduce enemy health
		let damage = combatState.playerAttack - combatState.enemyDefense;
		if (damage < 1) {
			damage = 1;
		}
		combatState.enemyHealth -= damage;
		// Reflect damage on the screen
		updateHealthDisplay();
		// Tell the user what happened
		addLogEntry("You attacked the " + combatState.enemyName + " for " + damage + " damage");

		// Check if combat is over
		if (combatState.enemyHealth <= 0) { // Combat is over
			// Let the user know
			addLogEntry("The enemy has been defeated!");
			// Disable combat buttons
			disableAttackButton();
			disableHealthPotionButton();
			// Make the enemy disappear
			hideEnemyDisplay();
			return;
		}
		// Combat is not over
		enemyTurn(); // Start enemy's turn
	}

	// Function to let the player use a health potion
	function playerUseHealthPotion() {
		// Check if player has no health potions
		if (combatState.playerHealthPotions <= 0) {
			addLogEntry("You have no Health Potions!");
			return;
		}
		else if (combatState.playerHealth == combatState.playerMaxHealth) { // Check if player is already at max hp; don't use potion and return
			addLogEntry("You are already at max health!");
			return;
		}
		else { // Increase player hp by 5
			// Reduce number of health potions
			combatState.playerHealthPotions -= 1;
			// Heal player
			combatState.playerHealth += 5;
			// Check if heal equals or exceeds max health
			if (combatState.playerHealth >= combatState.playerMaxHealth) {
				combatState.playerHealth = combatState.playerMaxHealth;
				addLogEntry("You healed to max!");
			}
			else {
				addLogEntry("You healed to " + combatState.playerHealth + " HP!");
			}
			// Reflect health on the screen
			updateHealthDisplay();
			// Reflect number of potions on the screen
			updateHealthPotionDisplay();
		}
	}

	// Function to handle the enemy's turn
	function enemyTurn() {
		// Disable all combat buttons
		disableAttackButton();
		disableHealthPotionButton();
		// Enemy attacks by default
		enemyAttack();
	}

	function enemyAttack() {
		setTimeout(() => {// Add a nice delay to the enemy's turn
			// Reduce player health
			let damage = combatState.enemyAttack - combatState.playerDefense;
			if (damage < 1) {
				damage = 1;
			}
			combatState.playerHealth -= damage;
			// Reflect damage on the screen
			updateHealthDisplay();
			// Tell the user what happened
			addLogEntry("The " + combatState.enemyName + " attacked you for " + damage + " damage");

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

	function updateHealthPotionDisplay() {
		document.querySelector("[data-player-health-potions]").textContent = combatState.playerHealthPotions;
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
			document.getElementById("enemy-attack").textContent = enemy.attack;
			document.getElementById("enemy-defense").textContent = enemy.defense;
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

	// Function to disable the health potion button and apply the disabled style
	function disableHealthPotionButton() {
		const healButton = document.getElementById("health-potion-btn");
		healButton.removeEventListener("click", playerUseHealthPotion);
		healButton.disabled = true;
		healButton.classList.add("PQButtonDisabled");
	}

	// Function to enable the health potion button and remove the disabled style
	function enableHealthPotionButton() {
		const healButton = document.getElementById("health-potion-btn");
		healButton.addEventListener("click", playerUseHealthPotion);
		healButton.disabled = false;
		healButton.classList.remove("PQButtonDisabled");
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
