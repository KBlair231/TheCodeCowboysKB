using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Reqnroll;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Microsoft.Extensions.Options;
namespace Tests_BDD.StepDefinitions {
	[Binding]
	public class SCRUM116StepDefinitions {
		private static IWebDriver webDriver;
		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
		}

		[Given("I am on the game page")]
		public void GivenIAmOnTheGamePage() {
			PromptQuestTestMethods.StartNewGame(webDriver, skipTutorial: true);
		}
		[Then("An elite should be spawned")]
		public void ThenAnEliteShouldBeSpawned() {
			Thread.Sleep(5000);
			// Search for the elite's name Dark Orc Warlock
			IWebElement eliteName = webDriver.FindElement(By.Id("enemy-name"));
			// Assert that the elite name displayed is Spectral Orc Berserker
			string testString = eliteName.Text;
			Assert.IsTrue(eliteName.Text == "Spectral Orc Berserker", "The elite name is: " + testString);
		}

		[When("I defeat the elite")]
		public void WhenIDefeatTheElite() {
			PromptQuestTestMethods.ClearRoom(webDriver);
		}

		[Then("I should be given an elite item")]
		public void ThenIShouldBeGivenAnEliteItem() {
			IWebElement newItem = webDriver.FindElement(By.XPath("//img[@src='/images/BerserkerAxe.png']"));
		}
	}
}