document.addEventListener("DOMContentLoaded", async function () {
	// Get modal elements
	const modal = document.getElementById("pq-modal");
	const openModalButton = document.getElementById("pq-modal-open");
	const closeModalButton = document.getElementById("pq-modal-close");

	// Open modal
	openModalButton.addEventListener("click", () => {
		modal.style.display = "block";
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
});

// Function to switch between tabs in the modal
function switchTab(tabName) {
	// Hide all tabs
	document.querySelectorAll('.tab-content').forEach(tab => {
		tab.classList.remove('active-tab');
	});

	// Show the selected tab
	document.getElementById(tabName + '-tab').classList.add('active-tab');
	if (tabName === 'inventory') {
		// Load items into the inventory tab
		LoadItems();
	}
}

// Set Inventory as default active tab on page load
document.addEventListener("DOMContentLoaded", function () {
	switchTab('inventory');
});