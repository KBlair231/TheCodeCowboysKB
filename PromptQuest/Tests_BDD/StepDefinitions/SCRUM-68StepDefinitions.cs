using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tests_BDD.StepDefinitions;

namespace Tests_BDD {

	[Binding]
	public class SCRUM68DisplaySteps {
		private static IWebDriver webDriver;

		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
			PromptQuestTestMethods.StartNewGame(webDriver);
		}

		[Given(@"I am on the inventory screen")]
		public void GivenIAmOnTheInventoryScreen() {
			// Navigate to the inventory screen in the application
			IWebElement menuButton = webDriver.FindElement(By.Id("open-inventory-btn"));
			menuButton.Click();
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "menu");
		}

		[Given("I have defeated the enemy")]
		public void GivenIHaveDefeatedTheEnemy() {
			// Click the attack button until the enemy is defeated
			PromptQuestTestMethods.ClearRoom(webDriver);
		}

		[When(@"I click the map button")]
		public void WhenIClickTheMapButton() {
			// Click the map button to change to the map tab
			IWebElement mapButton = webDriver.FindElement(By.Id("open-map-btn"));
			mapButton.Click();
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "map");
		}

		[When("I move to the next node")]
		public void WhenIMoveToTheNextNode() {
			// Get the next node
			IWebElement nextNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node') and @data-node-id='2']"));
			// Attempt to click the next node
			nextNode.Click();
		}

		[When("I close the menu")]
		public void WhenICloseTheMenu() {
			IWebElement closeButton = webDriver.FindElement(By.Id("close-map-btn"));
			closeButton.Click();
		}

		[Then(@"I should see the map with the current location highlighted")]
		public void ThenIShouldSeeTheMapWithTheCurrentLocationHighlighted() {
			// Find the map container by ID
			IWebElement mapContainer = webDriver.FindElement(By.Id("map-container"));
			// Assert that the map container is there
			Assert.IsTrue(mapContainer.Enabled, "The map is not enabled.");
			// Find the current node on the map
			IWebElement currentNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node') and contains(@class, 'map-node-current') and contains(@class, 'map-node-disabled') and @data-node-id='1']"));
			// Assert that the current node is displayed on the map
			Assert.IsTrue(currentNode.Displayed, "The current node is not displayed");
		}

		[Then(@"I should not be able to move to the next node")]
		public void ThenIShouldNotBeAbleToMoveToTheNextNode() {
			// Get the next node
			IWebElement nextNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node') and contains(@class, 'map-node-disabled') and @data-node-id='2']"));
			// Capture the initial state of the next node
			string initialClass = nextNode.GetAttribute("class");
			string initialDataNodeId = nextNode.GetAttribute("data-node-id");
			bool initialDisplayed = nextNode.Displayed;
			bool initialEnabled = nextNode.Enabled;
			// Attempt to click the next node
			nextNode.Click();
			// Capture the state of the next node after the click attempt
			string finalClass = nextNode.GetAttribute("class");
			string finalDataNodeId = nextNode.GetAttribute("data-node-id");
			bool finalDisplayed = nextNode.Displayed;
			bool finalEnabled = nextNode.Enabled;
			// Assert that the state of the next node has not changed
			Assert.AreEqual(initialClass, finalClass, "The class attribute of the next node has changed.");
			Assert.AreEqual(initialDataNodeId, finalDataNodeId, "The data-node-id attribute of the next node has changed.");
			Assert.AreEqual(initialDisplayed, finalDisplayed, "The displayed state of the next node has changed.");
			Assert.AreEqual(initialEnabled, finalEnabled, "The enabled state of the next node has changed.");
		}

		[Then("I should be able to move to the next node")]
		public void ThenIShouldBeAbleToMoveToTheNextNode() {
			// Get the next node
			IWebElement nextNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node-enabled') and @data-node-id='2']"));
			// Attempt to click the next node
			nextNode.Click();
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "map");
			// Re-locate the next node to avoid StaleElementReferenceException
			IWebElement updatedNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node') and @data-node-id='2']"));
			// Assert that node 2 is now the current node (We have successfully moved to the next node)
			string nodeClass = updatedNode.GetAttribute("class");
			Assert.IsTrue(nodeClass.Contains("map-node-current"), $"The node with data-node-id='2' does not have the 'map-node-current' class. Actual class: {nodeClass}");
		}

		[Then("I should see a new enemy")]
		public void ThenIShouldSeeANewEnemy() {
			// Get the enemy's health
			IWebElement enemyHealthElement = webDriver.FindElement(By.Id("enemy-hp"));
			// Get the text content of the enemy's health element
			string enemyHealthText = enemyHealthElement.Text;
			// Split the string and parse the first number
			string[] parts = enemyHealthText.Split('/');
			if(parts.Length > 0 && int.TryParse(parts[0], out int enemyHealth)) {
				// Assert that the enemy's health is greater than 0
				Assert.IsTrue(enemyHealth > 0, "The enemy's health is not greater than 0, indicating that it is not a new enemy.");
			}
			else {
				Assert.Fail("Failed to parse enemy health from the string.");
			}
		}
	}
}
