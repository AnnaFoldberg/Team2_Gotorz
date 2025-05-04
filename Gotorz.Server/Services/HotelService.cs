using System.Text.Json.Nodes;
using Gotorz.Shared.Models;
using Gotorz.Server.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly GotorzDbContext _dbContext;

        public HotelService(HttpClient httpClient, IConfiguration config, GotorzDbContext dbContext)
        {
            _httpClient = httpClient;
            _config = config;
            _dbContext = dbContext;
        }

        public async Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure)
        {
            var existingHotels = await _dbContext.Hotels
                .Where(h => h.Address.ToLower().Contains(city.ToLower()))
                .ToListAsync();

            foreach (var h in existingHotels)
            {
                Console.WriteLine($"🧾 {h.Name} | ExternalId: {h.ExternalHotelId}");
            }

            if (existingHotels.Any())
                return existingHotels;

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
                        if (string.IsNullOrWhiteSpace(address)) address = "Unknown";
                    }
                    Console.WriteLine("🧪 Raw JSON for hotel:");
                    Console.WriteLine(h);
                    hotels.Add(new Hotel
                    {
                        
                        Name = h?["hotel_name"]?.ToString() ?? "Unknown",
                        Address = address,
                        Rating = (int)(h?["review_score"]?.GetValue<double>() ?? 0),
                        Latitude = h?["latitude"]?.GetValue<double>() ?? 0,
                        Longitude = h?["longitude"]?.GetValue<double>() ?? 0,
                        ExternalHotelId = h?["hotel_id"]?.ToString() ?? h?["id"]?.ToString() ?? "N/A"
                    });
                }
            }

            if (hotels.Any())
            {
                _dbContext.Hotels.AddRange(hotels);
                await _dbContext.SaveChangesAsync();
            }

            await SaveSearchHistory(city, country, arrival, departure);
            return hotels;
        }

        public async Task<List<HotelRoom>> GetHotelRoomsAsync(string externalHotelId, DateTime arrival, DateTime departure)
        {
            Console.WriteLine($"🔍 Getting rooms for hotel {externalHotelId} from {arrival} to {departure}");

            var arrivalStr = arrival.ToString("yyyy-MM-dd");
            var departureStr = departure.ToString("yyyy-MM-dd");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/getRoomListWithAvailability?hotel_id={externalHotelId}&arrival_date={arrivalStr}&departure_date={departureStr}&adults=1&room_qty=1&units=metric&temperature_unit=c&currency_code=EUR&languagecode=en-us"),
                Headers =
                {
                    { "x-rapidapi-key", _config["RapidAPI:Hotels:Key"] },
                    { "x-rapidapi-host", _config["RapidAPI:Hotels:Host"] },
                },
            };

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("📦 Room API raw response:");
            Console.WriteLine(body);

            var json = JsonNode.Parse(body);
            var rooms = new List<HotelRoom>();
            var roomArray = json?["available"]?.AsArray();
            if (roomArray is null) return rooms;
            Console.WriteLine($"📋 Total rooms found in API: {roomArray?.Count ?? 0}");

            foreach (var room in roomArray)
            {
                var name = room?["name"]?.ToString() ?? "Unknown";
                var description = room?["description"]?.ToString();
                var maxGuests = room?["max_occupancy"]?.GetValue<int>() ?? 0;
                var price = room?["product_price_breakdown"]?["gross_amount"]?["value"]?.GetValue<decimal>() ?? 0;                var roomId = room?["room_id"]?.GetValue<int>().ToString() ?? "0";
                var mealPlan = room?["mealplan"]?.ToString();
                var surface = room?["room_surface_in_m2"]?.GetValue<int?>();
                var breakfast = room?["breakfast_included"]?.GetValue<int?>() == 1;
                var cancelPolicy = room?["policy_display_details"]?["cancellation"]?["description_details"]?["translation"]?.ToString();
                var refundPolicy = room?["policy_display_details"]?["refund_during_fc"]?["title_details"]?["translation"]?.ToString();

                rooms.Add(new HotelRoom
                {
                    ExternalHotelId = externalHotelId,
                    RoomId = roomId,
                    Name = name,
                    Description = description,
                    Capacity = maxGuests,
                    Surface = surface ?? 0, 
                    Price = price,
                    MealPlan = mealPlan,
                    Refundable = refundPolicy != null, 
                    CancellationPolicy = cancelPolicy,
                    ArrivalDate = arrival,
                    DepartureDate = departure
                });
                }

            if (rooms.Any())
            {
                _dbContext.HotelRooms.AddRange(rooms);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"💾 Saved {rooms.Count} rooms to DB.");
            }

            return rooms;
            
        }

        private async Task SaveSearchHistory(string city, string country, DateTime arrival, DateTime departure)
        {
            var history = new HotelSearchHistory
            {
                City = city,
                Country = country,
                ArrivalDate = arrival,
                DepartureDate = departure,
                SearchTimestamp = DateTime.Now
            };

            _dbContext.HotelSearchHistories.Add(history);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<HotelSearchHistory>> GetSearchHistory()
        {
            return await _dbContext.HotelSearchHistories
                .OrderByDescending(h => h.SearchTimestamp)
                .ToListAsync();
        }

        public async Task BookHotelAsync(HotelBooking booking)
        {
            _dbContext.HotelBookings.Add(booking);
            await _dbContext.SaveChangesAsync();
        }
    }
}