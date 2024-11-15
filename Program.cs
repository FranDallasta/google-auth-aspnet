// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Google;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;
// using System.Security.Claims;

// var builder = WebApplication.CreateBuilder(args);

// // Add services for authentication and authorization
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "Cookies"; // Default to cookies
//     options.DefaultChallengeScheme = "Google"; // Use Google for authentication challenges
// })
// .AddCookie("Cookies") // Add cookie authentication to handle login sessions
// .AddGoogle(options =>
// {
//     options.SignInScheme = "Cookies"; // Use cookies to store the login result
//     options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//     options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
// });

// // Register authorization services
// builder.Services.AddAuthorization();

// var app = builder.Build();

// // Enable authentication and authorization middleware
// app.UseAuthentication(); // Needs to be added before UseAuthorization
// app.UseAuthorization();

// // Login endpoint to initiate Google authentication
// app.MapGet("/login", async (HttpContext context) =>
// {
//     var properties = new AuthenticationProperties
//     {
//         RedirectUri = "/profile"
//     };
//     await context.ChallengeAsync("Google", properties);
// });

// // Protected profile endpoint to display user information
// app.MapGet("/profile", [Authorize] (HttpContext context) =>
// {
//     if (context.User.Identity?.IsAuthenticated ?? false)
//     {
//         var name = context.User.FindFirst(ClaimTypes.Name)?.Value;
//         var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
//         return Results.Ok(new { Name = name, Email = email });
//     }
//     return Results.Unauthorized();
// });

// app.Run();


using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/login", async context =>
{
    var clientSecrets = new ClientSecrets
    {
        ClientId = builder.Configuration["Authentication:Google:ClientId"],
        ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
    };

    var scopes = new[] { "https://www.googleapis.com/auth/userinfo.profile" };

    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        clientSecrets,
        scopes,
        "user",
        CancellationToken.None,
        new FileDataStore("DesktopAuth"));

    await context.Response.WriteAsync("Authentication complete!");
});

app.Run();
