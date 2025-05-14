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
	internal class ShopStepDefinitions {
		private static IWebDriver webDriver;

		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
			PromptQuestTestMethods.StartNewGame(webDriver);
		}

		[When("I move to a shop node")]
		public void WhenIMoveToAShopNode() {
			PromptQuestTestMethods.MoveToRoom(webDriver, 13);
			PromptQuestTestMethods.ClearRoom(webDriver);
			// Open the menu
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "menu");
			// Click the map button to change to the map tab
			IWebElement mapButton = webDriver.FindElement(By.Id("map-btn"));
			mapButton.Click();
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "map");
			// Get the next node
			IWebElement nextNode = webDriver.FindElement(By.XPath("//button[contains(@class, 'map-node') and @data-node-id='16']"));
			// Attempt to click the next node
			nextNode.Click();
		}

		[Then("I should see a shop with items to buy")]
		public void ThenIShouldSeeAShopWithItemsToBuy() {
			// Find the shop display container by class name
			IWebElement shopDisplay = webDriver.FindElement(By.Id("shop-button-display"));
			// Assert that the shop display is enabled
			Assert.IsTrue(shopDisplay.Enabled, "The shop display is not enabled.");
			// Assert the items are present within the display
			IWebElement item1 = shopDisplay.FindElement(By.XPath(".//button[contains(text(), 'Darksteel Leggings')]"));
			Assert.IsTrue(item1.Enabled, "Item 1 is not displayed in the shop.");
			IWebElement item2 = shopDisplay.FindElement(By.XPath(".//button[contains(text(), 'Radiant Glass Helm')]"));
			Assert.IsTrue(item2.Enabled, "Item 2 is not displayed in the shop.");
			IWebElement item3 = shopDisplay.FindElement(By.XPath(".//button[contains(text(), 'The Pencil Blade')]"));
			Assert.IsTrue(item3.Enabled, "Item 3 is not displayed in the shop.");
		}

	}

}
