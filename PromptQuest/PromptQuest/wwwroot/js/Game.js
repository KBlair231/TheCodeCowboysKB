document.addEventListener("DOMContentLoaded", function () {
	fetch("/Game/GetEnemy")
		.then(response => response.json())
		.then(enemy => {
			console.log("Enemy Loaded:", enemy);

			document.getElementById("enemy-name").textContent = enemy.name;
			document.getElementById("enemy-image").src = enemy.imageUrl;
			document.getElementById("enemy-image").alt = enemy.name;
		})
		.catch(error => console.error("Error loading enemy:", error));
});