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

            // Convert to DTOs if needed, or return full entity
            // var roomDtos = rooms.Select(r => new HotelRoomDto
            // {
            //     HotelRoomId = r.HotelRoomId,
            //     ExternalRoomId = r.ExternalRoomId,
            //     Name = r.Name,
            //     Capacity = r.Capacity,
            //     Price = r.Price,
            //     MealPlan = r.MealPlan,
            //     Refundable = r.Refundable
            // }).ToList();

            var roomDtos = _mapper.Map<List<HotelRoomDto>>(rooms);
            return Ok(roomDtos);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetSearchHistory()
        {
            var history = await _hotelService.GetSearchHistory();
            return Ok(history);
        }
    }
}