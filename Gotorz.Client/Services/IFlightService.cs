using Gotorz.Shared.DTO;

namespace Gotorz.Client.Services
{
    /// <summary>
    /// Retrieves flight data from the <c>FlightController</c> in the Server project.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Retrieves all <c>Airport</c> entities in the database
        /// by calling the Server project's API endpoint, <c>airports</c>.
        /// </summary>
        /// <returns>A collection of <see cref="AirportDto"/> entities.</returns>
        Task<IEnumerable<AirportDto>> GetAllAirports();

        /// <summary>
        /// Retrieves a list of <see cref="FlightDto"/> entities
        /// by calling the Server project's API endpoint, <c>flights</c>.
        /// </summary>
        /// <param name="date">The departure date to match the flight against.</param>
        /// <param name="departureAirport">The departure airport to match the flight against.</param>
        /// <param name="arrivalAirport">The arrival airport to match the flight against.</param>
        /// <returns>A list of <see cref="FlightDto"/> entities matching the specified parameters.</returns>
        Task<List<FlightDto>> GetFlights(string? date, string departureAirport, string arrivalAirport);
    }
}