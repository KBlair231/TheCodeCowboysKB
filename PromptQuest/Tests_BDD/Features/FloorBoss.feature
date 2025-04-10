Feature: Floor Boss

The ability for the player to fight a boss at the end of the floor

@scrum-83
Scenario: Entering the boss room
	Given I beat the 9th room
	When I move to the 10th room 
	Then A boss should be spawned

Scenario: Defeating the boss
	Given I am in the boss room
	When I defeat the boss
	Then I should be given a boss item