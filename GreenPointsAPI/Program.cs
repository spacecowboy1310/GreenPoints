using GreenPointsAPI.Data;
using GreenPointsAPI.Properties;
using GreenPointsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database context
builder.Services.AddDbContext<GreenPointsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication and Authorization services
builder.Services.AddSingleton<TokenService>();

byte[] secretKey = ApiSettings.GenerateSecretByte();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    // This code could be used to get all roles from the database
    //GreenPointsContext context = builder.Services.BuildServiceProvider().GetService<GreenPointsContext>();
    //foreach (string role in context.Roles.Select(r => r.Name).ToList())
    //{
    //    options.AddPolicy(role, policy => policy.RequireRole(role));
    //}
    options.AddPolicy("manager", policy => policy.RequireRole("manager"));
    options.AddPolicy("operator", policy => policy.RequireRole("operator"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Endpoints

app.MapPost("/login", (User userModel, GreenPointsContext context) =>
{
    User? user = context.Users.Include(u => u.Role).FirstOrDefault(user => user.Username == userModel.Username && user.Password == userModel.Password);

    if (user is null)
        return Results.NotFound(new { message = "Invalid username or password" });

    string token = TokenService.GenerateToken(user);

    user.Password = string.Empty;

    return Results.Ok(new { user, token });
});

app.MapGet("/operator", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Authenticated as {user.Identity?.Name ?? "Anonymous"}" });
}).RequireAuthorization("Operator");

app.MapGet("/manager", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Authenticated as {user.Identity?.Name ?? "Anonymous"}" });
}).RequireAuthorization("Manager");


app.Run();