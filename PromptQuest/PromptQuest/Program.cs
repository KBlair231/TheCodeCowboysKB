using Microsoft.AspNetCore.Authentication;
using PromptQuest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // Add in-memory cache
builder.Services.AddSession(options =>// Add session services
{
	// All data in the session (game state, user data, etc.) is stored on the server.
	// To match up the session to the user a unique session ID is created with each session
	// and stored in a session cookie (a small text file) and sent to the client's browser.
	options.IdleTimeout = TimeSpan.FromMinutes(10); // Session timeout
	// Cookie can only be accessed via HTTP requests, not by client-side scripts (for enhanced security).
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures the cookie is sent only over HTTPS
	options.Cookie.SameSite = SameSiteMode.Strict; // Prevents the browser from sending the cookie with cross-site requests
	// Indicates that the cookie is required for the basic functionality of the site and will be stored even if the user has not consented to non-essential cookies.
	// This is important for compliance with regulations like GDPR.
	options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor(); // Add HttpContextAccessor for accessing session

// Register GameService with the dependency injection container
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICombatService, CombatService>();

// Add Google authentication services
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "Application";
	options.DefaultSignInScheme = "External";
})
.AddCookie("Application", options => {
	options.Cookie.HttpOnly = true; // Ensures the cookie is only accessible via HTTP requests
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures the cookie is sent only over HTTPS
	options.Cookie.SameSite = SameSiteMode.Strict; // Prevents the browser from sending the cookie with cross-site requests
})
.AddCookie("External", options => {
	options.Cookie.HttpOnly = true; // Ensures the cookie is only accessible via HTTP requests
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures the cookie is sent only over HTTPS
	options.Cookie.SameSite = SameSiteMode.Lax; // Allows the browser to send the cookie with safe cross-site requests
})
.AddGoogle(options =>
{
	options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
	options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url"); // Map the Google profile picture URL claim
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
