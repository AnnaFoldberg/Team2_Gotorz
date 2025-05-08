using System.Net.Http.Json;
using Gotorz.Shared.DTO;

namespace Gotorz.Client.Services
{
    public class HolidayPackageService
    {
        private readonly HttpClient _http;


        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPackageService"/> class.
        /// </summary>
        /// <param name="http">The HTTP client used for sending API requests.</param>
        public HolidayPackageService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<HolidayPackageDto>> GetAllAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<HolidayPackageDto>>("HolidayPackage");
                return result ?? new List<HolidayPackageDto>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching holiday packages: {ex.Message}");
                return new List<HolidayPackageDto>();
            }
        }
        /*
        public async Task<HolidayPackageDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<HolidayPackageDto>($"HolidayPackage/{id}");
        }
        */

        /// <summary>
        /// Sends a request to create a new holiday package using the provided data.
        /// </summary>
        /// <param name="dto">The holiday package data transfer object containing package details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateAsync(HolidayPackageDto dto)
        {
            await _http.PostAsJsonAsync("HolidayPackage", dto);
        }

        /*
        public async Task DeleteAsync(int id)
        {
            await _http.DeleteAsync($"HolidayPackage/{id}");
        }
        */

        /*
        public async Task UpdateAsync(HolidayPackageDto dto)
        {
            await _http.PutAsJsonAsync($"HolidayPackage/{dto.Id}", dto);
        }
        */
    }
}
