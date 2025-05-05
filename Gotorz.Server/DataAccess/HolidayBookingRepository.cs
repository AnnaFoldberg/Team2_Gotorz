using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="HolidayBooking"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class HolidayBookingRepository : IHolidayBookingRepository
    {
        private readonly GotorzDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayBookingRepository"/> class.
        /// </summary>
        /// <param name="context">The application's Entity Framework Core database context.</param>
        public HolidayBookingRepository(GotorzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all <see cref="HolidayBooking"/> entities from the database.
        /// </summary>
        /// <returns>A collection of <see cref="HolidayBooking"/> entities.</returns>
        public async Task<IEnumerable<HolidayBooking>> GetAllAsync()
        {
            return await _context.HolidayBookings.ToListAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="HolidayBooking"/> entity by its <c>HolidayBookingId</c>.
        /// </summary>
        /// <param name="key">The <c>HolidayBookingId</c> of the <see cref="HolidayBookingId"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayBooking"/> or <c>null</c> if not found.</returns>
        public async Task<HolidayBooking?> GetByKeyAsync(int key)
        {
            return await _context.HolidayBookings
            .Include(b => b.HolidayPackage)
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.HolidayBookingId == key);
        }

        /// <summary>
        /// Retrieves a <see cref="HolidayBooking"/> entity by its <c>BookingReference</c>.
        /// </summary>
        /// <param name="bookingReference">The <c>BookingReference</c> of the <see cref="HolidayBooking"/> entity to retrieve.</param>
        /// <returns>The matching <see cref="HolidayBooking"/> or <c>null</c> if not found.</returns>
        public async Task<HolidayBooking?> GetByBookingReferenceAsync(string bookingReference)
        {
            return await _context.HolidayBookings
                .Include(b => b.HolidayPackage)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingReference == bookingReference);
        }

        /// <summary>
        /// Adds a new <see cref="HolidayBooking"/> to the database.
        /// </summary>
        /// <param name="holidayBooking">The <see cref="HolidayBooking"/> entity to add.</param>
        public async Task AddAsync(HolidayBooking holidayBooking)
        {
            await _context.HolidayBookings.AddAsync(holidayBooking);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="HolidayBooking"/> in the database.
        /// </summary>
        /// <param name="holidayBooking">The <see cref="HolidayBooking"/> entity to update.</param>
        public async Task UpdateAsync(HolidayBooking holidayBooking)
        {
            _context.HolidayBookings.Update(holidayBooking);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="HolidayBooking"/> from the database.
        /// </summary>
        /// <param name="key">The <c>HolidayBookingId</c> of the <see cref="HolidayBooking"/> entity to delete.</param>
        public async Task DeleteAsync(int key)
        {
            var holidayBooking = await GetByKeyAsync(key);
            if (holidayBooking != null)
            {
                _context.HolidayBookings.Remove(holidayBooking);
                await _context.SaveChangesAsync();
            }
        }
    }
}