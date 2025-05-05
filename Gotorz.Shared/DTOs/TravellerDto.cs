using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a traveller used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class TravellerDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PassportNumber { get; set; }
        public HolidayBookingDto HolidayBooking { get; set; }
    }
}