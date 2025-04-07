using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetHotelsAsync();
        Task AddHotelAsync(Hotel hotel);

Task<List<Hotel>> GetHotelsByCityName(string city, string country, DateTime arrival, DateTime departure);  
Task<List<HotelSearchHistory>> GetSearchHistory(); 
Task BookHotelAsync(HotelBooking booking);
 
}
}