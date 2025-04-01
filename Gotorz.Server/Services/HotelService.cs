using System.Text.Json.Nodes;
using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HotelService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

       public async Task<List<Hotel>> GetHotelsByCityName(string city, DateTime arrival, DateTime departure)
{
    var hotels = new List<Hotel>();

    var locationRequest = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={city}"),
        Headers =
        {
            { "x-rapidapi-key", _config["RapidAPI:Hotels:Key"] },
            { "x-rapidapi-host", _config["RapidAPI:Hotels:Host"] },
        },
    };

    using var locationResponse = await _httpClient.SendAsync(locationRequest);
    var locationBody = await locationResponse.Content.ReadAsStringAsync();
    JsonNode? locationRoot = JsonNode.Parse(locationBody);
    var lat = locationRoot?["data"]?[0]?["latitude"]?.GetValue<double>() ?? 0;
    var lon = locationRoot?["data"]?[0]?["longitude"]?.GetValue<double>() ?? 0;

    if (lat == 0 || lon == 0) 
    {
        return hotels;
    }
    string departureStr = departure.ToString("yyyy-MM-dd");

    string arrivalStr = arrival.ToString("yyyy-MM-dd");

    var hotelRequest = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotelsByCoordinates?latitude={lat}&longitude={lon}&adults=1&room_qty=1&units=metric&page_number=1&locale=en-us&currency_code=EUR&arrival_date={arrivalStr}&departure_date={departureStr}"),
        Headers =
        {
            { "x-rapidapi-key", _config["RapidAPI:Hotels:Key"] },
            { "x-rapidapi-host", _config["RapidAPI:Hotels:Host"] },
        },
    };

    using var hotelResponse = await _httpClient.SendAsync(hotelRequest);
    var hotelBody = await hotelResponse.Content.ReadAsStringAsync();

    Console.WriteLine("📦 Hotel API JSON:");
    Console.WriteLine(hotelBody);

    JsonNode? hotelRoot = JsonNode.Parse(hotelBody);

    if (hotelRoot?["data"] is JsonArray hotelArray)
    {
        foreach (var h in hotelArray)
        {
            hotels.Add(new Hotel
            {
                HotelId = hotels.Count + 1,
                Name = h?["hotel_name"]?.ToString() ?? "Unknown",
                Address = h?["address"]?.ToString() ?? "Unknown",
                Rating = h?["review_score"]?.GetValue<int>() ?? 0
            });
        }
    }
    else
    {
        Console.WriteLine("❌ Expected JSON array was not found in 'data'. Full response:");
        Console.WriteLine(hotelBody);
    }

    // در نهایت لیست هتل‌ها را برمی‌گردانیم
    return hotels;
}
    }
}