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
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<HotelSearchHistory> HotelSearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
    }
    
}
