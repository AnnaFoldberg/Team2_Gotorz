using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="FlightTicket"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class FlightTicketRepository : IRepository<FlightTicket>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightTicketRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public FlightTicketRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="FlightTicket"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="FlightTicket"/> entities.</returns>
        public async Task<IEnumerable<FlightTicket>> GetAllAsync()
        {
            return await _context.FlightTickets.ToListAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="FlightTicket"/> entity by its <c>FlightTicketId</c>.
        /// </summary>
        /// <param name="key">The <c>FlightTicketId</c> of the <see cref="FlightTicket"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="FlightTicket"/> or <c>null</c> if not found.</returns>
        public async Task<FlightTicket?> GetByKeyAsync(int key)
        {
            return await _context.FlightTickets.FirstOrDefaultAsync(f => f.FlightTicketId == key);
        }

        /// <summary>
        /// Adds a new <see cref="FlightTicket"/> to the database.
        /// </summary>
        /// <param name="flightTicket">The <see cref="FlightTicket"/> entity to add.</param>
        public async Task AddAsync(FlightTicket flightTicket)
        {
            await _context.FlightTickets.AddAsync(flightTicket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="FlightTicket"/> in the database.
        /// </summary>
        /// <param name="flightTicket">The <see cref="FlightTicket"/> entity to update.</param>
        public async Task UpdateAsync(FlightTicket flightTicket)
        {
            _context.FlightTickets.Update(flightTicket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="FlightTicket"/> from the database.
        /// </summary>
        /// <param name="key">The <c>FlightTicketId</c> of the <see cref="FlightTicket"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var flightTicket = await GetByKeyAsync(key);
            if (flightTicket != null)
            {
                _context.FlightTickets.Remove(flightTicket);
                await _context.SaveChangesAsync();
            }
        }
    }
}