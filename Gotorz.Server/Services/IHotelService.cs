using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetHotelsByCityName(string city, DateTime arrival, DateTime departure);
    }
}