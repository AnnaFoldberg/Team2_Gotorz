using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository interface for managing flight entities.
    /// Extends the functionality of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    /// <author>Anna</author>
    public interface IFlightRepository : IRepository<Flight>
    {
        /// <summary>
        /// Retrieves a <see cref="Flight"/> entity by its <c>FlightNumber</c>.
        /// </summary>
        /// <param name="flightNumber">The <c>FlightNumber</c> of the <see cref="Flight"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Flight"/> or <c>null</c> if not found.</returns>
        Task<Flight?> GetByFlightNumberAsync(string flightNumber);
    }
}