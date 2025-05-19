using System.Text.Json.Nodes;
using Gotorz.Server.Models;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHotelRepository _hotelRepository;
        private readonly IHotelRoomRepository _hotelRoomRepository;
        private readonly IHotelBookingRepository _hotelBookingRepository;

        public HotelService(
            HttpClient httpClient,
            IConfiguration config,
            IHotelRepository hotelRepository,
            IHotelRoomRepository hotelRoomRepository,
            IHotelBookingRepository hotelBookingRepository)
        {
            _httpClient = httpClient;
            _config = config;
            _hotelRepository = hotelRepository;
            _hotelRoomRepository = hotelRoomRepository;
            _hotelBookingRepository = hotelBookingRepository;
        }

        public async Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure)
        {
            var existingHotels = (await _hotelRepository.GetAllAsync())
                .Where(h => h.Address?.ToLower().Contains(city.ToLower()) == true)
                .ToList();

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
            var locationRoot = JsonNode.Parse(locationBody);

            var bestMatch = locationRoot?["data"]?.AsArray()?.FirstOrDefault(item =>
                item?["dest_type"]?.ToString() == "city" &&
                item?["country"]?.ToString()?.ToLower().Contains(country.ToLower()) == true &&
                item?["name"]?.ToString()?.ToLower().Contains(city.ToLower()) == true);

            if (bestMatch == null) return hotels;

            var lat = bestMatch["latitude"]?.GetValue<double>() ?? 0;
            var lon = bestMatch["longitude"]?.GetValue<double>() ?? 0;

            if (lat == 0 || lon == 0) return hotels;

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
            var hotelRoot = JsonNode.Parse(hotelBody);

            var hotelArray = hotelRoot?["data"]?["result"]?.AsArray();
            if (hotelArray == null) return hotels;

            foreach (var h in hotelArray)
            {
                var address = h?["address"]?.ToString() ?? $"{h?["district"]} {h?["zip"]} {h?["city"]}".Trim();
                if (string.IsNullOrWhiteSpace(address)) address = "Unknown";

                hotels.Add(new Hotel
                {
                    Name = h?["hotel_name"]?.ToString() ?? "Unknown",
                    Address = address,
                    Rating = (int)(h?["review_score"]?.GetValue<double>() ?? 0),
                    Latitude = h?["latitude"]?.GetValue<double>() ?? 0,
                    Longitude = h?["longitude"]?.GetValue<double>() ?? 0,
                    ExternalHotelId = h?["hotel_id"]?.ToString() ?? "N/A"
                });
            }

            foreach (var hotel in hotels)
            {
                await _hotelRepository.AddAsync(hotel);
            }

            return hotels;
        }

        public async Task<List<HotelRoom>> GetHotelRoomsAsync(string externalHotelId, DateTime arrival, DateTime departure)
        {
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
            var json = JsonNode.Parse(body);

            var roomArray = json?["available"]?.AsArray();
            if (roomArray == null) return new List<HotelRoom>();

            var rooms = new List<HotelRoom>();

            foreach (var room in roomArray)
            {
                int.TryParse(room?["max_occupancy"]?.ToString(), out var maxGuests);

                rooms.Add(new HotelRoom
                {
                    ExternalHotelId = externalHotelId,
                    RoomId = room?["room_id"]?.ToString() ?? "0",
                    Name = room?["name"]?.ToString() ?? "Unknown",
                    Description = room?["description"]?.ToString(),
                    Capacity = maxGuests,
                    Surface = room?["room_surface_in_m2"]?.GetValue<int>() ?? 0,
                    Price = room?["product_price_breakdown"]?["gross_amount"]?["value"]?.GetValue<decimal>() ?? 0,
                    MealPlan = room?["mealplan"]?.ToString(),
                    Refundable = room?["policy_display_details"]?["refund_during_fc"]?["title_details"]?["translation"] != null,
                    CancellationPolicy = room?["policy_display_details"]?["cancellation"]?["description_details"]?["translation"]?.ToString(),
                    ArrivalDate = arrival,
                    DepartureDate = departure
                });
            }

            foreach (var r in rooms)
            {
                await _hotelRoomRepository.AddAsync(r);
            }

            return rooms;
        }

        public async Task BookHotelAsync(HotelBooking booking)
        {
            await _hotelBookingRepository.AddAsync(booking);
        }

        public async Task<List<HotelSearchHistory>> GetSearchHistory()
        {
            throw new NotImplementedException("HotelSearchHistory repository not yet implemented.");
        }
    }
}