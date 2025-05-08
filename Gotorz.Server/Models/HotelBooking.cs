using Gotorz.Shared.DTOs;
using Gotorz.Server.Models; 

namespace Gotorz.Server.Models
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

    public int HotelRoomId { get; set; }      
    public HotelRoom? HotelRoom { get; set; } 

    public int HolidayPackageId { get; set; }
    public HolidayPackage? HolidayPackage { get; set; }
    }
}
