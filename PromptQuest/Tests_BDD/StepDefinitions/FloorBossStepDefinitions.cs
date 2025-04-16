using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Reqnroll;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Tests_BDD.StepDefinitions {
	[Binding]
	public class FloorBossStepDefinitions {
		private IWebDriver webDriver;

		[BeforeScenario]
		public void Setup() {
			// Initialize WebDriver before each scenario
			webDriver = new ChromeDriver();
			// Start a new game
			PromptQuestTestMethods.StartNewGame(webDriver, skipTutorial: true);
		}
		#region Boss Spawn Test
		[Given("I beat the {int}th room")]
		public void GivenIBeatTheThRoom(int p0) { 
			// Navigate to the game page
			webDriver.Navigate().GoToUrl("https://localhost:7186/Game/SkipToBoss");
			// Defeat the room's enemy
			PromptQuestTestMethods.ClearRoom(webDriver);
		}

		[When("I move to the {int}th room")]
		public void WhenIMoveToTheThRoom(int p0) { 
			// Click the menu button
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			// Wait for the menu modal to show
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "pq-modal");
			// Click the map tab
			IWebElement mapTab = webDriver.FindElement(By.Id("map-button"));
			mapTab.Click();
			// Wait for the map modal to show
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "map-tab");
			// Click the tenth room with attribute data-node-id="10"
			IWebElement tenthNode = webDriver.FindElement(By.XPath("//div[@data-node-id='10']"));
			tenthNode.Click();
		}

		[Then("A boss should be spawned")]
		public void ThenABossShouldBeSpawned() {
			// Close the menu
			IWebElement closeButton = webDriver.FindElement(By.Id("pq-modal-close"));
			closeButton.Click();
			// Wait for the boss to spawn
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "attack-btn");
			// Search for the boss' name Dark Orc Warlock
			IWebElement bossName = webDriver.FindElement(By.Id("enemy-name"));
			// Assert that the boss name displayed is Dark Orc Warlock
			Assert.IsTrue(bossName.Text == "Dark Orc Warlock", "The boss name is incorrect.");
		}
		#endregion
		#region Boss Defeat Test
		[Given("I am in the boss room")]
		public void GivenIAmInTheBossRoom() {
			PromptQuestTestMethods.SkipToBoss(webDriver);
		}

		[When("I defeat the boss")]
		public void WhenIDefeatTheBoss() {
			PromptQuestTestMethods.ClearRoom(webDriver);
		}

		[Then("I should be given a boss item")]
		public void ThenIShouldBeGivenABossItem() {
			// Open Inventory
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			// Wait for the menu modal to show
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "pq-modal");
			// Check for an item with the DarkStaff image
			IWebElement bossItem = webDriver.FindElement(By.XPath("//img[@src='/images/DarkStaff.png']"));	
		}
		#endregion
		[AfterScenario]
		public void TearDown() {
			if (webDriver != null) {
				webDriver.Quit(); // Ensure the browser is closed
				webDriver?.Dispose(); // Clean up unmanaged resources
			}
		}
	}
}
