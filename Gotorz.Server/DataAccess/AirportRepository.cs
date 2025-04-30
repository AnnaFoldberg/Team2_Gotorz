using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="Airport"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class AirportRepository : IRepository<Airport>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public AirportRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="Airport"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Airport"/> entities.</returns>
        public async Task<IEnumerable<Airport>> GetAllAsync()
        {
            return await _context.Airports.ToListAsync();
        }

        /// <summary>
        /// Retrieves an <see cref="Airport"/> entity by its <c>AirportId</c>.
        /// </summary>
        /// <param name="key">The <c>AirportId</c> of the <see cref="Airport"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Airport"/> or <c>null</c> if not found.</returns>
        public async Task<Airport?> GetByKeyAsync(int key)
        {
            return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == key);
        }

        /// <summary>
        /// Adds a new <see cref="Airport"/> to the database.
        /// </summary>
        /// <param name="airport">The <see cref="Airport"/> entity to add.</param>
        public async Task AddAsync(Airport airport)
        {
            await _context.Airports.AddAsync(airport);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="Airport"/> in the database.
        /// </summary>
        /// <param name="airport">The <see cref="Airport"/> entity to update.</param>
        public async Task UpdateAsync(Airport airport)
        {
            _context.Airports.Update(airport);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an <see cref="Airport"/> from the database.
        /// </summary>
        /// <param name="key">The <c>AirportId</c> of the <see cref="Airport"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var airport = await GetByKeyAsync(key);

            if (airport != null)
            {
                _context.Airports.Remove(airport);
                await _context.SaveChangesAsync();
            }
        }
    }
}