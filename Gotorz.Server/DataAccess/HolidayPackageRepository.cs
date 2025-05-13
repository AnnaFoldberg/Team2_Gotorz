using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    public class HolidayPackageRepository : IRepository<HolidayPackage>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPackageRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for data access.</param>
        public HolidayPackageRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new holiday package to the database.
        /// </summary>
        /// <param name="holidayPackage">The holiday package to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Add(holidayPackage);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a holiday package identified by the given key.
        /// </summary>
        /// <param name="key">The ID of the holiday package to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(int key)
        {
            var holidayPackage = await GetByKeyAsync(key);
            if (holidayPackage != null)
            {
                _context.HolidayPackages.Remove(holidayPackage);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieves all holiday packages from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of holiday packages.</returns>
        public async Task<IEnumerable<HolidayPackage>> GetAllAsync()
        {
            return await _context.HolidayPackages.ToListAsync();
        }

        /// <summary>
        /// Retrieves a holiday package by its unique key.
        /// </summary>
        /// <param name="key">The ID of the holiday package to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing the holiday package if found; otherwise, null.</returns>
        public async Task<HolidayPackage?> GetByKeyAsync(int key)
        {
            return await _context.HolidayPackages
                .FirstOrDefaultAsync(hp => hp.HolidayPackageId == key);
        }

        /// <summary>
        /// Updates an existing holiday package in the database.
        /// </summary>
        /// <param name="entity">The holiday package entity with updated values.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(HolidayPackage entity)
        {
            _context.HolidayPackages.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
