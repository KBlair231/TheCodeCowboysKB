using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tests_BDD {

	[Binding]
	public class InventoryDisplaySteps {
		private IWebDriver webDriver;

		[BeforeScenario]
		public void Setup() {
			// Initialize WebDriver before each scenario
			webDriver = new ChromeDriver();
			// Start a new game
			PromptQuestTestMethods.StartNewGame(webDriver, skipTutorial:true);
		}

		[Given(@"I am on the inventory tab")]
		public void GivenIAmOnTheInventoryTab() {
			// Navigate to the inventory tab in the application
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
		}

		[When(@"I click on an item in the inventory")]
		public void WhenIClickOnAnItemInTheInventory() {
			// Click on the first inventory Item Slot (Should have something)
			IWebElement FirstInventoryItem = webDriver.FindElement(By.Id("inventory-slot-1"));
			FirstInventoryItem.Click();
			//Wait for menu modal to show before continuing
			PromptQuestTestMethods.WaitForModalToOpen(webDriver, "pq-modal");
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

		[AfterScenario]
		public void TearDown() {
			if(webDriver != null) {
				webDriver.Quit(); // Ensure the browser is closed
				webDriver?.Dispose(); // Clean up unmanaged resources
			}
		}
	}
}