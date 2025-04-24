using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gotorz.Server.Configurations
{
    /// <summary>
    /// Configures flight entities.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        /// <summary>
        /// Disables cascade deletes to prevent airports from being deleted if they are linked to flights.
        /// </summary>
        /// <remarks>
        /// This is required as SQL Server does not support multiple <c>ON DELETE CASCADE</c> constraints
        /// from the same child table to the same parent table.
        /// </remarks>
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasOne(f => f.DepartureAirport)
                .WithMany()
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.ArrivalAirport)
                .WithMany()
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}