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

        public async Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure)
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

            var locationArray = locationRoot?["data"]?.AsArray();
            JsonNode? bestMatch = locationArray?.FirstOrDefault(item =>
                item?["dest_type"]?.ToString() == "city" &&
                item?["country"]?.ToString()?.ToLower().Contains(country.ToLower()) == true &&
                item?["name"]?.ToString()?.ToLower().Contains(city.ToLower()) == true
            );

            if (bestMatch == null)
                return hotels;

            var lat = bestMatch["latitude"]?.GetValue<double>() ?? 0;
            var lon = bestMatch["longitude"]?.GetValue<double>() ?? 0;

            if (lat == 0 || lon == 0)
                return hotels;

            string arrivalStr = arrival.ToString("yyyy-MM-dd");
            string departureStr = departure.ToString("yyyy-MM-dd");

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
            JsonNode? hotelRoot = JsonNode.Parse(hotelBody);

            if (hotelRoot?["data"]?["result"] is JsonArray hotelArray)
            {
                foreach (var h in hotelArray)
                {
                   var address = h?["address"]?.ToString();

if (string.IsNullOrWhiteSpace(address))
{
    var cityVal = h?["city"]?.ToString();
    var zip = h?["zip"]?.ToString();
    var district = h?["district"]?.ToString();

    address = $"{(district ?? "")} {(zip ?? "")} {(cityVal ?? "")}".Trim();

    if (string.IsNullOrWhiteSpace(address))
    {
        address = "Unknown";
    }
}

hotels.Add(new Hotel
{
    HotelId = hotels.Count + 1,
    Name = h?["hotel_name"]?.ToString() ?? "Unknown",
    Address = address,
    Rating = (int)(h?["review_score"]?.GetValue<double>() ?? 0),
    Latitude = h?["latitude"]?.GetValue<double>() ?? 0,
    Longitude = h?["longitude"]?.GetValue<double>() ?? 0
});
                }
            }

            return hotels;
        }
    }
}
