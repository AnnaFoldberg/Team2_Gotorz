// using Gotorz.Server.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace Gotorz.Server.Configurations
// {
//     public class HolidayBookingConfiguration : IEntityTypeConfiguration<HolidayBooking>
//     {
//         public void Configure(EntityTypeBuilder<HolidayBooking> builder)
//         {
//             // Primary Key
//             builder.HasKey(hb => hb.HolidayBookingId);

//             // BookingReference is required
//             builder.Property(hb => hb.BookingReference)
//                    .IsRequired();

//             // Relationship: Each HolidayBooking has one Customer
//             builder.HasOne(hb => hb.Customer)
//                    .WithMany() // Assuming ApplicationUser doesn't track bookings directly
//                    .HasForeignKey(hb => hb.CustomerId)
//                    .OnDelete(DeleteBehavior.Restrict);

//             // Relationship: Each HolidayBooking is linked to one HolidayPackage
//             builder.HasOne(hb => hb.HolidayPackage)
//                    .WithMany()
//                    .HasForeignKey(hb => hb.HolidayPackageId)
//                    .OnDelete(DeleteBehavior.Cascade);

//             // Relationship: Each HolidayBooking may have one HotelBooking (optional One-to-One)
//             builder.HasOne(hb => hb.HotelBooking)
//                    .WithOne(b => b.HolidayBooking)
//                    .HasForeignKey<HolidayBooking>(hb => hb.HotelBookingId)
//                    .OnDelete(DeleteBehavior.Restrict); 
//         }
//     }
// }