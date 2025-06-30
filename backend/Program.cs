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

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
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
builder.Services.AddSingleton<ConnectionMapping>();
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
    await DbInitializer.SeedRolesAsync(scope.ServiceProvider);

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();


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
