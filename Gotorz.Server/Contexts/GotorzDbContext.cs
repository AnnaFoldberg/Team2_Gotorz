using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Contexts
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the Gotorz application.
    /// </summary>
    public class GotorzDbContext : DbContext
    {
        public GotorzDbContext(DbContextOptions<GotorzDbContext> options) : base(options) {}

        public DbSet<Airport> Airports { get; set; }
    }
}