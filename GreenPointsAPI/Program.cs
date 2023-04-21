using GreenPointsAPI.Data;
using GreenPointsAPI.Properties;
using GreenPointsAPI.Services;
using GreenPointsAPI.Services.MailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

    UserDTO dto = user.ToDTO();

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

app.MapPost("/changeRole", (roleRequest request, GreenPointsContext context) =>
{
    User? user = context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == request.userId);
    if (user is null)
        return Results.NotFound("User not found");
    List<Role> roles = new();
    foreach (string role in request.newRoles)
    {
        Role? temp = context.Roles.FirstOrDefault(r => r.Name == role);
        if (temp is not null)
            roles.Add(temp);
    }
    user.Roles = roles;
    context.Users.Entry(user).State = EntityState.Modified;
    context.SaveChanges();
    return Results.Ok("User roles updated");
}).RequireAuthorization(Roles.Administrator);

app.MapPost("/greenpoints/request", (EditGreenPoint editGreenPoint, GreenPointsContext context) =>
{
    if (editGreenPoint.Original is null && editGreenPoint.Latitude is null || editGreenPoint.Longitude is null || string.IsNullOrEmpty(editGreenPoint.Name.Trim()))
        return Results.BadRequest("Info is missing: all requests for new points must have at least a name, latitude and longitude");

    User? collaborator = context.Users.Find(editGreenPoint.Collaborator.Id);

    if (collaborator is null)
        return Results.NotFound("User not found");

    editGreenPoint.SetCollaborator(collaborator);
    context.EditGreenPoints.Add(editGreenPoint);
    context.SaveChanges();
    return Results.Ok("GreenPoint sent");
}).RequireAuthorization(Roles.Collaborator);

app.MapPost("/greenpoints/accept", (EditGreenPoint editGreenPoint, GreenPointsContext context) =>
{
    GreenPoint greenPoint;
    try { greenPoint = editGreenPoint.ToGreenPoint(); }
    catch { return Results.BadRequest("Info is missing: all requests for new points must have at least a name, latitude and longitude"); }

    if (editGreenPoint.Original is null)
        context.GreenPoints.Add(greenPoint);
    else
    {
        GreenPoint? original = context.GreenPoints.Find(editGreenPoint.Original.Id);
        if (greenPoint is null)
            return Results.NotFound("Original point not found");
        original = greenPoint;
        context.GreenPoints.Entry(original).State = EntityState.Modified;
    }
    context.SaveChanges();
    return Results.Ok("Greenpoint accepted");
}).RequireAuthorization(Roles.Editor);

/*
 * if a new point is porposed by more than one user we cant relate them, after accepting one editors should be able to make that relationship in order to accept extra properties
 */

app.Run();