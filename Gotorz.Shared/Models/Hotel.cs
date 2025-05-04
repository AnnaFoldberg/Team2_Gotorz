namespace Gotorz.Shared.Models
{
    public class Hotel {
        public int HotelId { get; set; }
        public string? ExternalHotelId { get; set; }  // Getting hotel_id from Booking API
         public string? Name { get; set; }
        public string? Address { get; set; }
        public int Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string GoogleMapsUrl => $"https://www.google.com/maps?q={Latitude},{Longitude}";


    }
}