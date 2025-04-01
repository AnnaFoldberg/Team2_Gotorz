using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
    /// <summary>
    /// Handles flight-related API requests to Skyscanner using RapidAPI.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Retrieves a list of <see cref="Airport"/> entities whose names contain the specified <paramref name="airport"/>.
        /// </summary>
        /// <param name="airport">The search term to match airport names against.</param>
        /// <returns>A list of <see cref="Airport"/> entities matching the specified <paramref name="airport"/>.</returns>
        Task<List<Airport>> GetAirport(string airport);

        /// <summary>
        /// Retrieves a list of <see cref="Flight"/> entities flying from the specified <paramref name="departureAirport"/> to 
        /// <paramref name="arrivalAirport"/>. Filters by <paramref name="date"/> if specified.
        /// </summary>
        /// <param name="date">The departure date to match the flight against.</param>
        /// <param name="departureAirport">The departure airport to match the flight against.</param>
        /// <param name="arrivalAirport">The arrival airport to match the flight against.</param>
        /// <returns>A list of <see cref="Flight"/> entities matching the specified parameters.</returns>
        Task<List<Flight>> GetFlights(DateOnly? date, Airport departureAirport, Airport arrivalAirport);
    }
}