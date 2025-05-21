using Gotorz.Server.Models;
using Gotorz.Server.Contexts;
using System.Threading.Tasks;

namespace Gotorz.Server.Services
{
    public class HotelBookingService : IHotelBookingService
    {
        private readonly GotorzDbContext _context;

        public HotelBookingService(GotorzDbContext context)
        {
            _context = context;
        }

        public async Task AddHotelBookingAsync(HotelBooking booking)
        {
                Console.WriteLine($"📦 Saving booking for HotelId: {booking.HotelId}, PackageId: {booking.HolidayPackageId}");
            _context.HotelBookings.Add(booking);
            await _context.SaveChangesAsync();
        }
    }
}