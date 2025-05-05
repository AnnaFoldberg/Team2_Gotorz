using System.Text.Json.Serialization;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a traveller stored in the database.
    /// </summary>
    public class Traveller
    {
        public int TravellerId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PassportNumber { get; set; }
        public int HolidayBookingId { get; set; }
    
        // Navigation property that EF Core uses to join and
        // materialize related data.
        public HolidayBooking HolidayBooking { get; set; } = null;
    }
}