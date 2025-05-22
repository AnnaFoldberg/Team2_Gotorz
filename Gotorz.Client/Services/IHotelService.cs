using Gotorz.Shared.DTOs;
using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetHotelsAsync();
        Task AddHotelAsync(HotelDto hotel);

Task<List<HotelDto>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure);  
Task<List<HotelSearchHistory>> GetSearchHistory(); 
Task<List<HotelRoomDto>> GetHotelRoomsByHotelId(string externalHotelId, DateTime arrival, DateTime departure);
Task<bool> BookHotelAsync(HotelBookingDto booking);
 
}
}