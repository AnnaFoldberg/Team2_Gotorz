namespace Gotorz.Shared.DTOs
{
   public class HotelBookingDto
{
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public HolidayPackageDto HolidayPackageDto { get; set; }
    
   // public HotelDto Hotel { get; set; }
        public HotelRoomDto? HotelRoom { get; set; }
 }
}