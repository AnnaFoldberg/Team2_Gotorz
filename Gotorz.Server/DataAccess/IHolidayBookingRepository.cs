using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository interface for managing holiday booking entities.
    /// Extends the functionality of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    /// <author>Anna</author>
    public interface IHolidayBookingRepository : IRepository<HolidayBooking>
    {
        /// <summary>
        /// Retrieves a <see cref="HolidayBooking"/> entity by its <c>BookingReference</c>.
        /// </summary>
        /// <param name="bookingReference">The <c>BookingReference</c> of the <see cref="HolidayBooking"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayBooking"/> or <c>null</c> if not found.</returns>
        Task<HolidayBooking?> GetByBookingReferenceAsync(string bookingReference);

        /// <summary>
        /// Retrieves <see cref="HolidayBooking"/> entites by their <c>CustomerId</c>.
        /// </summary>
        /// <param name="customerId">The <c>CustomerId</c> of the <see cref="HolidayBooking"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayBooking"/> entities or <c>null</c> if not found.</returns>
        Task<IEnumerable<HolidayBooking>?> GetByCustomerIdAsync(string customerId);
    }
}