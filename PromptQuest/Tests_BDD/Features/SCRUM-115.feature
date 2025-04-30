Feature: Implement Status Effects

@scrum-115
Scenario: Apply status effect to enemy
	Given I am on the game page
	When I apply a status effect to an enemy
	Then the enemy should have the status effect applied