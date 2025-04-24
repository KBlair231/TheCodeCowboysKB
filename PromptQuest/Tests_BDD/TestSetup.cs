using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_BDD.StepDefinitions {
	[Binding]
	public static class TestSetup {
		private static Process serverProcess;
		private static IWebDriver WebDriver; // Shared WebDriver instance

		[BeforeTestRun]
		public static void StartServer() {
		string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\PromptQuest"));
			// Run Prompt Quest on https://localhost:7186/ before all tests start
			serverProcess = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = "C:\\Program Files\\dotnet\\dotnet.exe", // This should work on all windows machines.
					Arguments = "run",
					WorkingDirectory = projectPath,
					UseShellExecute = false, // Run without shell
					CreateNoWindow = true // Prevent opening a new terminal window
				}
			};
			serverProcess.Start();

			// Initialize the WebDriver (headless browser)
			var options = new ChromeOptions();
			options.AddArgument("--headless"); // Run in headless mode
			options.AddArgument("--no-sandbox");
			options.AddArgument("--disable-dev-shm-usage");
			options.AddArgument("--window-size=1920,1080"); // Optional: Set a default window size
			WebDriver = new ChromeDriver(options);
		}

		[AfterTestRun]
		public static void StopServer() {
			// Stop Prompt Quest once all the tests are done
			if(serverProcess != null && !serverProcess.HasExited) {
				serverProcess.Kill(); // Terminate the process
				serverProcess.Dispose(); // Clean up unmanaged resources
			}
			// Quit the WebDriver
			if (WebDriver != null) {
				WebDriver.Quit();
				WebDriver.Dispose();
			}
		}
		public static IWebDriver GetWebDriver() {
			return WebDriver; // Return the shared WebDriver instance
		}
	}
}