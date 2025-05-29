using Microsoft.EntityFrameworkCore;
using Gotorz.Server.Models;
using Gotorz.Server.Configurations;

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

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }

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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(
                typeof(GotorzDbContext).Assembly,
                type => type != typeof(ApplicationUserConfiguration)
            );
            builder.Ignore<ApplicationUser>();
        }
    }

}