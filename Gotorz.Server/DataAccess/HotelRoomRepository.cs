using Gotorz.Server.Contexts;
using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository class for managing <see cref="HotelRoom"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>Provides CRUD operations for hotel room records including hotel-related filtering.</remarks>
    public class HotelRoomRepository : IHotelRoomRepository
    {
        private readonly GotorzDbContext _context;

        public HotelRoomRepository(GotorzDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelRoom>> GetAllAsync()
        {
            return await _context.HotelRooms.ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByExternalHotelIdAsync(string externalHotelId)
        {
            return await _context.HotelRooms
                .Where(r => r.ExternalHotelId == externalHotelId)
                .ToListAsync();
        }

        public async Task<HotelRoom?> GetByIdAsync(int id)
        {
            return await _context.HotelRooms.FindAsync(id);
        }

        public async Task AddAsync(HotelRoom room)
        {
            await _context.HotelRooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelRoom room)
        {
            _context.HotelRooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await GetByIdAsync(id);
            if (room != null)
            {
                _context.HotelRooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }
    }
}