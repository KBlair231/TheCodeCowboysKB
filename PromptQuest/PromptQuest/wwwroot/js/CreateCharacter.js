let description;//Player prompt in character creation
let previewBtn;//The button used to preview the player icon
let createCharacterBtn;//The button used to create your character and start the game.
let playerImage;//A picture of the player's character at the top of the screen.
let playerImageLoader;//A loading icon that shows while openAi is generating the iamge.
let passiveDescription; // description for passive abilities
document.addEventListener("DOMContentLoaded", async () => { //currently only included in CreateCharacter.cshtml
	playerImage = document.getElementById('player-image');
	playerImage.src = `/Game/GetCharacterImage?isPreview=${true}`;
	playerImageLoader = document.getElementById('player-image-loader');
	description = document.getElementById('description');
	previewBtn = document.getElementById('preview-btn');
	createCharacterBtn = document.getElementById('create-character-btn');
	passiveDescription = document.getElementById("passive-description");
	document.getElementById("passive-select").addEventListener("change", loadPassiveDescription, false);
	loadPassiveDescription();
	previewBtn.addEventListener('click', async (e) => {
		e.preventDefault();//Stops the form from being submitted.
		previewBtn.disabled = true; //This button click costs Ben money, lets make damn sure it doesn't get spammed.
		createCharacterBtn.disabled = true;
		await generatePlayerImage(description.value);
		previewBtn.disabled = false;
		createCharacterBtn.disabled = false;
	});
	console.log(playerImage.src);
});

async function generatePlayerImage(prompt) {
	if (prompt === "") {
		return;
	}
	playerImageLoader.style.display = "block";//Show loading icon while we wait for the image to be made.
	playerImage.style.opacity = "0.5"; //Grey out the image
	playerImage.style.pointerEvents = "none"; //Prevent interaction with the image
	let endpoint = `/Game/CreateCharacterImage?prompt=${prompt}`;
	try {
		const response = await fetch(endpoint);
		//Make sure response is valid.
		if (!response.ok) {
			throw new Error(`Server error: ${response.status} ${response.statusText}`);
			alert("We had trouble bringing your character to life. Try again later or start your adventure with the current character image.");
		}
	}
	catch (error) { //Something went wrong.
		console.error("GET request error:", error);
	}
	//Let's force a reload because we created an image server side
	playerImage.src += "&refresh=" + new Date().getTime();
	// Wait for image to fully load before hiding the loader
	playerImage.onload = () => {
		playerImageLoader.style.display = "none"; // Hide loader
		playerImage.style.opacity = "1"; // Restore image opacity
		playerImage.style.pointerEvents = ""; // Re-enable interactions
	};
	// Handle errors (optional)
	playerImage.onerror = () => {
		alert("We had trouble bringing your character to life. Try again later or start your adventure with the current character image.");
		playerImageLoader.style.display = "none"; // Hide loader in case of failure
		playerImage.style.opacity = "1"; // Restore image opacity
		playerImage.style.pointerEvents = ""; // Re-enable interactions
	};
}

function loadPassiveDescription() {
	let passiveSelect = document.getElementById("passive-select");
	let selectedPassive = passiveSelect.options[passiveSelect.selectedIndex].value;
	passiveDescription.innerHTML = getPassiveDescription(selectedPassive);
}