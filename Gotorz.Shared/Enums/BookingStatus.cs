using System.Text.Json.Serialization;

namespace Gotorz.Shared.Enums
{
    /// <summary>
    /// Represents the status options for a holiday booking
    /// </summary>
    /// <author>Anna</author>
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}