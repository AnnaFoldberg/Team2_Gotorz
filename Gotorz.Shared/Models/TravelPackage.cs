namespace Gotorz.Shared.Models
{
    public class TravelPackage
    {
        public int TravelPackageId { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal Price { get; set; }
        public List<HotelBooking> HotelBookings { get; set; } = new();
    }
}