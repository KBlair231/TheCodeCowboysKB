using OpenQA.Selenium;
using NUnit.Framework;

namespace Tests_BDD.StepDefinitions
{
    [Binding]
    public class GoldStepDefinitions
    {
		private IWebDriver webDriver;
		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
		}

		[When("the user kills an enemy")]
        public void WhenTheUserKillsAnEnemy()
        {
			PromptQuestTestMethods.ClearRoom(webDriver);
		}

        [Then("the user should gain gold")]
        public void ThenTheUserShouldGainGold()
        {
			// Navigate to the inventory tab in the application
			IWebElement menuButton = webDriver.FindElement(By.XPath("//button[normalize-space(text()='Menu')]"));
			menuButton.Click();
			//Wait for menu modal to show before continuing
			PromptQuestTestMethods.WaitForElementToLoad(webDriver, "menu");
			// Get the gold display element's text
			IWebElement goldDisplay = webDriver.FindElement(By.Id("gold-display"));
			string goldText = goldDisplay.Text;
			if (goldText == "0") {
				Assert.Fail("The user did not gain any gold after defeating the enemy.");
			}
			else {
				int goldAmount = int.Parse(goldText);
				Assert.Greater(goldAmount, 0, "The user did not gain any gold after defeating the enemy.");
			}

		}
	}
}
