using Gotorz.Server.Contexts;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Services;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GotorzDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection
    ("ConnectionStrings:AnnaConnection").Value);
});


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