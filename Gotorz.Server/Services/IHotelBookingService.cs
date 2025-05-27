using Gotorz.Server.Models; 
using System.Threading.Tasks;

namespace Gotorz.Server.Services
{
    public interface IHotelBookingService
    {
        Task AddHotelBookingAsync(HotelBooking booking);
    }
}