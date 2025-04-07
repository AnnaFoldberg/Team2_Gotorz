using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
    public interface IHotelService
    {
Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure);  
Task<List<HotelSearchHistory>> GetSearchHistory();
Task BookHotelAsync(HotelBooking booking);
}
}