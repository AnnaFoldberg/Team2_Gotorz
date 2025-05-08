using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Contexts
{
    public class GotorzDbContext : DbContext
    {
        public GotorzDbContext(DbContextOptions<GotorzDbContext> options) : base(options) {}

        public DbSet<Airport> Airports { get; set; }

        // public DbSet<Flight> Flights { get; set; }

        // public DbSet<AirportFlight> AirportFlights { get; set; }

        public DbSet<HolidayPackage> HolidayPackages { get; set; }
    }
}