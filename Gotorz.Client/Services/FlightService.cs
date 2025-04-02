using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    /// <inheritdoc />
    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for API requests.</param>
        public FlightService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Airport>> GetAllAirports()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Airport>>($"http://localhost:5181/Flight/airports");
        }

        /// <inheritdoc />
        public async Task<List<Flight>> GetFlights(string? date, string departureAirport, string arrivalAirport)
        {
			return await _httpClient.GetFromJsonAsync<List<Flight>>($"http://localhost:5181/Flight/flights?date={date}&departureAirport={Uri.EscapeDataString(departureAirport)}&arrivalAirport={Uri.EscapeDataString(arrivalAirport)}");
        }
    }
}