using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public interface IFlightService
    {
        Task<List<Flight>> GetFlights(string date, string departureAirport, string arrivalAirport);
    }
}