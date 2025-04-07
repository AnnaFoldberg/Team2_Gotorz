
using System.Text.Json.Nodes;
using Gotorz.Shared.Models;
using Gotorz.Server.Contexts;

namespace Gotorz.Server.Services
{
public class HotelBookingService : IHotelBookingService
{
    private readonly GotorzDbContext _context;

    public HotelBookingService(GotorzDbContext context)
    {
        _context = context;
    }

    public async Task AddHotelBookingAsync(HotelBooking booking)
    {
        _context.HotelBookings.Add(booking);
        await _context.SaveChangesAsync();
    }
    /*
    public async Task<List<HotelBooking>> GetBookingsByRejsepakkeId(int rejsepakkeId)
    {
    return await _context.HotelBookings
        .Where(b => b.RejsepakkeId == rejsepakkeId)
        .Include(b => b.Hotel)
        .ToListAsync();
    }
    */
 }
}