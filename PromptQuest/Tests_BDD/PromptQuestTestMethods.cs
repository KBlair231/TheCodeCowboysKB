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
					WaitForElementToLoad(webDriver, "tutorial-popup");
					//Click on the "Skip Tutorial" button.
					IWebElement skipTutorialButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='Skip Tutorial']"));
					skipTutorialButton.Click();
					//Wait for tutorial modal window to hide
					WaitForElementToLoad(webDriver, "tutorial-popup");
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
			EquipItem(webDriver, 5);
			EquipItem(webDriver, 1);
			EquipItem(webDriver, 2);
			EquipItem(webDriver, 3);
			EquipItem(webDriver, 4);
			// Click on the attack button until the enemy is dead.
			while (webDriver.FindElement(By.Id("enemy-display")).Displayed && webDriver.FindElement(By.Id("action-button-display")).Displayed) {
				IWebElement healthBar = webDriver.FindElement(By.Id("player-health-bar"));
				string heightStyle = healthBar.GetAttribute("style"); // e.g., "height: 42%;"
				double percent = 100; // Default to 100% if not found

				// Try to extract the percentage from the style string
				var match = System.Text.RegularExpressions.Regex.Match(heightStyle, @"height:\s*(\d+)%");
				if (match.Success) {
					percent = double.Parse(match.Groups[1].Value);
				}
				if (percent < 50) { 
					IWebElement healthPotionButton = webDriver.FindElement(By.Id("health-potion-btn"));
					healthPotionButton.Click();
				}
				AttackEnemy(webDriver);
			}
		}
		/// <summary> Equip item in slot X. </summary>
		public static void EquipItem(IWebDriver webDriver, int slot) {
			// Equip better weapon from inventory
			IWebElement inventoryButton = webDriver.FindElement(By.Id("open-inventory-btn"));
			inventoryButton.Click();
			// Wait for the menu modal to show
			WaitForElementToLoad(webDriver, "inventory-slot-1");
			// Click weapon
			IWebElement weaponSlot = webDriver.FindElement(By.Id($"inventory-slot-{slot}"));
			weaponSlot.Click();
			// Click Equip Item
			IWebElement equipButton = webDriver.FindElement(By.Id("equip-btn"));
			equipButton.Click();
			// Close the menu
			IWebElement closeButton = webDriver.FindElement(By.Id("close-inventory-btn"));
			closeButton.Click();
		}
		/// <summary> Attack Enemy</summary>
		public static void AttackEnemy(IWebDriver webDriver) {
			IWebElement attackButton = webDriver.FindElement(By.Id("attack-btn"));
			attackButton.Click();
		}

		/// <summary> Gets you to the boss room with full health/potions. </summary>
		public static void SkipToBoss(IWebDriver webDriver) {
			MoveToRoom(webDriver, 18);
		}

		/// <summary> Moves user to the specified roomNumber. </summary>
		public static void MoveToRoom(IWebDriver webDriver, int targetRoom) {
			// Navigate to the game page
			webDriver.Navigate().GoToUrl($"https://localhost:7186/Game/SkipToRoom?targetRoom={targetRoom}");
		}

		/// <summary> Runs the test X times before failing. </summary>
		public static void Retry(Action action, int maxRetries) {
			int retryCount = 0;
			while (retryCount < maxRetries) {
				try {
					action.Invoke();
					return;
				}
				catch (Exception) {
					retryCount++;
				}
			}
			throw new Exception($"Action failed after {maxRetries} retries.");
		}
	}
}
