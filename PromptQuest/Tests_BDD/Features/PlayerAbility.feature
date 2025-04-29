Feature: Player Ability

active abilities
@scrum-104

Scenario: Warrior Ability functionality
	Given the user is in combat
	And the user has the "Warrior" class
	When the user performs an "ability"
	Then the enemy should receive damage equal to the user's attack times 2 minus enemy defense

Scenario: Warrior Ability disables after use
	Given the user is in combat
	And the user has the "Warrior" class
	When the user performs an "ability"
	Then the ability cooldown should be set to 3

Scenario: Warrior Ability cooldown decreases on attack
	Given the user is in combat
	And the user has the "Warrior" class
	And the ability has a cooldown of 3
	When the user performs an "attack"
	Then the ability cooldown should be set to 2

Scenario: Warrior Ability cooldown resets on combat starting
	Given the user is in combat
	And the user has the "Warrior" class
	And the ability has a cooldown of 3
	When the user enters combat
	Then the ability cooldown should be set to 0