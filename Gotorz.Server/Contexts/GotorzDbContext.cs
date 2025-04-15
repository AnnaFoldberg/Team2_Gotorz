using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Gotorz.Server.Models;

namespace Gotorz.Server.Contexts
{
    public class GotorzDbContext : DbContext
    {
        public GotorzDbContext(DbContextOptions<GotorzDbContext> options) : base(options) {}

        public DbSet<Airport> Airports { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<HotelSearchHistory> HotelSearchHistories { get; set; }
    }
}