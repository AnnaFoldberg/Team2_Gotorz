using System.Net.Http.Json;
using Gotorz.Shared.DTO;

namespace Gotorz.Client.Services
{
    public class HolidayPackageService
    {
        private readonly HttpClient _http;

        public HolidayPackageService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<HolidayPackageDto>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<HolidayPackageDto>>("HolidayPackage");
            return result ?? new List<HolidayPackageDto>();
        }

        public async Task<HolidayPackageDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<HolidayPackageDto>($"HolidayPackage/{id}");
        }

        public async Task<HolidayPackageDto?> CreateAsync(CreateHolidayPackageDto dto)
        {
            var response = await _http.PostAsJsonAsync("HolidayPackage", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<HolidayPackageDto>();
            }
            return null;
        }

        public async Task DeleteAsync(int id)
        {
            await _http.DeleteAsync($"HolidayPackage/{id}");
        }

        public async Task UpdateAsync(HolidayPackageDto dto)
        {
            await _http.PutAsJsonAsync($"HolidayPackage/{dto.Id}", dto);
        }
    }
}
