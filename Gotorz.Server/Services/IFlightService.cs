using Gotorz.Shared.DTO;

namespace Gotorz.Server.Services
{
    /// <summary>
    /// Handles flight-related API requests to Skyscanner using RapidAPI.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Retrieves a list of <see cref="AirportDto"/> entities whose names contain the specified <paramref name="airportName"/>.
        /// </summary>
        /// <param name="airportName">The search term to match airport names against.</param>
        /// <returns>A list of <see cref="AirportDto"/> entities matching the specified <paramref name="airportName"/>.</returns>
        Task<List<AirportDto>> GetAirport(string airportName);

        /// <summary>
        /// Retrieves a list of <see cref="FlightDto"/> entities flying from the specified <paramref name="departureAirport"/> to 
        /// <paramref name="arrivalAirport"/>. Filters by <paramref name="date"/> if specified.
        /// </summary>
        /// <param name="date">The departure date to match the flight against.</param>
        /// <param name="departureAirport">The departure airport to match the flight against.</param>
        /// <param name="arrivalAirport">The arrival airport to match the flight against.</param>
        /// <returns>A list of <see cref="FlightDto"/> entities matching the specified parameters.</returns>
        Task<List<FlightDto>> GetFlights(DateOnly? date, AirportDto departureAirport, AirportDto arrivalAirport);
    }
}