using Gotorz.Client.Services;

namespace Gotorz.Server.Services
{
    /// <summary>
    /// Handles creation of unique booking references for holiday bookings.
    /// </summary>
    /// <author>Anna</author>
    public interface IBookingService
    {
        /// <summary>
        /// Generates the next booking reference number.
        /// </summary>
        /// <returns>A <c>string</c> representing the next available booking reference.</returns>
        Task<string> GenerateNextBookingReferenceAsync();
    }
}