using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    public interface IHolidayPackageService
    {
        Task<List<HolidayPackageDto>> GetAllAsync();

        Task<HolidayPackageDto?> GetByUrlAsync(string url);

        Task<HolidayPackageDto> CreateAsync(HolidayPackageDto dto);
    }
}