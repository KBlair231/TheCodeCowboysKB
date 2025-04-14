@scrum-68
Feature: Add Capability to Travel
	Allows the user to travel to different locations in the game

Scenario: Check Map Status
	Given I am on the inventory screen
	When I click the map button
	Then I should see the map with the current location highlighted

Scenario: Check Next Map Node Can't Be Used Initially
	Given I am on the inventory screen
	When I click the map button
	Then I should not be able to move to the next node

Scenario: Check Next Map Node Can Be Used After Defeating Enemy
	Given I have defeated the enemy
	And I am on the inventory screen
	When I click the map button
	Then I should be able to move to the next node

Scenario: Check New Enemy Will Spawn After Moving to Next Node
	Given I have defeated the enemy
	And I am on the inventory screen
	When I click the map button
	And I move to the next node
	And I close the menu
	Then I should see a new enemy