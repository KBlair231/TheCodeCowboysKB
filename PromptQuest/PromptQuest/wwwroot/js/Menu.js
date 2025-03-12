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
