// Function to enable the Event buttons and add their event handlers.
function enableEventButtons() {
	// Enable Accept button.
	const acceptButton = document.getElementById("accept-btn");
	acceptButton.addEventListener("click", handleAcceptClick);
	acceptButton.disabled = false;
	acceptButton.classList.remove("PQButtonDisabled");
	// Enable Deny button.
	const denyButton = document.getElementById("deny-btn");
	denyButton.addEventListener("click", handleDenyClick);
	denyButton.disabled = false;
	denyButton.classList.remove("PQButtonDisabled");
}

// Function to disable the Event buttons and add their event handlers.
function disableEventButtons() {
	// Enable Accept button.
	const acceptButton = document.getElementById("accept-btn");
	acceptButton.removeEventListener("click", handleAcceptClick);
	acceptButton.disabled = true;
	acceptButton.classList.add("PQButtonDisabled");
	// Enable Deny button.
	const denyButton = document.getElementById("deny-btn");
	denyButton.removeEventListener("click", handleDenyClick);
	denyButton.disabled = true;
	denyButton.classList.add("PQButtonDisabled");
}

// Wrapper for Accept button click event handler
async function handleAcceptClick() {
	await executePlayerAction('accept');
	disableEventButtons(); // Disable the buttons after resting
}

// Wrapper for Deny button click event handler
async function handleDenyClick() {
	await executePlayerAction('deny');
	disableEventButtons(); // Disable the buttons after resting
}

// Function to show the Event UI.
function showEventUI() {
	// Enable the event buttons
	enableEventButtons();
	// Show event background (eventually).
	// const campsiteBackgroundDisplay = document.getElementById("bg-image");
	// campsiteBackgroundDisplay.style.visibility = "visible";
	// Show the event buttons display.
	const eventButtonsDisplay = document.getElementById("event-button-display");
	eventButtonsDisplay.style.visibility = "visible";
}

// Function to hide the event UI.
function hideEventUI() {
	// Just in case.
	disableEventButtons();
	// Hide event background (eventually).
	// const campsiteBackgroundDisplay = document.getElementById("bg-image");
	// campsiteBackgroundDisplay.style.visibility = "hidden";
	// Hide the event buttons display.  
	const eventButtonsDisplay = document.getElementById("event-button-display");
	eventButtonsDisplay.style.visibility = "hidden";
}