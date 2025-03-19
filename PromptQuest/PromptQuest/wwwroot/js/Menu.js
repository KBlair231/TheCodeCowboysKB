// Global Variables
let tabCurrent;

document.addEventListener("DOMContentLoaded", async function () {
	// Get modal elements
	const modal = document.getElementById("pq-modal");
	const openModalButton = document.getElementById("pq-modal-open");
	const closeModalButton = document.getElementById("pq-modal-close");
	// Open modal
	openModalButton.addEventListener("click", () => {
		modal.style.display = "block";
		switchTab(tabCurrent);
	});
	// Close modal
	closeModalButton.addEventListener("click", () => {
		modal.style.display = "none";
	});
	// Close modal when clicking outside the modal content
	window.addEventListener("click", (event) => {
		if (event.target === modal) {
			modal.style.display = "none";
		}
	});
	// Inventory is the default tab
	switchTab('inventory');
});

// Function to switch between tabs in the modal
function switchTab(tabName) {
	// Update tabCurrent
	tabCurrent = tabName;
	// Hide all tabs
	document.querySelectorAll('.tab-content').forEach(tab => {
		tab.classList.remove('show-tab');
	});
	// Clear all selected tab highlights
	document.querySelectorAll('.pq-tab').forEach(tab => {
		tab.classList.remove('pq-tab-current');
	});
	// Highlight the selected tab
	document.getElementById(tabName +'-button').classList.add('pq-tab-current');
	// Show the selected tab
	document.getElementById(tabName + '-tab').classList.add('active-tab');
	if (tabName === 'inventory') {
		// Load items into the inventory tab
		LoadItems();
	}
	document.getElementById(tabName + '-tab').classList.add('show-tab');
	if (tabName === 'map') {
		updateMap();
	}
}