using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTO
{
    /// <summary>
    /// Represents a flight ticket used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class FlightTicketDto
    {
        public double Price { get; set; }
        public FlightDto Flight { get; set; }
    }
}