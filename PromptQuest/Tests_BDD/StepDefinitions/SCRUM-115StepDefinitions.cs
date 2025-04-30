using System;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;

namespace Tests_BDD.StepDefinitions {
	[Binding]
	public class SCRUM115StepDefinitions {
		private IWebDriver webDriver;

		[BeforeScenario]
		public void Setup() {
			// Initialize the web driver before each scenario
			webDriver = TestSetup.GetWebDriver();
		}

		[When("I apply a status effect to an enemy")]
		public void WhenIApplyAStatusEffectToAnEnemy() {
			PromptQuestTestMethods.AttackEnemy(webDriver);
		}

		[Then("the enemy should have the status effect applied")]
		public void ThenTheEnemyShouldHaveTheStatusEffectApplied() {
			// check for id of enemy-status-effect
			PromptQuestTestMethods.Retry(() => {
				PromptQuestTestMethods.MoveToRoom(webDriver, 1);
				PromptQuestTestMethods.EquipItem(webDriver, 4);
				PromptQuestTestMethods.AttackEnemy(webDriver);
				var indicator = webDriver.FindElement(By.Id("bleeding-indicator")).Displayed;
			}, 15);
		}
	}
}
