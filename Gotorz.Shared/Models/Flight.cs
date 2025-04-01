using System.Text.Json.Serialization;

namespace Gotorz.Shared.Models
{
    /// <summary>
    /// Represents the data structure for a <see cref="Flight"/> entity.
    /// </summary>
    public class Flight
    {
        private static int _nextId = 1;

        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateOnly DepartureDate { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }

        public Flight()
        {
            FlightId = _nextId++;
            Console.WriteLine($"Flight {FlightNumber} created with FlightId {FlightId}");
        }
    }
}