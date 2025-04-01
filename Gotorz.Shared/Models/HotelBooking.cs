namespace Gotorz.Shared.Models
{
    public class HotelBooking
    {
        public int HotelBookingId { get; set; }
        public int HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int RoomCapacity { get; set; }
        public decimal Price { get; set; }
    }
}