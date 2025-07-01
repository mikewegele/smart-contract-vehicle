using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartContractVehicle.Data;
using SmartContractVehicle.Hubs;
using SmartContractVehicle.Model;
using SmartContractVehicle.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Null check for Configuration
if (builder.Configuration == null)
{
    throw new InvalidOperationException("Configuration is not available.");
}

// Database
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(defaultConnectionString))
{
    throw new InvalidOperationException("DefaultConnection connection string is not configured in appsettings.json.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseNpgsql(
        defaultConnectionString,
        o => o.UseNetTopologySuite()
    )
);

// Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtKey = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrEmpty(jwtIssuer))
    {
        throw new InvalidOperationException("Jwt:Issuer is not configured in appsettings.json.");
    }
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer, // Often the audience is the same as the issuer
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartContractVehicle API", Version = "v1" });

    // Add JWT auth to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // It's good practice to get origins from configuration, especially in production
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (allowedOrigins == null || allowedOrigins.Length == 0)
        {
            // Fallback or throw if no origins are configured. For development, you might allow specific defaults.
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174") // Defaulting for development
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            // Alternatively, throw an exception:
            // throw new InvalidOperationException("Cors:AllowedOrigins is not configured in appsettings.json.");
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// Add SignalR Service
builder.Services.AddSignalR()
    .AddJsonProtocol(options => // Configure JSON serialization for NetTopologySuite
    {
        options.PayloadSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
    });

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Scoped Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CarCommandService>();

// Singleton Services
builder.Services.AddSingleton<ConnectionMappingService>();
builder.Services.AddSingleton<TelemetryService>();

// Hosted Services
builder.Services.AddHostedService<StatusTimerService>(); // This service will update the database so cars status will get reset

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Development middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed roles into the DB
using (var scope = app.Services.CreateScope())
{
    // Null checks for services resolved from the service provider
    var serviceProvider = scope.ServiceProvider ?? throw new InvalidOperationException("Service provider is null during scope creation.");
    await DbInitializer.SeedRolesAsync(serviceProvider);

    var context = serviceProvider.GetService<AppDbContext>() ?? throw new InvalidOperationException("AppDbContext not registered or resolved from service provider.");
    var userManager = serviceProvider.GetService<UserManager<User>>() ?? throw new InvalidOperationException("UserManager<User> not registered or resolved from service provider.");
    var mail = "this.is.not@an-email.dd";
    var user = await userManager.FindByEmailAsync(mail);
    if (user == null)
    {
        user = new User()
        {
            UserName = "Dummy",
            Name = "McDummy",
            Email = mail,
            EmailConfirmed = true,
        };
        var res = await userManager.CreateAsync(user, "SuperSecurePassword123!");
        if (!res.Succeeded)
            throw new Exception("Failed to create seed user: " + string.Join(", ", res.Errors.Select(e => e.Description)));
    }

    if (!context.Cars.Any())
    {
        var fuels = context.FuelTypes.ToArray();
        var drivetrains = context.Drivetrains.ToArray();

        // Null check for Find result
        var available = context.CarStatuses.Find((int)CarStatuses.Available) ?? throw new Exception("The car status \"available\" was not found in the database. It is strictly required.");
        var (companies, cars) = DbInitializer.SeedCars(user, fuels, drivetrains, available);

        var tac = context.AutomotiveCompanies.AddRangeAsync(companies);
        var tc = context.Cars.AddRangeAsync(cars);
        await tac;
        await tc;
        await context.SaveChangesAsync();
    }
}


app.MapHub<CarHub>("/Telemetry");
app.MapHub<CarMonitorHub>("/Dashboard");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Map}/{action=Index}/{id?}");

app.Run();