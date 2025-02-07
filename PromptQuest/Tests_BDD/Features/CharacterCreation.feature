Feature: Character Creation

The ability for the player to create a character

@SomeTag
Scenario: Form Submission
	Given I am a user
	And I am on the character creation page
	When I click the Start Adventure button
	And all fields are valid
	Then then all form fields will be cleared
