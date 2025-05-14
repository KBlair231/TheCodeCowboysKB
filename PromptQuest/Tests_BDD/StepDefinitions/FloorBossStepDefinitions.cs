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
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
		}

		#region Boss Spawn Test

		[When("I move to the {int}th room")]
		public void WhenIMoveToTheThRoom(int p0) {
			// Move to the desired room
			PromptQuestTestMethods.MoveToRoom(webDriver, p0);
		}

		[Then("A boss should be spawned")]
		public void ThenABossShouldBeSpawned() {
			// Search for the boss' name Dark Orc Warlock
			IWebElement bossName = webDriver.FindElement(By.Id("enemy-name"));
			// Assert that the boss name displayed is Dark Orc Warlock
			Assert.IsTrue(bossName.Text == "Dark Orc Warlock", "The boss name is incorrect.");
		}
		#endregion
		#region Boss Defeat Test
		[Given("I am in the boss room")]
		public void GivenIAmInTheBossRoom() {
			PromptQuestTestMethods.MoveToRoom(webDriver, 18);
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
			// Click the inventory tab
			IWebElement inventoryTab = webDriver.FindElement(By.Id("inventory-btn"));
			inventoryTab.Click();
			// Wait for the menu modal to show
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "menu");
			// Check for an item with the DarkStaff image
			IWebElement bossItem = webDriver.FindElement(By.XPath("//img[@src='/images/DarkStaff.png']"));	
		}
		#endregion
	}
}
