using System.Text.Json.Serialization;

namespace Gotorz.Shared.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DepartureAirport { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ArrivalAirport { get; set; }
    }
}