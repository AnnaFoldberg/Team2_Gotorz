using Gotorz.Server.Contexts;
using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="Airport"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    public class AirportRepository : ISimpleKeyRepository<Airport>
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public AirportRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="Airport"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Airport"/> entities.</returns>
        public IEnumerable<Airport> GetAll()
        {
            return _context.Airports.ToList();
        }

        /// <summary>
        /// Retrieves an <see cref="Airport"/> entity by its <c>AirportId</c>.
        /// </summary>
        /// <param name="key">The <c>AirportId</c> of the <see cref="Airport"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="Airport"/> or <c>null</c> if not found.</returns>
        public Airport? GetByKey(int key)
        {
            return _context.Airports.FirstOrDefault(a => a.AirportId == key);
        }

        /// <summary>
        /// Adds a new <see cref="Airport"/> to the database.
        /// </summary>
        /// <param name="airport">The <see cref="Airport"/> entity to add.</param>
        public void Add(Airport airport)
        {
            _context.Airports.Add(airport);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing <see cref="Airport"/> in the database.
        /// </summary>
        /// <param name="airport">The <see cref="Airport"/> entity to update.</param>
        public void Update(Airport airport)
        {
            _context.Airports.Update(airport);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes an <see cref="Airport"/> from the database.
        /// </summary>
        /// <param name="key">The <c>AirportId</c> of the <see cref="Airport"/> entity to delete.</param>
        public void Delete(int key)
        {
            var airport = GetByKey(key);
            if (airport != null)
            {
                _context.Airports.Remove(airport);
                _context.SaveChanges();
            }
        }
    }
}