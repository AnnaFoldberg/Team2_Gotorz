using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTO
{
    /// <summary>
    /// Represents a flight ticket used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class FlightTicket
    {
        public double Price { get; set; }
        public Flight Flight { get; set; };
    }
}