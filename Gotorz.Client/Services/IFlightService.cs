using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    /// <summary>
    /// Retrieves flight data from the <c>FlightController</c> in the Server project.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Retrieves all <see cref="Airport"/> entities in the database
        /// by calling the Server project's API endpoint, <c>airports</c>.
        /// </summary>
        /// <returns>A collection of <see cref="Airport"/> entities.</returns>
        Task<IEnumerable<Airport>> GetAllAirports();

        /// <summary>
        /// Retrieves a list of <see cref="Flight"/> entities
        /// by calling the Server project's API endpoint, <c>flights</c>.
        /// </summary>
        /// <param name="date">The departure date to match the flight against.</param>
        /// <param name="departureAirport">The departure airport to match the flight against.</param>
        /// <param name="arrivalAirport">The arrival airport to match the flight against.</param>
        /// <returns>A list of <see cref="Flight"/> entities matching the specified parameters.</returns>
        Task<List<Flight>> GetFlights(string? date, string departureAirport, string arrivalAirport);
    }
}