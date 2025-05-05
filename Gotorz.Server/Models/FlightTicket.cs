using System.Text.Json.Serialization;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a flight ticket stored in the database.
    /// </summary>
    /// <author>Anna</author>
    public class FlightTicket
    {
        public int FlightTicketId { get; set; }
        public double Price { get; set; }
        public int FlightId { get; set; }
        public int HolidayPackageId { get; set; }

        // Navigation properties that EF Core uses to join and
        // materialize related data.
        public Flight Flight { get; set; } = null!;
        public HolidayPackage HolidayPackage { get; set; } = null!;
    }
}