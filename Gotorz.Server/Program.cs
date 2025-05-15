using Gotorz.Server.Contexts;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Services;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRepository<Airport>, AirportRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IRepository<FlightTicket>, FlightTicketRepository>();
builder.Services.AddScoped<IRepository<HolidayPackage>, HolidayPackageRepository>();
builder.Services.AddScoped<IHolidayBookingRepository, HolidayBookingRepository>();
builder.Services.AddScoped<IRepository<Traveller>, TravellerRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHolidayPackageRepository, HolidayPackageRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("FrederikConnection");

builder.Services.AddDbContext<GotorzDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:7159", "http://localhost:5092", "http://localhost:7166") 
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddHttpClient<IFlightService, FlightService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllersWithViews(); // Til Web API + MVC
builder.Services.AddRazorPages(); // Blazor Pages support

var app = builder.Build();

// Automatic setup of the database migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    authDb.Database.Migrate();

    var generalDb = scope.ServiceProvider.GetRequiredService<GotorzDbContext>();
    generalDb.Database.Migrate();

    // Seed roles
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "admin", "customer", "sales" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed Default Admin User
    // Username: admin@gotorz.com
    // Password: Admin123
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = "admin@gotorz.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, "Admin123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
            await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "admin"));

        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("MyAllowedOrigins");
app.UseRouting(); //Bruges tiL??
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapFallbackToFile("index.html"); // <-- Sørg for Blazor fallback virker

app.Run();