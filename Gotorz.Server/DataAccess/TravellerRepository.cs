using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="Traveller"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class TravellerRepository : IRepository<Traveller>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TravellerRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public TravellerRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="Traveller"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Traveller"/> entities.</returns>
        public async Task<IEnumerable<Traveller>> GetAllAsync()
        {
            return await _context.Travellers.ToListAsync();
        }

        /// <summary>
        /// Retrieves an <see cref="Traveller"/> entity by its <c>TravellerId</c>.
        /// </summary>
        /// <param name="key">The <c>TravellerId</c> of the <see cref="Traveller"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Traveller"/> or <c>null</c> if not found.</returns>
        public async Task<Traveller?> GetByKeyAsync(int key)
        {
            return await _context.Travellers.FirstOrDefaultAsync(p => p.TravellerId == key);
        }

        /// <summary>
        /// Adds a new <see cref="Traveller"/> to the database.
        /// </summary>
        /// <param name="traveller">The <see cref="Traveller"/> entity to add.</param>
        public async Task AddAsync(Traveller traveller)
        {
            await _context.Travellers.AddAsync(traveller);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="Traveller"/> in the database.
        /// </summary>
        /// <param name="traveller">The <see cref="Traveller"/> entity to update.</param>
        public async Task UpdateAsync(Traveller traveller)
        {
            _context.Travellers.Update(traveller);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an <see cref="Traveller"/> from the database.
        /// </summary>
        /// <param name="key">The <c>TravellerId</c> of the <see cref="Traveller"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var traveller = await GetByKeyAsync(key);
            if (traveller != null)
            {
                _context.Travellers.Remove(traveller);
                await _context.SaveChangesAsync();
            }
        }
    }
}