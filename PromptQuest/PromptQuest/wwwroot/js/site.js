// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code. 
let description;//player prompt in character creation
let previewButton;//the button used to preview the player icon
document.addEventListener("DOMContentLoaded", () => { //currently only included in CreateCharacter.cshtml
	description = document.getElementById("description");
	previewButton = document.getElementById('previewButton')
	previewButton.addEventListener('click', preview);
});
function preview() {// called by the non-button button
	//description.value = "test";//test line to make sure that the correct element is being looked at
}