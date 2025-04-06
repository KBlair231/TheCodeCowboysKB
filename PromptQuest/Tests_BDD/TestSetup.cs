using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_BDD {
	[SetUpFixture]
	public class TestSetup {
		private Process serverProcess;

		[OneTimeSetUp]
		public void StartServer() {
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
		}

		[OneTimeTearDown]
		public void StopServer() {
			// Stop Prompt Quest once all the tests are done
			if(serverProcess != null && !serverProcess.HasExited) {
				serverProcess.Kill(); // Terminate the process
				serverProcess.Dispose(); // Clean up unmanaged resources
			}
		}
	}
}