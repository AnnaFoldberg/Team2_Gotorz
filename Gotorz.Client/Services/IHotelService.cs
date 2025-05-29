using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetHotelsAsync();
        Task AddHotelAsync(HotelDto hotel);
        Task<List<HotelDto>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure);  
        Task<List<HotelRoomDto>> GetHotelRoomsByHotelId(string externalHotelId, DateTime arrival, DateTime departure);
        Task BookHotelAsync(HotelBookingDto booking);
    }
}