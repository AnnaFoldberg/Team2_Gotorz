using System.Text.Json.Serialization;

namespace Gotorz.Shared.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public string FlightNumber { get; set; }
        public DateOnly DepartureDate { get; set; }
        public decimal Price { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
    }
}