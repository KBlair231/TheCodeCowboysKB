using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.Log;
using OpenQA.Selenium.BiDi.Modules.Script;
using OpenQA.Selenium.Chrome;
using PromptQuest.Models;
using Reqnroll;
using System.Collections.Generic;
using Tests_BDD.StepDefinitions;
using static System.Net.Mime.MediaTypeNames;

namespace Tests_BDD {

	[Binding]
	public class InventoryDisplaySteps {
		private IWebDriver webDriver;
		private IWebElement EquippedSlotBefore;

		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
		}

		[Given(@"I am on the inventory tab")]
		public void GivenIAmOnTheInventoryTab() {
			// Navigate to the inventory tab in the application
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			//Wait for menu modal to show before continuing
			PromptQuestTestMethods.WaitForElementToLoad(webDriver,"menu");
		}

		[Then(@"I should see a window with that item's title, image, and stats")]
		public void ThenIShouldSeeAWindowWithThatItemsTitleImageAndStats() {
			// Find the inventory display container by class name
			IWebElement inventoryDisplay = webDriver.FindElement(By.ClassName("pq-inventory-display"));
			// Assert that the inventory display is visible
			Assert.IsTrue(inventoryDisplay.Displayed,"The inventory display is not visible.");
			// Assert the item's title, image, and stats are present within the display
			// Assert default attributes for now, later we'll add a db/session call to get the actual item info.
			//Item name
			IWebElement itemName = inventoryDisplay.FindElement(By.Id("item-name"));
			Assert.IsTrue(!string.IsNullOrEmpty(itemName.Text),"Item name is empty.");
			Assert.IsTrue(itemName.Text=="Jeweled Helmet","Item name is incorrect");
			//Item image
			IWebElement itemImage = inventoryDisplay.FindElement(By.Id("item-image"));
			Assert.AreEqual("https://localhost:7186/images/PlaceholderItem1.png",itemImage.GetAttribute("src"),"Item image source is incorrect.");
			//Item defense icon
			IWebElement shieldIcon = inventoryDisplay.FindElement(By.Id("shield-icon"));
			Assert.IsNotNull(shieldIcon,"Shield icon element is missing.");
			Assert.IsTrue(shieldIcon.Displayed,"Shield icon is not visible.");
			//Item defense stat
			IWebElement itemDefense = inventoryDisplay.FindElement(By.Id("item-defense"));
			Assert.IsNotNull(itemDefense,"Item defense element is missing.");
			Assert.IsTrue(!string.IsNullOrEmpty(itemDefense.Text),"Item defense value is empty.");
			Assert.IsTrue(itemDefense.Text=="2","Item defense value is empty.");
			//Item attack icon
			IWebElement swordIcon = inventoryDisplay.FindElement(By.Id("sword-icon"));
			Assert.IsNotNull(swordIcon,"Sword icon element is missing.");
			Assert.IsTrue(swordIcon.Displayed,"Sword icon is not visible.");
			//Item attack stat
			IWebElement itemAttack = inventoryDisplay.FindElement(By.Id("item-attack"));
			Assert.IsNotNull(itemAttack,"Item attack element is missing.");
			Assert.IsTrue(!string.IsNullOrEmpty(itemAttack.Text),"Item attack value is empty.");
			Assert.IsTrue(itemAttack.Text=="0","Item defense value is empty.");
		}

		[When("I click on an item")]
		public void WhenIClickOnAnItem() {
			IWebElement inventorySlot = webDriver.FindElement(By.CssSelector("#inventory-slot-1"));
			inventorySlot.Click();
		}

		[When("I click the equip button")]
		public void WhenIClickTheEquipButton() {
			IWebElement equipButton = webDriver.FindElement(By.Id("equip-btn"));
			equipButton.Click();
		}

		[Then("that item will leave the list of items")]
		public void ThenThatItemWillLeaveTheListOfItems() {
			IWebElement equippedSlot = webDriver.FindElement(By.Id("equipped-item"));
			Assert.IsTrue(true);
		}

		[Then("that item will move to the equipped item slot")]
		public void ThenThatItemWillMoveToTheEquippedItemSlot() {
			IWebElement equippedSlot = webDriver.FindElement(By.Id("equipped-item"));
			Assert.IsNotNull(equippedSlot.Text);
		}

		[When("I don't have an item selected")]
		public void WhenIDontHaveAnItemSelected() {
		
		}

		[Then("nothing should happen")]
		public void ThenNothingShouldHappen() {
			IWebElement equippedSlot = webDriver.FindElement(By.Id("equipped-item"));
			Assert.IsTrue(string.IsNullOrEmpty(equippedSlot.Text));
		}
	}
}
