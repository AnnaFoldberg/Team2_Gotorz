namespace Gotorz.Shared.DTOs
{
   public class HotelBookingDto
{
    public int HotelBookingId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCapacity { get; set; }
    public decimal Price { get; set; }

    public int HotelId { get; set; }
    public int HotelRoomId { get; set; }

    // ID is optional since it will be resolved from the title
    public int? HolidayPackageId { get; set; }

    // Used to resolve HolidayPackage by title from frontend
    public string HolidayPackageTitle { get; set; } = string.Empty;

    public HotelDto? Hotel { get; set; }
    public HotelRoomDto? HotelRoom { get; set; }
}
}