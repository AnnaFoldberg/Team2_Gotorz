using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetHotelsAsync();
        Task AddHotelAsync(Hotel hotel);
    }
}