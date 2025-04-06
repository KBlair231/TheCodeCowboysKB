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
		public static void StartNewGame(IWebDriver webDriver,bool skipTutorial = true) {
			try {
				//Navigate to the Main menu page
				webDriver.Navigate().GoToUrl("https://localhost:7186/");

				//Click on the "New Adventure" button
				IWebElement newAdventureButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='New Adventure']"));
				newAdventureButton.Click();

				//Enter a name in the "Name" field
				IWebElement nameField = webDriver.FindElement(By.Id("name")); // Assuming the 'Name' field has an ID "name"
				nameField.Clear();
				nameField.SendKeys("PlayerName");

				//Click on the "Start Adventure" button
				IWebElement startAdventureButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='Start Adventure']"));
				startAdventureButton.Click();

				if(skipTutorial) {
					//Wait for tutorial modal window to show
					WaitForModalToOpen(webDriver, "tutorialModal");
					//Click on the "Skip Tutorial" button.
					IWebElement skipTutorialButton = webDriver.FindElement(By.XPath("//button[normalize-space(text())='Skip Tutorial']"));
					skipTutorialButton.Click();
					//Wait for tutorial modal window to hide
					WaitForModalToClose(webDriver, "tutorialModal");
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
		public static void WaitForModalToOpen(IWebDriver webDriver, string modalId) {
			//Set timeout time to 10 seconds.
			WebDriverWait wait = new WebDriverWait(webDriver,TimeSpan.FromSeconds(10));
			wait.Until(d => d.FindElement(By.Id(modalId)).Displayed);
		}

		/// <summary> Takes in a IWebDriver object and the id of a modal element and then waits until the modal has closed. </summary>
		public static void WaitForModalToClose(IWebDriver webDriver, string modalId) {
			//Set timeout time to 10 seconds.
			WebDriverWait wait = new WebDriverWait(webDriver,TimeSpan.FromSeconds(10));
			wait.Until(d => !d.FindElement(By.Id(modalId)).Displayed);
		}
	}
}
