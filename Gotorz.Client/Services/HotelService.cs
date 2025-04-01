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

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Hotel>>("api/hotel");
            return response ?? new List<Hotel>();
        }

        public async Task AddHotelAsync(Hotel hotel)
        {
            await _httpClient.PostAsJsonAsync("api/hotel", hotel);
        }
    }
}