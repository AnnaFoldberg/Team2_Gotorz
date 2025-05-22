using Gotorz.Shared.DTOs;
using Gotorz.Shared.Models;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using static Microsoft.JSInterop.IJSRuntime;
using Microsoft.JSInterop;


namespace Gotorz.Client.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HotelService> _logger;


        public HotelService(HttpClient httpClient, ILogger<HotelService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
        // public async Task BookHotelAsync(HotelBookingDto booking)
        // {
        //     await _httpClient.PostAsJsonAsync("http://localhost:5181/api/hotelbooking", new { bookingDto = booking });
        // }
        public async Task<bool> BookHotelAsync(HotelBookingDto booking)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5181/api/hotelbooking", new { bookingDto = booking });

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Hotelbooking succeeded. RoomId: {RoomId}", booking.HotelRoom.HotelRoomId);
                    return true;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Hotelbooking failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during hotel booking.");
                return false;
            }
        }
    }
}
