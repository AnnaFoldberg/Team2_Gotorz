
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
        /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
        /// <author>Anna</author>
        Task<string> GenerateNextBookingReferenceAsync();
    }
}