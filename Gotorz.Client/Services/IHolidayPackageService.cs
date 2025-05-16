using System.Net.Http.Json;
using System.Web;
using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    public interface IHolidayPackageService
    {
        Task<List<HolidayPackageDto>> GetAllAsync();

        Task<HolidayPackageDto?> GetByUrlAsync(string url);

        Task CreateAsync(HolidayPackageDto dto);
    }
}
