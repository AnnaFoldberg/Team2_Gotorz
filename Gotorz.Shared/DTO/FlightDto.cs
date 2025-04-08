using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTO
{
    /// <summary>
    /// Represents a flight used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class FlightDto
    {
        public string FlightNumber { get; set; }
        public DateOnly DepartureDate { get; set; }
        public AirportDto DepartureAirport { get; set; }
        public AirportDto ArrivalAirport { get; set; }
    }
}