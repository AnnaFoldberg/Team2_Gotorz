using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="HolidayPackage"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class HolidayPackageRepository : IRepository<HolidayPackage>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPackageRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public HolidayPackageRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="HolidayPackage"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="HolidayPackage"/> entities.</returns>
        public async Task<IEnumerable<HolidayPackage>> GetAllAsync()
        {
            return await _context.HolidayPackages.ToListAsync();
        }

        /// <summary>
        /// Retrieves an <see cref="HolidayPackage"/> entity by its <c>HolidayPackageId</c>.
        /// </summary>
        /// <param name="key">The <c>HolidayPackageId</c> of the <see cref="HolidayPackage"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayPackage"/> or <c>null</c> if not found.</returns>
        public async Task<HolidayPackage?> GetByKeyAsync(int key)
        {
            return await _context.HolidayPackages.FirstOrDefaultAsync(p => p.HolidayPackageId == key);
        }

        /// <summary>
        /// Adds a new <see cref="HolidayPackage"/> to the database.
        /// </summary>
        /// <param name="holidayPackage">The <see cref="HolidayPackage"/> entity to add.</param>
        public async Task AddAsync(HolidayPackage holidayPackage)
        {
            await _context.HolidayPackages.AddAsync(holidayPackage);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="HolidayPackage"/> in the database.
        /// </summary>
        /// <param name="holidayPackage">The <see cref="HolidayPackage"/> entity to update.</param>
        public async Task UpdateAsync(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Update(holidayPackage);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an <see cref="HolidayPackage"/> from the database.
        /// </summary>
        /// <param name="key">The <c>HolidayPackageId</c> of the <see cref="HolidayPackage"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var holidayPackage = await GetByKeyAsync(key);
            if (holidayPackage != null)
            {
                _context.HolidayPackages.Remove(holidayPackage);
                await _context.SaveChangesAsync();
            }
        }
    }
}