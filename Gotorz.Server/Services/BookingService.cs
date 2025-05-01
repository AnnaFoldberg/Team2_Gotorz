
using Gotorz.Client.Services;
using Gotorz.Server.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Services
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class BookingService : IBookingService
    {
        GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public BookingService(GotorzDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<string> GenerateNextBookingReferenceAsync()
        {
            var lastBooking = await _context.HolidayBookings.OrderByDescending(b => b.BookingReference)
                .FirstOrDefaultAsync();

            if (lastBooking == null || string.IsNullOrWhiteSpace(lastBooking.BookingReference))
                return "G0001";

            var lastNumber = int.Parse(lastBooking.BookingReference.Substring(2));
            var nextNumber = lastNumber + 1;

            return $"G{nextNumber:D4}";
        }
    }
}