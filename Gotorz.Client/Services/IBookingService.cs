using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    /// <summary>
    /// Retrieves booking data from the <c>BookingController</c> in the Server project.
    /// </summary>
    /// <author>Anna</author>
    public interface IBookingService
    {
        /// <summary>
        /// Retrieves a holiday booking matching the specified <paramref name="bookingReference"/> from the database.
        /// </summary>
        /// <param name="bookingReference">The booking reference to match the holiday booking against.</param>
        /// <returns>The <see cref="HolidayBookingDto"/> matching the specified <paramref name="bookingReference"/>.</returns>
        Task<HolidayBookingDto> GetHolidayBookingAsync(string bookingReference);

        /// <summary>
        /// Posts a holiday booking to the Server by calling its API endpoint, <c>holiday-booking</c>.
        /// </summary>
        /// <param name="holidayBooking">The holiday booking to be posted.</param>
        Task PostHolidayBookingAsync(HolidayBookingDto holidayBooking);

        /// <summary>
        /// Retrieves travellers linked to the holiday booking
        /// matching the specified <paramref name="bookingReference"/> from the database.
        /// </summary>
        /// <param name="bookingReference">The booking reference to match the holiday booking against.</param>
        /// <returns>A collection of <see cref="TravellerDto"/> entities.</returns>
        Task<IEnumerable<TravellerDto>> GetTravellersAsync(string bookingReference);

        /// <summary>
        /// Posts travellers to the Server by calling its API endpoint, <c>travellers</c>.
        /// </summary>
        /// <param name="travellers">The travellers to be posted.</param>
        Task PostTravellersAsync(List<TravellerDto> travellers);
    }
}