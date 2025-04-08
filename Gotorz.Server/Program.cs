using Gotorz.Server.Contexts;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Gotorz.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ISimpleKeyRepository<Airport>, AirportRepository>();

});
    options.UseSqlServer(builder.Configuration.GetConnectionString("SayeConnection"));
{
builder.Services.AddDbContext<GotorzDbContext>(options =>
builder.Services.AddScoped<IRepository<Airport>, AirportRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
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
builder.Services.AddScoped<IHotelService, HotelService>(); 
builder.Services.AddScoped<IHotelBookingService, HotelBookingService>();
builder.Services.AddHttpClient<IHotelService, HotelService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("MyAllowedOrigins");

app.Run();