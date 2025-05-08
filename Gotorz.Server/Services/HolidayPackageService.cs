using Gotorz.Server.Contexts;
using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Services
{
    public class HolidayPackageService
    {
        private IDbContextFactory<GotorzDbContext> _dbContextFactory;

        public HolidayPackageService(IDbContextFactory<GotorzDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }


        public void AddHolidayPackage(HolidayPackage holidayPackage)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.HolidayPackages.Add(holidayPackage);
            }
        }


    }
}
