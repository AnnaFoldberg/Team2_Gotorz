using Gotorz.Server.Models;

namespace Gotorz.Server.Services
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure); 
        Task<List<HotelRoom>> GetHotelRoomsAsync(string externalHotelId, DateTime arrival, DateTime departure); 
        Task BookHotelAsync(HotelBooking booking);
    }
}