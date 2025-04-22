using Gotorz.Server.Contexts;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Services;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Gotorz.Server.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GotorzDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection
    ("ConnectionStrings:FrederikConnection").Value);
});


builder.Services.AddScoped<IRepository<Airport>, AirportRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(typeof(HolidayPackageProfile));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
options.AddPolicy("MyAllowedOrigins",
    policy =>
    {
        policy.WithOrigins("https://localhost:7159", "http://localhost:5092") // note the port is included 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient<IFlightService, FlightService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddControllersWithViews(); // Til Web API + MVC
builder.Services.AddRazorPages(); // Blazor Pages support

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(); // <-- Gør det muligt at servere Blazor-filerne

app.UseRouting();

app.UseCors("MyAllowedOrigins");

app.MapControllers();

app.MapFallbackToFile("index.html"); // <-- Sørg for Blazor fallback virker

app.Run();