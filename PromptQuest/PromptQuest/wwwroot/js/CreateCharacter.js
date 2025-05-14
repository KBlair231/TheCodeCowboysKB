let description;//Player prompt in character creation
let previewBtn;//The button used to preview the player icon
let createCharacterBtn;//The button used to create your character and start the game.
let playerImage;//A picture of the player's character at the top of the screen.
let playerImageData;//A dummy input on the form that the player's image data hitches a ride on up to the server.
let playerImageLoader;//A loading icon that shows while openAi is generating the iamge.
let passiveDescription; // description for passive abilities
document.addEventListener("DOMContentLoaded", async () => { //currently only included in CreateCharacter.cshtml
	playerImage = document.getElementById('player-image');
	playerImageData = document.getElementById('player-image-data');
	await fetchDefaultPlayerImage() //Load in default image and image data so that it's never blank/null.
	playerImageLoader = document.getElementById('player-image-loader');
	description = document.getElementById("description");
	previewButton = document.getElementById('preview-btn');
	passiveDescription = document.getElementById("passive-description");
	document.getElementById("passive-select").addEventListener("change", loadPassiveDescription, false);
	loadPassiveDescription();
	previewButton.addEventListener('click', async (e) => {
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
		const image = await response.json();
		//Log the result from the server for debugging.
		console.log(image);
		//Return the result as Json.
		playerImage.src = `data:image/png;base64,${image}`;
		playerImageData.value = image;//Make sure the image gets sent up with the form.
	}
	catch (error) { //Something went wrong.
		console.error("GET request error:", error);
	}
	playerImageLoader.style.display = "none";//Hide loading icon because it is done.
	playerImage.style.opacity = "1"; //Restore image opacity
	playerImage.style.pointerEvents = ""; //Re-enable interactions
}

//Fetches the default player image from a text file (it is stored there as a base64 string).
async function fetchDefaultPlayerImage() {
	if (playerImageData.value != "") {
		return; //Shouldn't happen. Just in case.
	}
	const response = await fetch('/images/DefaultPlayerImage.txt');
	const image = await response.text();
	playerImageData.value = image;//Store image in the data field
	playerImage.src = `data:image/png;base64,${image.trim()}`;//Show the user the default image.
	playerImage.style.display = "block";//Show image now that it is loaded, we don't want the user seeing it loading.
}


	function loadPassiveDescription() {
		let passiveSelect = document.getElementById("passive-select");
		let selectedPassive = passiveSelect.options[passiveSelect.selectedIndex].value;
		passiveDescription.innerHTML = getPassiveDescription(selectedPassive);

	}