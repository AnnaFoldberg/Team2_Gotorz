using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="HotelBooking"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Created by ChatGPT — tailored for Gotorz project structure.</remarks>
    /// <author>Sayeh</author>
    public class HotelBookingRepository : IRepository<HotelBooking>
    {
        private readonly GotorzDbContext _context;

        public HotelBookingRepository(GotorzDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelBooking>> GetAllAsync()
        {
            return await _context.HotelBookings
                .Include(b => b.HotelRoom)
                .Include(b => b.HolidayPackage)
                .ToListAsync();
        }

        public async Task<HotelBooking?> GetByKeyAsync(int key)
        {
            return await _context.HotelBookings
                .Include(b => b.HotelRoom)
                .Include(b => b.HolidayPackage)
                .FirstOrDefaultAsync(b => b.HotelBookingId == key);
        }

        public async Task AddAsync(HotelBooking booking)
        {
            await _context.HotelBookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelBooking booking)
        {
            _context.HotelBookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int key)
        {
            var booking = await GetByKeyAsync(key);
            if (booking != null)
            {
                _context.HotelBookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }
}