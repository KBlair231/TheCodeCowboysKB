//This file got hit by the refactor so it isn't complete garbage now, but don't worry, the tutorial is still just as awful as it was before. It simply wasn't worth the hassle of improving it.
//To whoever has to update the tutorial: the bootstrap modal JQuery crap has got to go and we should be completely blocking off any actions except what the user should be clicking. 
// "Global" tutorial variables
let tutorialModal;
let skipTutorialBtn;
let stepCur = 0;

document.addEventListener("DOMContentLoaded", async () => {
	const isTutorial = await sendGetRequest("/Game/IsTutorial"); // get the tutorial flag
	if (isTutorial) {
		//Setup
		tutorialModal = new bootstrap.Modal(document.getElementById("tutorial-popup"));
		$("#tutorial-popup").on('hidden.bs.modal', function () {
			if (stepCur == 0) {//Progress automatically for first step because it don't require user interaction.
				progressToNextStep();
			}
		})
		skipTutorialBtn = document.getElementById("skip-tutorial-btn");
		skipTutorialBtn.addEventListener("click", skipTutorial);
		showMessage("Welcome to the Tutorial", "Hello and welcome to PromptQuest!");//show the tutorial
		tutorialModal.show();
	}
});

async function SetupCurStep() {
	switch (stepCur) {
		case 1:
			showMessage("Tutorial 1/5: Attacking", "Press the Attack button to attack the enemy, they will take an action after you.");
			attackBtn.addEventListener("click", progressToNextStep);//Move to next step when they attack like they were told.
			break;
		case 2:
			showMessage("Tutorial 2/5: Attacking part 2", "Attacks deal damage equal to the attacker's Attack stat, minus the defender's Defense stat. These are represented by the sword and shieild icons next to character portraits.")
			break;
		case 3:
			showMessage("Tutorial 3/5: Defense", "Damage can be reduced with Defense to a minimum of 1, so you will always take or deal 1 damage per attack.")
			break;
		case 4:
			showMessage("Tutorial 4/5: Potions", "You can use Potions to heal. They restore 5 health, and can't heal you past your maximum HP. You only have a limited supply, so be careful!")
			attackBtn.removeEventListener("click", progressToNextStep);//Attacking no longer progresses tutorial
			healBtn.addEventListener("click", progressToNextStep);//Move to next step when the use the heal button like they were told.
			break;
		case 5:
			showMessage("Tutorial 5/5: The End!", "The Tutorial has ended, good luck!")
			await sendPostRequest("/Game/EndTutorial");//Tell server we're done.
			return;//End recursion here, tutorial over.
		default:
			console.log("Error in tutorial loop");
			break;
	}
}

//Simple event handlers, but needs to be a functions so they can be removed after it isn't needed anymore.
function progressToNextStep() {
	stepCur++;
	SetupCurStep();
}

function skipTutorial() {
	sendPostRequest("/Game/EndTutorial");
	stepCur = -1;//So that the next line doesn't trigger the modal hide event (that needs to be removed but it's a very low priority).
	tutorialModal.hide();
}

function showMessage(title, message) {
	let textbox = document.getElementById('tutorialText');
	textbox.textContent = message;
	let titlebox = document.getElementById('tutorialModalLabel');
	titlebox.textContent = title;
	tutorialModal.show();
}