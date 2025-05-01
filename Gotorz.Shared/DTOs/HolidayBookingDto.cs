using System.Text.Json.Serialization;
using Gotorz.Shared.Enums;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a holiday booking used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class HolidayBookingDto
    {
        public string BookingReference { get; set; }
        public string CustomerEmail { get; set; }
        public BookingStatus Status { get; set; }
        public HolidayPackageDto HolidayPackage { get; set; }
    }
}