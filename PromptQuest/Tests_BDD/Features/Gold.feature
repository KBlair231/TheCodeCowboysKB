Feature: Gold

@scrum-79
Scenario: Gold Gained on Kill
	Given the user is on the game page
	When the user kills an enemy
	Then the user should gain gold