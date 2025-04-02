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
    }
}