// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Gotorz.Server.Contexts;
// using Gotorz.Shared.Models;
// using Gotorz.Shared.DTOs;

// namespace Gotorz.Server.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class HotelRoomController : ControllerBase
//     {
//         private readonly GotorzDbContext _context;

//         public HotelRoomController(GotorzDbContext context)
//         {
//             _context = context;
//         }

//         [HttpGet("rooms")]
//         public async Task<ActionResult<List<HotelRoomDto>>> GetHotelRooms(
//             [FromQuery] string externalHotelId,
//             [FromQuery] DateTime arrival,
//             [FromQuery] DateTime departure)
//         {
//             Console.WriteLine($"[HotelRoomController] externalHotelId: {externalHotelId}, arrival: {arrival}, departure: {departure}");

//             var rooms = await _context.HotelRooms
//                 .Where(r => r.ExternalHotelId == externalHotelId
//                             && r.ArrivalDate.Date == arrival.Date
//                             && r.DepartureDate.Date == departure.Date)
//                 .Select(r => new HotelRoomDto
//                 {
//                     HotelRoomId = r.HotelRoomId,
//                     RoomId = r.RoomId,
//                     Name = r.Name,
//                     Capacity = r.Capacity,
//                     Price = r.Price,
//                     MealPlan = r.MealPlan,
//                     Refundable = r.Refundable
//                 })
//                 .ToListAsync();

//             if (!rooms.Any())
//             {
//                 Console.WriteLine($"[HotelRoomController] No rooms found for externalHotelId: {externalHotelId} between {arrival} and {departure}");
//                 return NotFound();
//             }

//             return rooms;
//         }
//     }
// }