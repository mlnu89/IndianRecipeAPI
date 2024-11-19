using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using IndianRecipeAPI.Models;
using IndianRecipeAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MongoDB configuration
var mongoSettingsSection = builder.Configuration.GetSection("MongoSettings");
var mongoSettings = mongoSettingsSection.Get<MongoSettings>();

if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString) || string.IsNullOrEmpty(mongoSettings.DatabaseName))
{
    throw new InvalidOperationException("MongoSettings are not properly configured in appsettings.json");
}

var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

// Add services
builder.Services.AddSingleton(database);
builder.Services.AddScoped<IRepository<User>>(sp => new MongoRepository<User>(database, "Users"));
builder.Services.AddScoped<IRepository<Recipe>>(sp => new MongoRepository<Recipe>(database, "Recipes"));
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub";
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOAuth("GitHub", options =>
{
    var clientId = builder.Configuration["GitHubAuth:ClientId"];
    var clientSecret = builder.Configuration["GitHubAuth:ClientSecret"];

    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    {
        throw new InvalidOperationException("GitHub OAuth settings (ClientId or ClientSecret) are missing in the configuration.");
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.CallbackPath = new PathString("/signin-github");

    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
    options.UserInformationEndpoint = "https://api.github.com/user";

    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            var id = user.RootElement.GetProperty("id").GetRawText();
            var login = user.RootElement.GetProperty("login").GetString();
            var email = user.RootElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;

            context.Identity!.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            context.Identity.AddClaim(new Claim(ClaimTypes.Name, login!));
            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email ?? ""));
        }
    };
});

var app = builder.Build();

// Middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
