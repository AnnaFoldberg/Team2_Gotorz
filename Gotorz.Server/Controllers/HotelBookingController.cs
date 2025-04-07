using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;

namespace Gotorz.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HotelBookingController : ControllerBase
{
    private readonly IHotelBookingService _service;

    public HotelBookingController(IHotelBookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddBooking([FromBody] HotelBooking booking)
    {
        await _service.AddHotelBookingAsync(booking);
        return Ok();
    }

   /* [HttpGet("{rejsepakkeId}")]
    public async Task<IActionResult> GetByRejsepakke(int rejsepakkeId)
    {
        var result = await _service.GetBookingsByRejsepakkeId(rejsepakkeId);
        return Ok(result);
    }
    */
}