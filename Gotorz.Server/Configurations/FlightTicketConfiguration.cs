using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gotorz.Server.Configurations
{
    /// <summary>
    /// Configures flight ticket entities.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class FlightTicketConfiguration : IEntityTypeConfiguration<FlightTicket>
    {
        /// <summary>
        /// Disables cascade deletes to prevent flights and holiday packages from being deleted
        /// if they are linked to flight tickets.
        /// </summary>
        /// <remarks>
        /// This is required as SQL Server does not support multiple <c>ON DELETE CASCADE</c> constraints
        /// from the same child table to the same parent table.
        /// </remarks>
        public void Configure(EntityTypeBuilder<FlightTicket> builder)
        {
            builder.HasOne(t => t.Flight)
                .WithMany()
                .HasForeignKey(t => t.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.HolidayPackage)
                .WithMany()
                .HasForeignKey(t => t.HolidayPackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}