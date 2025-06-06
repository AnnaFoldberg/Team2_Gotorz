using System.Net.Http.Json;
using Gotorz.Shared.DTOs;

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
        public async Task<IEnumerable<AirportDto>> GetAllAirportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AirportDto>>($"/Flight/airports");
        }

        /// <inheritdoc />
        public async Task<List<FlightDto>> GetFlightsAsync(string? date, string departureAirport, string arrivalAirport)
        {
            return await _httpClient.GetFromJsonAsync<List<FlightDto>>($"/Flight/flights?date={date}&departureAirport={Uri.EscapeDataString(departureAirport)}&arrivalAirport={Uri.EscapeDataString(arrivalAirport)}");
        }

        /// <inheritdoc />
        public async Task PostFlightTicketsAsync(List<FlightTicketDto> flightTickets)
        {
            await _httpClient.PostAsJsonAsync($"/Flight/flight-tickets", flightTickets);
        }
    }
}