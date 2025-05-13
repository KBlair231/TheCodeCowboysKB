document.addEventListener("DOMContentLoaded", async function () {
	const continueBtn = document.getElementById("continue-btn");
	//// Ask the server if the continue button should be enabled or not.
	//let response = await fetch("/Game/GetGameSaveStatus");
	//var isAuthenticated = await response.json();
	//// Enable or disable the submit button based on authentication status
	//if (!isAuthenticated) {
	//	continueBtn.disabled = true;
	//	continueBtn.classList.add("PQButtonDisabled");
	//	return;
	//}
	continueBtn.enabled = true;
	continueBtn.classList.remove("PQButtonDisabled");
});