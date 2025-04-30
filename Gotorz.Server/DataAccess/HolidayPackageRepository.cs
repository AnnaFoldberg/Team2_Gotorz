using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    public class HolidayPackageRepository : IRepository<HolidayPackage>
    {
        private readonly GotorzDbContext _context;

        public HolidayPackageRepository(GotorzDbContext context)
        {
            _context = context;
        }
        

        public async Task AddAsync(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Add(holidayPackage);
            await _context.SaveChangesAsync();
        }

     
        public async Task DeleteAsync(int key)
        {
            var holidayPackage = await GetByKeyAsync(key);
            if (holidayPackage != null)
            {
                _context.HolidayPackages.Remove(holidayPackage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<HolidayPackage>> GetAllAsync()
        {
            return await _context.HolidayPackages.ToListAsync();
        }

        public async Task<HolidayPackage?> GetByKeyAsync(int key)
        {
            return await _context.HolidayPackages
                .FirstOrDefaultAsync(hp => hp.HolidayPackageId == key);
        }


        public async Task UpdateAsync(HolidayPackage entity)
        {
            _context.HolidayPackages.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
