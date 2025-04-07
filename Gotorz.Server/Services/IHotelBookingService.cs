using Gotorz.Shared.Models;

namespace Gotorz.Server.Services
{
public interface IHotelBookingService
{
    Task AddHotelBookingAsync(HotelBooking booking);
   // Task<List<HotelBooking>> GetBookingsByRejsepakkeId(int rejsepakkeId);
 }
}