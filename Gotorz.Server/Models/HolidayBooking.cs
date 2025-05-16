namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a holiday booking stored in the database.
    /// </summary>
    public class HolidayBooking
    {
        public int HolidayBookingId { get; set; }
        public string BookingReference { get; set; }
        public int Status { get; set; }
        public string CustomerId { get; set; }
        public int HolidayPackageId { get; set; }

        // Navigation properties that EF Core uses to join and
        // materialize related data.
        public ApplicationUser Customer { get; set; } = null;
        public HolidayPackage HolidayPackage { get; set; } = null;

    }
}