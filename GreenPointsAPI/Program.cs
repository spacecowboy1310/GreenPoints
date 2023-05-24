using GreenPointsAPI.Data;
using GreenPointsAPI.Properties;
using GreenPointsAPI.Services;
using GreenPointsAPI.Services.MailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
const string MyAllowSpecificOrigins = "CorsPolicy";

// Add services to the container.
builder.Services.AddCors(options =>
{
    // add cors options
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:7120");
    });
});

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
    // This code could be used to get all roles from the database insteaad of hardcoding them
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

/// <summary>
/// The login endpoint used to verify user credentials and generate the session token
/// </summary>
/// <param name="userModel">A user object with the Mail and Password params</param>
/// <returns>The UserDTO without its password and the session token</returns>
app.MapPost("/login", [EnableCors(MyAllowSpecificOrigins)] (UserDTO userModel, GreenPointsContext context) =>
{
    User? user = context.Users.Include(u => u.Roles).FirstOrDefault(user => user.Mail == userModel.Mail);

    if (user is null)
        return Results.NotFound(new { message = "E-mail not found" });

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

    UserWithToken response = new(user.ToDTO(), token);

    return Results.Ok(response);
});

/// <summary>
/// Registers a new user.
/// </summary>
/// <param name="userModel">User model containing registration information.</param>
/// <returns>Returns a response with success or failure message.</returns>
app.MapPost("/register", (TemporalUser userModel, GreenPointsContext context, IMailService mailService) =>
{
    User? user = context.Users.FirstOrDefault(user => user.Mail == userModel.Mail);

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

/// <summary>
/// Endpoint to confirm a user's account based on the GUID in the URL.
/// </summary>
/// <param name="id">Unique identifier of the user to be confirmed.</param>
/// <returns>Returns a response with success or failure message.</returns>
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

/// <summary>
/// Endpoint to update a user's roles based on the request payload.
/// Administrator authorization is required.
/// </summary>
/// <param name="request">Instance of RoleRequest class containing user id and a list with all the roles for the user.</param>
/// <returns>Returns a response with success or failure message.</returns>
app.MapPost("/changeRole", (RoleRequest request, GreenPointsContext context) =>
{
    User? user = context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == request.UserId);
    if (user is null)
        return Results.NotFound("User not found");
    List<Role> roles = new();
    foreach (string role in request.NewRoles)
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

/// <summary>
/// Endpoint to store greenpoint proposals on the temporal table
/// Collaborator authorization is required.
/// </summary>
/// <param name="editGreenPoint">Instance of EditGreenPoint with the proposed information, if it does not contain the Original parameter Latitude, Longitude and Name are required</param>
/// <returns>Returns a response with success or failure message.</returns>
app.MapPost("/greenpoints/request", (EditGreenPointDTO editGreenPoint, GreenPointsContext context) =>
{
    if (editGreenPoint.Original is null && editGreenPoint.Latitude is null || editGreenPoint.Longitude is null || string.IsNullOrEmpty(editGreenPoint.Name.Trim()))
        return Results.BadRequest("Info is missing: all requests for new points must have at least a name, latitude and longitude");

    User? collaborator = context.Users.Find(editGreenPoint.Collaborator);

    if (collaborator is null)
        return Results.NotFound("User not found");

    GreenPoint? original = context.GreenPoints.Find(editGreenPoint.Original);

    EditGreenPoint temp = new EditGreenPoint { Latitude = editGreenPoint.Latitude, Longitude = editGreenPoint.Longitude, Name = editGreenPoint.Name, Properties = editGreenPoint.Properties, Collaborator = collaborator, Original = original };
    context.EditGreenPoints.Add(temp);
    context.SaveChanges();
    return Results.Ok("GreenPoint sent");
}).RequireAuthorization(Roles.Collaborator);

/// <summary>
/// Endpoint to store greenpoint information on the master table and delete entries from the temporal table
/// Editor authorization is required.
/// </summary>
/// <param name="request">Instance of AcceptRequest with the list of ids from the temporal table to delete and an instance of EditGreenpoint with the definitive information to store.</param>
/// <returns>Returns a response with success or failure message.</returns>
app.MapPost("/greenpoints/accept", (AcceptRequest request, GreenPointsContext context) =>
{
    if (request.GreenPoint is not null)
    {
        if (string.IsNullOrWhiteSpace(request.GreenPoint.Name))
            return Results.BadRequest("Info is missing: all requests for new points must have at least a name, latitude and longitude");

        List<User> collaborators = new();
        foreach (int id in request.GreenPoint.Collaborators)
        {
            User? temp = context.Users.Find(id);
            if (temp is not null)
                collaborators.Add(temp);
        }
        GreenPoint greenPoint = new()
        {
            Id = request.GreenPoint.Id,
            Latitude = request.GreenPoint.Latitude,
            Longitude = request.GreenPoint.Longitude,
            Name = request.GreenPoint.Name,
            Properties = request.GreenPoint.Properties,
            Collaborators = collaborators
        };

        if (request.GreenPoint.Id is 0)
        {
            context.GreenPoints.Add(greenPoint);
        }
        else
        {
            GreenPoint? original = context.GreenPoints.Find(request.GreenPoint.Id);
            if (original is null)
                return Results.NotFound("Original point not found");
            // We keep record of every collaborator
            greenPoint.Collaborators = greenPoint.Collaborators.Union(original.Collaborators).ToList();
            original = greenPoint;
            context.GreenPoints.Entry(original).State = EntityState.Modified;
        }
    }
    if (request.ChangeIDs is not null)
    {
        context.EditGreenPoints.RemoveRange(context.EditGreenPoints.Where(e => request.ChangeIDs.Contains(e.Id)));
    }
    context.SaveChanges();
    return Results.Ok("Greenpoint accepted");
}).RequireAuthorization(Roles.Editor);

/// <summary>
/// Endpoint to retrieve grenpoints between two points
/// </summary>
/// <param name="lat1">Latitude of the first point.</param>
/// <param name="lon1">Longitude of the first point.</param>
/// <param name="lat2">Latitude of the second point.</param>
/// <param name="lon2">Longitude of the second point.</param>
/// <returns>Returns the list of greenpoints.</returns>
app.MapGet("/greenpoints/{lat1}/{lon1}/{lat2}/{lon2}", (double lat1, double lon1, double lat2, double lon2, GreenPointsContext context) =>
{
    List<GreenPoint> greenPoints = context.GreenPoints.Include(g => g.Properties)
                                  .Include(g => g.Collaborators)
                                  .Where(g => g.Latitude >= Math.Min(lat1, lat2)
                                           && g.Latitude <= Math.Max(lat1, lat2)
                                           && g.Longitude >= Math.Min(lon1, lon2)
                                           && g.Longitude <= Math.Max(lon1, lon2)).ToList();
    List<GreenPointDTO> response = new();
    foreach (GreenPoint greenPoint in greenPoints)
    {
        response.Add(greenPoint.ToDTO());
    }
    return Results.Ok(response);
});

/// <summary>
/// Endpoint to retrieve the information of an specific greenpoint
/// </summary>
/// <param name="id">The id of the greenpoint.</param>
/// <returns>An instance of GreenpointDTO with all its information.</returns>
app.MapGet("/greenpoints/{id}", (int id, GreenPointsContext context) =>
    Results.Ok(context.GreenPoints.Include(g => g.Properties)
                                  .Include(g => g.Collaborators)
                                  .FirstOrDefault(g => g.Id == id)?.ToDTO()));

/// <summary>
/// Endpoint to retrieve all the greenpoint proposals
/// Editor authorization is required.
/// </summary>
/// <returns>The list of EditGreenPoints.</returns>
app.MapGet("/greenpoints/request", (GreenPointsContext context) =>
    Results.Ok(context.EditGreenPoints.Include(e => e.Properties).Include(e => e.Collaborator).ToList()))
    .RequireAuthorization(Roles.Editor);

app.UseCors();

app.Run();