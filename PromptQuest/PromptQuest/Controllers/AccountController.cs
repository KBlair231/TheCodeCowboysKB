using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace PromptQuest.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult GoogleLogin()
		{
			var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		public async Task<IActionResult> GoogleResponse()
		{
			var authenticateResult = await HttpContext.AuthenticateAsync("External");
			if (!authenticateResult.Succeeded)
				return BadRequest(); // TODO: Handle this better.

			if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower() == "google")
			{
				if (authenticateResult.Principal != null)
				{
					// Get Google account ID for any operation to be carried out on the basis of the ID
					var googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
					// Get Google profile image URL
					var googleProfileImageUrl = authenticateResult.Principal.FindFirst("urn:google:picture")?.Value;
					var claimsIdentity = new ClaimsIdentity("Application");

					if (authenticateResult.Principal != null)
					{
						var details = authenticateResult.Principal.Claims.ToList();
						// Add Google account ID and profile image URL to claims
						claimsIdentity.AddClaim(new Claim("GoogleAccountId", googleAccountId));
						claimsIdentity.AddClaim(new Claim("GoogleProfileImageUrl", googleProfileImageUrl));
						await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
						return RedirectToAction("Index", "Home");
					}
				}
			}
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> SignOutFromGoogleLogin()
		{
			//Check if any cookie value is present
			if (HttpContext.Request.Cookies.Count > 0)
			{
				//Check for the cookie value with the name mentioned for authentication and delete each cookie
				var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
				foreach (var cookie in siteCookies)
				{
					Response.Cookies.Delete(cookie.Key);
				}
			}
			//signout with any cookie present 
			await HttpContext.SignOutAsync("External");
			return RedirectToAction("Index", "Home");
		}
	}
}