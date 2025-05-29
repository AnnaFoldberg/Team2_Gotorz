using Gotorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Services;
using AutoMapper;

namespace Gotorz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IMapper _mapper;


        public HotelController(IHotelService hotelService, IMapper mapper)
        {
            _hotelService = hotelService;
            _mapper = mapper;
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

            if (string.IsNullOrWhiteSpace(externalHotelId))
                return BadRequest("Hotel ID is required.");

            var rooms = await _hotelService.GetHotelRoomsAsync(externalHotelId, arrival, departure);
            if (rooms == null || !rooms.Any())
                return NotFound();

            var roomDtos = _mapper.Map<List<HotelRoomDto>>(rooms);
            return Ok(roomDtos);
        }
    }
}