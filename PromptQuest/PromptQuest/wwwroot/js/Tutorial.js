// "Global" tutorial variables
let tutorialModal;
let tutorialCounter;
const potionButton = document.getElementById("health-potion-btn");
const attackButton = document.getElementById("attack-btn");
async function startTutorial() {
	await respawnPlayer()
	await spawnNewEnemy()
	tutorialModal = new bootstrap.Modal(document.getElementById('tutorialModal'));// set tutorialModal to the right modal
	tutorialCounter = 1
	potionButton.style.visibility = "hidden";
	showTutorialModal("Welcome to the Tutorial", "Hello and welcome to PromptQuest!");//show the tutorial
	disableCombatButtons();
	$("#tutorialModal").on('hidden.bs.modal', function () {//increments the 'steps' of the tutorial when the modal is closed
		tutorialIncrement();//the increment function will add and remove listenrs, hide elements, and keep track of which portion of the tutorial they are on.
		
	})
}
function showTutorialModal(title, message) {
	let textbox = document.getElementById('tutorialText');
	textbox.textContent = message;
	let titlebox = document.getElementById('tutorialModalLabel');
	titlebox.textContent = title;
	
	tutorialModal.show();
}
function tutorialIncrement() {
	
	if (tutorialCounter == 1) {
		enableCombatButtons()
		showTutorialModal("Tutorial 1/5: Attacking","Press the Attack button to attack the enemy, they will take an action after you.")
	}
	
	if (tutorialCounter == 2) {
		attackButton.addEventListener("click", tutorialStep2);
	}

	if (tutorialCounter == 3) {
		attackButton.removeEventListener("click", tutorialStep2);
		attackButton.addEventListener("click", tutorialStep3);
	}
	if (tutorialCounter == 4) {
		attackButton.removeEventListener("click", tutorialStep3);
		attackButton.addEventListener("click", tutorialStep4);
	}
	if (tutorialCounter == 5) {
		attackButton.removeEventListener("click", tutorialStep4);
		
		potionButton.addEventListener("click", tutorialStep5);
	}
	if (tutorialCounter == 6) {
		potionButton.removeEventListener("click", tutorialStep5);
		
	}
	tutorialCounter += 1;
}

function tutorialStep2() {
	showTutorialModal("Tutorial 2/5: Attacking part 2", "Attacks deal damage equal to the attacker's Attack stat, minus the defender's Defense stat. These are represented by the sword and shieild icons next to character portraits.")
}

function tutorialStep3() {
	showTutorialModal("Tutorial 3/5: Defense", "Damage can be reduced with Defense to a minimum of 1, so you will always take or deal 1 damage per attack.")

}

function tutorialStep4() {
	potionButton.style.visibility = "visible";
	attackButton.style.visibility = "hidden";
	showTutorialModal("Tutorial 4/5: Potions", "You can use Potions to heal. They restore 5 health, and can't heal you past your maximum HP. You only have a limited supply, so be careful!")

}

async function tutorialStep5() {
	showTutorialModal("Tutorial 5/5: The End!", "The Tutorial has ended, good luck!")
	endTutorial()
}

async function endTutorial() {
	attackButton.style.visibility = "visible";
	potionButton.style.visibility = "visible";
	attackButton.removeEventListener("click", tutorialStep2);
	attackButton.removeEventListener("click", tutorialStep3);
	attackButton.removeEventListener("click", tutorialStep4);
	attackButton.removeEventListener("click", tutorialStep5);
	tutorialCounter = 100;
	tutorialModal.hide();
	if (gameState.isPlayersTurn == true) {
		enableCombatButtons();// Start player's turn.
	}
	await fetch("/Game/EndTutorial", { method: "POST" });
}
//----------- Helper Functions - End -----------------------------------------------------------------------------------------------------------