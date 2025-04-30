Feature: Player Ability

active abilities
@scrum-104
Scenario: Warrior Ability functionality
	Given the user is in combat
	And the user has the "Warrior" class
	When the user performs an "ability"
	Then the enemy should receive damage equal to the user's attack times 2 minus enemy defense
@scrum-104
Scenario: Warrior Ability disables after use
	Given the user is in combat
	And the user has the "Warrior" class
	When the user performs an "ability"
	Then the ability cooldown should be set to 3
@scrum-104
Scenario: Warrior Ability cooldown decreases on attack
	Given the user is in combat
	And the user has the "Warrior" class
	And the ability has a cooldown of 3
	When the user performs an "attack"
	Then the ability cooldown should be set to 2
@scrum-104
Scenario: Warrior Ability cooldown resets on combat starting
	Given the user is in combat
	And the user has the "Warrior" class
	And the ability has a cooldown of 3
	When the user enters combat
	Then the ability cooldown should be set to 0
@scrum-106
Scenario: Mage Ability functionality
	Given the user is in combat
	And the user has the "Mage" class
	When the user performs an "ability"
	Then the ability cooldown should be set to 4
@scrum-106
Scenario: Mage Ability cooldown decreases on attack
	Given the user is in combat
	And the user has the "Mage" class
	And the ability has a cooldown of 3
	When the user performs an "attack"
	Then the ability cooldown should be set to 2
@scrum-106
Scenario: Mage Ability cooldown resets on combat starting
	Given the user is in combat
	And the user has the "Mage" class
	And the ability has a cooldown of 3
	When the user enters combat
	Then the ability cooldown should be set to 0
@defenseBuff(implicit)
Scenario: Defense Buff resets on combat starting
	Given the user is in combat
	And the user has a defense buff of 6
	When the user enters combat
	Then the defense buff should be set to 0
@scrum-106
Scenario: Defense Buff on Mage Ability
	Given the user is in combat
	And the user has the "Mage" class
	When the user performs an "ability"
	Then the defense buff should be set to 6
@defenseBuff(implicit)
Scenario: Defense Buff applies to enemy attack
	Given the user is in combat
	And the user has a defense buff of 6
	When the user is attacked
	Then the damage received should be reduced by the defense buff
@defenseBuff(implicit)
	Scenario: Defense Buff resets on enemy attack
	Given the user is in combat
	And the user has a defense buff of 6
	When the user is attacked
	Then the defense buff should be set to 0