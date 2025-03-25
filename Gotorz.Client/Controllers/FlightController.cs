namespace Gotorz.Client.Controllers
{
    public class FlightController
    {

    }
}

// using System.Text.Json;
// using Microsoft.AspNetCore.Mvc;
// using WeatherForecast.Models;

// namespace WeatherForecast.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class WeatherForecastController : Controller
// {
//   [HttpGet]
//   public IActionResult Index()
//   {
//       return View();
//   }

//   [HttpGet("current")]
//   public async Task<IActionResult> GetCurrent(string query, string units)
//   {
//     using (var client = new HttpClient())
//     {
//       var url = new Uri($"http://localhost:5217/WeatherForecastAPI/current?query={query}&units={units}");
//       var response = await client.GetAsync(url);  // Await the HTTP call
//       if (response.IsSuccessStatusCode)
//       {
//           string responseBody = await response.Content.ReadAsStringAsync();

//           var weatherData = JsonSerializer.Deserialize<WeatherData>(responseBody, new JsonSerializerOptions
//           {
//               PropertyNameCaseInsensitive = true
//           });

//           // Store selected unit in ViewBag
//           ViewBag.Units = units;

//           return View("Current", weatherData);
//       }
//       else
//       {
//           return View($"Error: {response.StatusCode}");
//       }
//     }
//   }
// }
