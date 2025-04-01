using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;

        public FlightService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Airport>> GetAllAirports()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Airport>>($"http://localhost:5181/Flight/airports");
        }

        public async Task<List<Flight>> GetFlights(string? date, string departureAirport, string arrivalAirport)
        {
			return await _httpClient.GetFromJsonAsync<List<Flight>>($"http://localhost:5181/Flight/flights?date={date}&departureAirport={Uri.EscapeDataString(departureAirport)}&arrivalAirport={Uri.EscapeDataString(arrivalAirport)}");
        }
    }
}