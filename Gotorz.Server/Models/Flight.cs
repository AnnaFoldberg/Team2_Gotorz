using System.Text.Json.Serialization;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a flight stored in the database.
    /// </summary>
    /// <author>Anna</author>
    public class Flight
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateOnly DepartureDate { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }

        // Navigation properties that EF Core uses to join and
        // materialize related data.
        public Airport DepartureAirport { get; set; } = null!;
        public Airport ArrivalAirport { get; set; } = null!;
    }
}