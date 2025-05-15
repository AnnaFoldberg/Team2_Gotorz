using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    public interface IHolidayPackageRepository : IRepository<HolidayPackage>
    {
        
        Task<HolidayPackage?> GetByUrlAsync(string url);
    }
}
