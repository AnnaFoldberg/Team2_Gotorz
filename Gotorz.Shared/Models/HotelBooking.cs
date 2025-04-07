namespace Gotorz.Shared.Models
{
   public class HotelBooking
{
    public int HotelBookingId { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public int RoomCapacity { get; set; }
    public decimal Price { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    //public int RejsepakkeId { get; set; }
   // public Rejsepakke? Rejsepakke { get; set; }
    }
}