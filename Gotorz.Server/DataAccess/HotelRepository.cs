using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="Hotel"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Provides basic CRUD operations for Hotel data.</remarks>
    public class HotelRepository : IHotelRepository
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public HotelRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all hotels from the database.
        /// </summary>
        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        /// <summary>
        /// Gets a hotel by its ID.
        /// </summary>
        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels.FindAsync(id);
        }

        /// <summary>
        /// Adds a new hotel to the database.
        /// </summary>
        public async Task AddAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing hotel in the database.
        /// </summary>
        public async Task UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a hotel from the database by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var hotel = await GetByIdAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }
        }
    }
} 