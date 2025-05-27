using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gotorz.Server.Configurations
{
    public class HotelBookingConfiguration : IEntityTypeConfiguration<HotelBooking>
    {
        public void Configure(EntityTypeBuilder<HotelBooking> builder)
        {

                // Define relationship with HotelRoom (each booking is linked to one room; each room can have multiple bookings)
                builder.HasOne(b => b.HotelRoom)
                    .WithMany()
                    .HasForeignKey(b => b.HotelRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Define relationship with HolidayPackage (each booking belongs to one holiday package)
                builder.HasOne(b => b.HolidayPackage)
                    .WithMany(p => p.HotelBookings)
                    .HasForeignKey(b => b.HolidayPackageId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}