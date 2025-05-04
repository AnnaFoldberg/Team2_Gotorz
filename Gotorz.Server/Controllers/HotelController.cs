using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Models;
using Gotorz.Server.Services;

namespace Gotorz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetHotelsByCity(
            [FromQuery] string city,
            [FromQuery] string country,
            [FromQuery] DateTime arrival,
            [FromQuery] DateTime departure)
        {
            var result = await _hotelService.GetHotelsByCityName(city, country, arrival, departure);
            return Ok(result);
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetHotelRooms(
            [FromQuery] string externalHotelId,
            [FromQuery] DateTime arrival,
            [FromQuery] DateTime departure)
        {
            Console.WriteLine("🛎️ HotelController -> GetHotelRooms() called");
            if (string.IsNullOrWhiteSpace(externalHotelId))
            return BadRequest("Hotel ID is required.");

            var rooms = await _hotelService.GetHotelRoomsAsync(externalHotelId, arrival, departure);
            if (rooms == null || !rooms.Any())
            return NotFound(); // Not Empty
            return Ok(rooms);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetSearchHistory()
        {
            var history = await _hotelService.GetSearchHistory();
            return Ok(history);
        }
    }
}