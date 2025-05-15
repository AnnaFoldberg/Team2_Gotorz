using System.Net.Http.Json;
using System.Web;
using Gotorz.Shared.DTOs;

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

        /// <summary>
        /// Sends a request to retrieve all available holiday packages.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation, with a list of holiday package DTOs as the result.
        /// </returns>
        public async Task<List<HolidayPackageDto>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<HolidayPackageDto>>("HolidayPackage");
            return result ?? new List<HolidayPackageDto>();
        }

        /*
        public async Task<HolidayPackageDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<HolidayPackageDto>($"HolidayPackage/{id}");
        }
        */
        /// <summary>
        /// Sends a request to retrieve a specific holiday package identified by its URL-friendly title.
        /// </summary>
        /// <param name="url">The URL-friendly string identifying the holiday package.</param>
        /// <returns>
        public async Task<HolidayPackageDto?> GetByUrlAsync(string url)
        {
            return await _http.GetFromJsonAsync<HolidayPackageDto>($"HolidayPackage/{url}");
        }


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
