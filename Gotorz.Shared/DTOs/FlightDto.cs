using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a flight used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class FlightDto
    {
        [JsonIgnore]
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateOnly DepartureDate { get; set; }
        public AirportDto DepartureAirport { get; set; }
        public AirportDto ArrivalAirport { get; set; }
        public double TicketPrice { get; set; }
    }
}