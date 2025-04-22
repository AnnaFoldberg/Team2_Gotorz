using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    public class HolidayPackageRepository : ISimpleKeyRepository<HolidayPackage>
    {
        private readonly GotorzDbContext _context;

        public HolidayPackageRepository(GotorzDbContext context)
        {
            _context = context;
        }
        
        //Skal evt fjernes efter opdateret ISimpleKeyRepository
        public void Add(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Add(holidayPackage);
            _context.SaveChanges();
        }

        public async Task<HolidayPackage> AddAsync(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Add(holidayPackage);
            await _context.SaveChangesAsync();
            return holidayPackage;
        }

        //Skal evt fjernes efter opdateret ISimpleKeyRepository
        public void Delete(int key)
        {
            var holidayPackage = GetByKeyAsync(key);
            if (holidayPackage != null)
            {
                _context.HolidayPackages.Remove(holidayPackage);
                _context.SaveChanges();
            }
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

        //Skal evt fjernes efter opdateret ISimpleKeyRepository
        public IEnumerable<HolidayPackage> GetAll()
        {
            return _context.HolidayPackages.ToList();
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


        //Skal evt fjernes efter opdateret ISimpleKeyRepository
        public void Update(HolidayPackage holidayPackage)
        {
            _context.HolidayPackages.Update(holidayPackage);
            _context.SaveChanges();

        }

        public async Task UpdateAsync(HolidayPackage entity)
        {
            _context.HolidayPackages.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
