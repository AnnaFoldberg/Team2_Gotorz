using Gotorz.Shared.DTOs;
using Gotorz.Shared.Models;
using System.Net.Http.Json;

namespace Gotorz.Client.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;

        public HotelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HotelDto>> GetHotelsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<HotelDto>>("http://localhost:5181/api/hotel");
            return response ?? new List<HotelDto>();
        }

        public async Task AddHotelAsync(HotelDto hotel)
        {
            await _httpClient.PostAsJsonAsync("http://localhost:5181/api/hotel", hotel);
        }

        public async Task<List<HotelDto>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure)
        {
            var query = $"http://localhost:5181/api/hotel/search?city={city}&country={country}&arrival={arrival:yyyy-MM-dd}&departure={departure:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<List<HotelDto>>(query);
            return response ?? new List<HotelDto>();
        }
        public async Task<List<HotelSearchHistory>> GetSearchHistory()
        {
            var result = await _httpClient.GetFromJsonAsync<List<HotelSearchHistory>>("http://localhost:5181/api/hotel/history");
            return result ?? new List<HotelSearchHistory>();
        }
        public async Task<List<HotelRoomDto>> GetHotelRoomsByHotelId(string externalHotelId, DateTime arrival, DateTime departure)
        {
            var query = $"http://localhost:5181/api/hotel/rooms?externalHotelId={externalHotelId}&arrival={arrival:yyyy-MM-dd}&departure={departure:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<List<HotelRoomDto>>(query);
            return response ?? new List<HotelRoomDto>();
        }
        public async Task BookHotelAsync(HotelBookingDto booking)
        {
            await _httpClient.PostAsJsonAsync("http://localhost:5181/api/hotelbooking", new { bookingDto = booking });
        }
    }
}
