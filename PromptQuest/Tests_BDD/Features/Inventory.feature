Feature: Inventory

@scrum-98
Scenario: Display item details when an item is clicked
	Given I am on the inventory tab
	When I click on an item
	Then I should see a window with that item's title, image, and stats

@scrum-99
Scenario: Equiping items
	Given I am on the inventory tab
	When I click on an item
	And I click the equip button
	Then that item will leave the list of items
	And that item will move to the equipped item slot

@scrum-99
Scenario: Trying to equip no item
	Given I am on the inventory tab
	When I don't have an item selected
	And I click the equip button
	Then nothing should happen

	@scrum-123
Scenario: Add slots to items
	Given I am on the inventory tab
	When I click on an item
	Then the item should display its type