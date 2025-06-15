using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartContractVehicle;
using SmartContractVehicle.Data;
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
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<ICarStateService, InMemoryCarStateService>();

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

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Scoped Services
builder.Services.AddScoped<UserService>();

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

        var (companies, cars) = DbInitializer.SeedCars(user, fuels, drivetrains);

        var tac = context.AutomotiveCompanies.AddRangeAsync(companies);
        var tc = context.Cars.AddRangeAsync(cars);
        await tac;
        await tc;
        await context.SaveChangesAsync();
    }

}

app.MapHub<CarInformationHub>("/CarInformationHub");

app.Run();
