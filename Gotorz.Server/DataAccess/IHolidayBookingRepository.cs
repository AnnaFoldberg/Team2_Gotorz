using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository interface for managing holiday booking entities.
    /// Extends the functionality of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    public interface IHolidayBookingRepository : IRepository<HolidayBooking>
    {
        /// <summary>
        /// Retrieves a <see cref="HolidayBooking"/> entity by its <c>BookingReference</c>.
        /// </summary>
        /// <param name="bookingReference">The <c>BookingReference</c> of the <see cref="HolidayBooking"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayBooking"/> or <c>null</c> if not found.</returns>
        Task<HolidayBooking?> GetByBookingReferenceAsync(string bookingReference);
    }
}