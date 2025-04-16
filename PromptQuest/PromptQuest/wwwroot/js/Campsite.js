// Function to enable the campsite buttons and add their event handlers.
function enableCampsiteButtons() {
	// Enable Rest button.
	const restButton = document.getElementById("rest-btn");
	restButton.addEventListener("click", handleRestClick);
	restButton.disabled = false;
	restButton.classList.remove("PQButtonDisabled");
	// Enable Skip Rest button.
	const skipRestButton = document.getElementById("skip-rest-btn");
	skipRestButton.addEventListener("click", handleSkipRestClick);
	skipRestButton.disabled = false;
	skipRestButton.classList.remove("PQButtonDisabled");
}

// Function to disable the campsite buttons and add their event handlers.
function disableCampsiteButtons() {
	// Disable Rest button.
	const restButton = document.getElementById("rest-btn");
	restButton.removeEventListener("click", handleRestClick);
	restButton.disabled = true;
	restButton.classList.add("PQButtonDisabled");
	// Disable Skip Rest button.
	const skipRestButton = document.getElementById("skip-rest-btn");
	skipRestButton.removeEventListener("click", handleSkipRestClick);
	skipRestButton.disabled = true;
	skipRestButton.classList.add("PQButtonDisabled");
}

// Wrapper for Rest button click event handler
async function handleRestClick() {
	await executePlayerAction('rest');
	disableCampsiteButtons(); // Disable the buttons after resting
}

// Wrapper for Skip Rest button click event handler
async function handleSkipRestClick() {
	await executePlayerAction('skip-rest');
	disableCampsiteButtons(); // Disable the buttons after resting
}

// Function to show the Campsite UI.
function showCampsiteUI() {
	// Enable the campsite buttons
	enableCampsiteButtons();
	// Show campsite background.
	const campsiteBackgroundDisplay = document.getElementById("bg-image");
	campsiteBackgroundDisplay.style.visibility = "visible";
	// Show the campsite buttons display.
	const campsiteButtonsDisplay = document.getElementById("campsite-button-display");
	campsiteButtonsDisplay.style.visibility = "visible";
}

// Function to hide the Campsite UI.
function hideCampsiteUI() {
	// Just in case.
	disableCampsiteButtons();
	// Hide campsite background.
	const campsiteBackgroundDisplay = document.getElementById("bg-image");
	campsiteBackgroundDisplay.style.visibility = "hidden";
	// Hide the campsite buttons display.  
	const campsiteButtonsDisplay = document.getElementById("campsite-button-display");
	campsiteButtonsDisplay.style.visibility = "hidden";
}