Feature: Elite Enemy Encounters

	@scrum-116
	Scenario: Elite Enemy Encountered
		Given I am on the game page
		When I move to the 11th room 
		Then An elite should be spawned

	Scenario: Defeating the elite
		Given I am on the game page
		When I move to the 11th room
		And I defeat the elite
		Then I should be given an elite item