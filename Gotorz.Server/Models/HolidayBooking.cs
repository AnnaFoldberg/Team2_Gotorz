using System.Text.Json.Serialization;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a holiday booking stored in the database.
    /// </summary>
    public class HolidayBooking
    {
        public int HolidayBookingId { get; set; }
        public string BookingReference { get; set; }
        public string CustomerEmail { get; set; }
        public int Status { get; set; }
        public int HolidayPackageId { get; set; }

        // Navigation property that EF Core uses to join and
        // materialize related data.
        public HolidayPackage HolidayPackage { get; set; } = null;
    }
}