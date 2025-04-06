@scrum-92
Feature: Google Sign-In Integration
	Gives the user the ability to login using Google sign-in

Scenario: Redirect to Google Sign-In Page
	Given I am a user on the main menu
	When I click the log in button
	Then I should be redirected to the Google sign in page

Scenario: Successful Google Login
	Given I am on the Google sign in page
	When I sign in with my credentials
	Then I should see my Google profile image
	And I should see an option to log out
