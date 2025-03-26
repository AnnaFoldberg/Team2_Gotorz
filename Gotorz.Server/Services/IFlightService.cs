using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
    public interface IFlightService
    {
        Task<List<Airport>> GetAirport(string airport);
        Task<List<Flight>> GetFlights(DateOnly date, Airport departureAirport, Airport arrivalAirport);
    }
}