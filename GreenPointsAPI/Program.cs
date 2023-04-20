using GreenPointsAPI.Data;
using GreenPointsAPI.Properties;
using GreenPointsAPI.Services;
using GreenPointsAPI.Services.MailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database context
builder.Services.AddDbContext<GreenPointsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mail service
builder.Services.AddScoped<IMailService, MailService>();

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
    options.AddPolicy(Roles.Collaborator, policy => policy.RequireRole(Roles.Collaborator));
    options.AddPolicy(Roles.Editor, policy => policy.RequireRole(Roles.Editor));
    options.AddPolicy(Roles.Administrator, policy => policy.RequireRole(Roles.Administrator));
});

// Service

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Endpoints

app.MapPost("/login", (UserDTO userModel, GreenPointsContext context) =>
{
    User? user = context.Users.Include(u => u.Roles).FirstOrDefault(user => user.Username == userModel.Username);

    if (user is null)
        return Results.NotFound(new { message = "Username not found" });

    PasswordHasher<User> hasher = new();
    switch (hasher.VerifyHashedPassword(user, user.Password, userModel.Password))
    {
        case PasswordVerificationResult.Failed:
            return Results.BadRequest(new { message = "Incorrect password" });
        case PasswordVerificationResult.SuccessRehashNeeded:
            user.Password = hasher.HashPassword(user, user.Password);
            context.Users.Entry(user).State = EntityState.Modified;
            context.SaveChanges();
            break;
    }

    string token = TokenService.GenerateToken(user);

    user.Password = string.Empty;

    UserDTO dto = user.toDTO();

    return Results.Ok(new { dto, token });
});

app.MapPost("/register", (TemporalUser userModel, GreenPointsContext context, IMailService mailService) =>
{
    User? user = context.Users.FirstOrDefault(user => user.Username == userModel.Username);

    if (user is not null)
        return Results.Conflict("Username already exists");

    user = context.Users.FirstOrDefault(user => user.Mail == userModel.Mail);

    if (user is not null)
        return Results.Conflict("E-mail already registered");

    TemporalUser? temporalUser = context.TemporalUsers.FirstOrDefault(user => user.Mail == userModel.Mail);
    string id;
    PasswordHasher<TemporalUser> hasher = new();
    if (temporalUser is not null)
    {
        id = temporalUser.ID.ToString();
        temporalUser.Username = userModel.Username;
        temporalUser.Password = hasher.HashPassword(userModel, userModel.Password);
        context.TemporalUsers.Entry(temporalUser).State = EntityState.Modified;
    }
    else
    {
        userModel.ID = Guid.NewGuid();
        id = userModel.ID.ToString();
        userModel.Password = hasher.HashPassword(userModel, userModel.Password);
        context.TemporalUsers.Add(userModel);
    }
    context.SaveChanges();

    string subject = "GreenPoints - Confirm your account";
    string body = $@"
        <h1>GreenPoints</h1>
        <p>Thank you for registering in GreenPoints. To confirm your account, please click on the following link:</p>
        <a href=""https://localhost:7204/confirm/{id}"">Confirm account</a>
    ";
    mailService.SendMail(userModel.Mail, subject, body);

    return Results.Ok("User registered");
});

app.MapGet("/confirm/{id}", (Guid id, GreenPointsContext context) =>
{
    TemporalUser? temporalUser = context.TemporalUsers.Find(id);

    if (temporalUser is null)
        return Results.NotFound("User not found");

    Role? role = context.Roles.FirstOrDefault(role => role.Name == Roles.Collaborator);
    User user = new()
    {
        Username = temporalUser.Username,
        Password = temporalUser.Password,
        Mail = temporalUser.Mail,
        Roles = new() { role }
    };

    context.Users.Add(user);
    context.TemporalUsers.Remove(temporalUser);
    context.SaveChanges();

    return Results.Ok("User confirmed");
});

app.MapPost("/greenpoints", (GreenPointsContext context, EditGreenPoint greenPoint) =>
{
    User? collaborator = context.Users.Find(greenPoint.Collaborator.Id);
    
    if (collaborator is null)
        return Results.NotFound("User not found");
    
    greenPoint.setCollaborator(collaborator);
    context.EditGreenPoints.Add(greenPoint);
    context.SaveChanges();
    return Results.Ok("GreenPoint accepted");
}).RequireAuthorization(Roles.Collaborator);

app.Run();