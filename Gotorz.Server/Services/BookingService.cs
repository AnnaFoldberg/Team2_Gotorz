
using Gotorz.Client.Services;
using Gotorz.Server.Contexts;
using Gotorz.Server.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Services
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IHolidayBookingRepository _holidayBookingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="holidayBookingRepository">The <see cref="IHolidayBookingRepository"/> used to access
        /// <see cref="HolidayBooking"/> data in the database.</param>
        public BookingService(IHolidayBookingRepository holidayBookingRepository)
        {
            _holidayBookingRepository = holidayBookingRepository;
        }

        /// <inheritdoc />
        public async Task<string> GenerateNextBookingReferenceAsync()
        {
            var lastBooking = (await _holidayBookingRepository.GetAllAsync())
                .OrderByDescending(b => b.BookingReference).FirstOrDefault();

            if (lastBooking == null || string.IsNullOrWhiteSpace(lastBooking.BookingReference))
                return "G0001";

            var lastNumber = int.Parse(lastBooking.BookingReference.Substring(2));
            var nextNumber = lastNumber + 1;

            return $"G{nextNumber:D4}";
        }
    }
}