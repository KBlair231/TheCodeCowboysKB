Feature: Inventory

@scrum-98
Scenario: Display item details when an item is clicked
	Given I am on the inventory tab
	When I click on an item in the inventory
	Then I should see a window with that item's title, image, and stats
