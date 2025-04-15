using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_BDD {
	static class PromptQuestTestMethods {

		/// <summary> Takes in an IWebDriver object and navigates it through the process of creating a basic character. Run this before testing something from the game view. </summary>
		public static void StartNewGame(IWebDriver webDriver, bool skipTutorial = true) {
			try {
				//Navigate to the Main menu page
				webDriver.Navigate().GoToUrl("https://localhost:7186/");

				//Click on the "New Adventure" button
				IWebElement newAdventureButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='New Adventure']"));
				newAdventureButton.Click();

				//Enter a name in the "Name" field
				IWebElement nameField = webDriver.FindElement(By.Id("name")); // Assuming the 'Name' field has an ID "name"
				WaitForElementToLoad(webDriver, "name");
				nameField.Clear();
				nameField.SendKeys("PlayerName");

				//Click on the "Start Adventure" button
				IWebElement startAdventureButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='Start Adventure']"));
				startAdventureButton.Click();

				if(skipTutorial) {
					//Wait for tutorial modal window to show
					WaitForElementToLoad(webDriver, "tutorialModal");
					//Click on the "Skip Tutorial" button.
					IWebElement skipTutorialButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='Skip Tutorial']"));
					skipTutorialButton.Click();
					//Wait for tutorial modal window to hide
					WaitForElementToLoad(webDriver, "tutorialModal");
				}
			}
			catch(NoSuchElementException ex) {
				Console.WriteLine($"Element not found: {ex.Message}");
			}
			catch(Exception ex) {
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}

		/// <summary> Takes in a IWebDriver object and the id of a modal element and then waits until the modal has opened. </summary>
		public static void WaitForElementToLoad(IWebDriver webDriver, string modalId) {
			//Set timeout time to 10 seconds.
			WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
			wait.Until(d => d.FindElement(By.Id(modalId)).Displayed);
		}
		/// <summary> Repeatedly clicks attack until the enemy is defeated. </summary>
		public static void ClearRoom(IWebDriver webDriver) {
			// Equip better weapon from inventory
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			// Wait for the menu modal to show
			WaitForElementToLoad(webDriver, "pq-modal");
			// Make sure the inventory tab is selected
			IWebElement inventoryTab = webDriver.FindElement(By.Id("inventory-button"));
			inventoryTab.Click();
			// Click weapon
			IWebElement weaponSlot = webDriver.FindElement(By.Id("inventory-slot-4"));
			weaponSlot.Click();
			// Click Equip Item
			IWebElement equipButton = webDriver.FindElement(By.Id("equip-button"));
			equipButton.Click();
			// Close the menu
			IWebElement closeButton = webDriver.FindElement(By.Id("pq-modal-close"));
			closeButton.Click();
			// Click on the attack button until the enemy is dead.
			while (webDriver.FindElement(By.Id("enemy-display")).Displayed && webDriver.FindElement(By.Id("action-button-display")).Displayed) {
				IWebElement attackButton = webDriver.FindElement(By.Id("attack-btn"));
				if (int.Parse(webDriver.FindElement(By.Id("player-hp")).Text.Split("/")[0]) <= 10) {
					IWebElement healthPotionButton = webDriver.FindElement(By.Id("health-potion-btn"));
					healthPotionButton.Click();
				}
				attackButton.Click();
			}
		}

		/// <summary> Gets you to the boss room with full health/potions. </summary>
		public static void SkipToBoss(IWebDriver webDriver) {
			// Navigate to the game page
			webDriver.Navigate().GoToUrl("https://localhost:7186/Game/SkipToBoss");
			// Defeat the room's enemy
			ClearRoom(webDriver);
			// Click the menu button
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			// Wait for the menu modal to show
			WaitForElementToLoad(webDriver, "pq-modal");
			// Click the map tab
			IWebElement mapTab = webDriver.FindElement(By.Id("map-button"));
			mapTab.Click();
			// Wait for the map modal to show
			WaitForElementToLoad(webDriver, "map-tab");
			// Click the tenth room with attribute data-node-id="10"
			IWebElement tenthNode = webDriver.FindElement(By.XPath("//div[@data-node-id='10']"));
			tenthNode.Click();
			// Close the menu
			IWebElement closeButton = webDriver.FindElement(By.Id("pq-modal-close"));
			closeButton.Click();
			// Wait for the boss to spawn
			WaitForElementToLoad(webDriver, "attack-btn");
		}
	}
}
