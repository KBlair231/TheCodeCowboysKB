Feature: Floor Boss

The ability for the player to fight a boss at the end of the floor

@scrum-83
Scenario: Entering the boss room
	Given I am on the game page
	When I move to the 18th room 
	Then A boss should be spawned

Scenario: Defeating the boss
	Given I am on the game page
	When I move to the 18th room 
	When I defeat the boss
	Then I should be given a boss item