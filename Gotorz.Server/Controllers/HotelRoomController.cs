using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gotorz.Server.Contexts;
using Gotorz.Shared.Models;
using Gotorz.Shared.DTO;

namespace Gotorz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelRoomController : ControllerBase
    {
        private readonly GotorzDbContext _context;

        public HotelRoomController(GotorzDbContext context)
        {
            _context = context;
        }

       /* [HttpGet("{externalHotelId}")]
        public async Task<ActionResult<List<HotelRoom>>> GetHotelRooms(string externalHotelId, [FromQuery] DateTime arrival, [FromQuery] DateTime departure)
        {
            var rooms = await _context.HotelRooms
                .Where(r => r.ExternalHotelId == externalHotelId
                            && r.ArrivalDate == arrival
                            && r.DepartureDate == departure)
                .ToListAsync();

            if (!rooms.Any())
                return NotFound();

            return rooms;
        }
        */
        [HttpGet("rooms")]
public async Task<ActionResult<List<HotelRoomDto>>> GetHotelRooms([FromQuery] string externalHotelId, [FromQuery] DateTime arrival, [FromQuery] DateTime departure)
{
    var rooms = await _context.HotelRooms
        .Where(r => r.ExternalHotelId == externalHotelId
                    && r.ArrivalDate == arrival
                    && r.DepartureDate == departure)
        .Select(r => new HotelRoomDto
        {
            HotelRoomId = r.HotelRoomId, 
            RoomId = r.RoomId,
            Name = r.Name,
            Capacity = r.Capacity,
            Price = r.Price,
            MealPlan = r.MealPlan,
            Refundable = r.Refundable
        })
        .ToListAsync();

    if (!rooms.Any())
        return NotFound();

    return rooms;
}
    }
}