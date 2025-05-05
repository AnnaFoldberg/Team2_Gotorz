using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="Flight"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class FlightRepository : IFlightRepository
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public FlightRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="Flight"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Flight"/> entities.</returns>
        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await _context.Flights.ToListAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="Flight"/> entity by its <c>FlightId</c>.
        /// </summary>
        /// <param name="key">The <c>FlightId</c> of the <see cref="Flight"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Flight"/> or <c>null</c> if not found.</returns>
        public async Task<Flight?> GetByKeyAsync(int key)
        {
            return await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == key);
        }

        /// <summary>
        /// Retrieves a <see cref="Flight"/> entity by its <c>FlightNumber</c>.
        /// </summary>
        /// <param name="flightNumber">The <c>FlightNumber</c> of the <see cref="Flight"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Flight"/> or <c>null</c> if not found.</returns>
        public async Task<Flight?> GetByFlightNumberAsync(string flightNumber)
        {
            return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
        }

        /// <summary>
        /// Adds a new <see cref="Flight"/> to the database.
        /// </summary>
        /// <param name="flight">The <see cref="Flight"/> entity to add.</param>
        public async Task AddAsync(Flight flight)
        {
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="Flight"/> in the database.
        /// </summary>
        /// <param name="flight">The <see cref="Flight"/> entity to update.</param>
        public async Task UpdateAsync(Flight flight)
        {
            _context.Flights.Update(flight);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="Flight"/> from the database.
        /// </summary>
        /// <param name="key">The <c>FlightId</c> of the <see cref="Flight"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var flight = await GetByKeyAsync(key);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }
        }
    }
}