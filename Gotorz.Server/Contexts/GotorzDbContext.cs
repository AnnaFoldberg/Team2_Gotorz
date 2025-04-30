using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Contexts
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the Gotorz application.
    /// </summary>
    public class GotorzDbContext : DbContext
    {
        /// <inheritdoc />
        public GotorzDbContext(DbContextOptions<GotorzDbContext> options) : base(options) {}

        /// <summary>
        /// DbSet for <see cref="Airport"/> entities
        /// </summary>
        public DbSet<Airport> Airports { get; set; }

        /// <summary>
        /// DbSet for <see cref="Flight"/> entities
        /// </summary>
        public DbSet<Flight> Flights { get; set; }

        /// <summary>
        /// Disables cascade deletes to prevent airports from being deleted if they are linked to flights.
        /// This is required as SQL Server does not support multiple <c>ON DELETE CASCADE</c> constraints
        /// from the same child table to the same parent table.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure entity relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DepartureAirport)
                .WithMany()
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany()
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// DbSet for <see cref="FlightTicket"/> entities
        /// </summary>
        public DbSet<FlightTicket> FlightTickets { get; set; }
        public DbSet<HolidayPackage> HolidayPackages { get; set; }
    }
}