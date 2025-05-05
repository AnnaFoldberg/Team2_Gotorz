using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Gotorz.Server.Models;

namespace Gotorz.Server.Contexts
{
    public class GotorzDbContext : DbContext
    {
        /// <summary>
        /// DbSet for <see cref="Airport"/> entities
        /// </summary>
        public DbSet<Airport> Airports { get; set; }

        /// <summary>
        /// DbSet for <see cref="Flight"/> entities
        /// </summary>
        public DbSet<Flight> Flights { get; set; }

        /// <summary>
        /// DbSet for <see cref="FlightTicket"/> entities
        /// </summary>
        public DbSet<FlightTicket> FlightTickets { get; set; }

        /// <summary>
        /// DbSet for <see cref="HolidayPackage"/> entities
        /// </summary>
        public DbSet<HolidayPackage> HolidayPackages { get; set; }

        /// <summary>
        /// DbSet for <see cref="HolidayBooking"/> entities
        /// </summary>
        public DbSet<HolidayBooking> HolidayBookings { get; set; }

        /// <summary>
        /// DbSet for <see cref="Traveller"/> entities
        /// </summary>
        public DbSet<Traveller> Travellers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GotorzDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the <see cref="GotorzDbContext"/> instance with.</param>
        public GotorzDbContext(DbContextOptions<GotorzDbContext> options) : base(options) { }

        /// <summary>
        /// Applies all entity configurations that implement <see cref="IEntityTypeConfiguration{TEntity}"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure entity relationships.</param>
        /// <remarks>Based on suggestion from ChatGPT. Customized for this project.</remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GotorzDbContext).Assembly);
            modelBuilder.Entity<HotelRoom>()
                    .Property(r => r.Price)
                    .HasPrecision(10, 2);

            modelBuilder.Entity<HotelBooking>()
            .HasOne(b => b.HotelRoom)
            .WithMany(r => r.HotelBookings)
            .HasForeignKey(b => b.HotelRoomId);

            modelBuilder.Entity<HotelBooking>()
                .Property(b => b.Price)
                .HasPrecision(10, 2);
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<HotelSearchHistory> HotelSearchHistories { get; set; }

    }

}
