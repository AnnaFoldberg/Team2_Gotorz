using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTOs;
using AutoMapper;
using System.Linq;

namespace Gotorz.Server.Controllers;

/// <summary>
/// API controller for booking-related data.
/// </summary>
/// <remarks>Handles incoming HTTP requests related to bookings.</remarks>
/// <author>Anna</author>
[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<HolidayPackage> _holidayPackageRepository;
    private readonly IHolidayBookingRepository _holidayBookingRepository;
    private readonly IRepository<Traveller> _travellerRepository;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingController"/> class.
    /// </summary>
    /// <param name="mapper">The <see cref="IMapper"/> used for mapping DTOs to Models.</param>
    /// <param name="holidayPackageRepository">The <see cref="IRepository<HolidayPackage>"/> used to access
    /// <see cref="HolidayPackage"/> data in the database.</param>
    /// <param name="holidayBookingReposiotry">The <see cref="IHolidayBookingRepository"/> used to access
    /// <see cref="HolidayBooking"/> data in the database.</param>
    /// /// <param name="travellerRepository">The <see cref="IRepository<Traveller>"/> used to access
    /// <see cref="Traveller"/> data in the database.</param>
    public BookingController(IMapper mapper, IRepository<HolidayPackage> holidayPackageRepository,
        IHolidayBookingRepository holidayBookingReposiotry, IRepository<Traveller> travellerRepository)
    {
        _mapper = mapper;
        _holidayPackageRepository = holidayPackageRepository;
        _holidayBookingRepository = holidayBookingReposiotry;
        _travellerRepository = travellerRepository;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that retrieves a <see cref="HolidayBooking"/>
    /// entity matching the specified <paramref name="bookingReference"/> from the database.
    /// </summary>
    /// <param name="bookingReference">The <c>BookingReference</c> to match holiday bookings against.</param>
    /// <returns>The <see cref="HolidayBookingDto"/> entity matching the specified <paramref name="BookingReference"</>.</returns>
    [HttpGet("holiday-booking")]
    public async Task<HolidayBookingDto?> GetHolidayBookingAsync(string bookingReference)
    {
        var holidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(bookingReference);
        var holidayBookingDto = _mapper.Map<HolidayBookingDto>(holidayBooking);
        return holidayBookingDto;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP POST that adds a <see cref="HolidayBooking"/> entity to the database.
    /// </summary>
    /// <param name="holidayBooking">The <see cref="HolidayBookingDto"/> object representing the holiday booking to be added.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if the <see cref="HolidayBooking"/> entity was
    /// added to the database successfully, otherwise <c>BadRequest</c>.</returns>
    [HttpPost("holiday-booking")]
    public async Task<IActionResult> PostHolidayBookingAsync(HolidayBookingDto holidayBooking)
    {
        var _holidayBooking = _mapper.Map<HolidayBooking>(holidayBooking);
        _holidayBooking.HolidayPackageId = 2; // ONLY TEMPORARY UNTIL WE HAVE MERGED BRANCHES!!!!!!!!!!!!!!!!!!!!!!
        await _holidayBookingRepository.AddAsync(_holidayBooking);
        return Ok($"Successfully added holiday booking to database");
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that retrieves a <see cref="Traveller"/>
    /// entities matching the specified <paramref name="bookingReference"/> from the database.
    /// </summary>
    /// <param name="bookingReference">The <c>BookingReference</c> to match travellers against.</param>
    /// <returns>The <see cref="TravellerDto"/> entities matching the specified <paramref name="BookingReference"</>.</returns>
    [HttpGet("travellers")]
    public async Task<IEnumerable<TravellerDto>?> GetTravellersAsync(string bookingReference)
    {
        var holidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(bookingReference);
        var holidayBookingId = holidayBooking.HolidayBookingId;
        var travellers = (await _travellerRepository.GetAllAsync()).Where(t => t.HolidayBookingId == holidayBookingId);
        var travellerDtos = _mapper.Map<IEnumerable<TravellerDto>>(travellers);
        return travellerDtos;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP POST that adds <see cref="Traveller"/> entities to the database.
    /// </summary>
    /// <param name="travellers">The <see cref="TravellerDto"/> objects representing the travellers to be added.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if the <see cref="Traveller"/> entities were
    /// added to the database successfully, otherwise <c>BadRequest</c>.</returns>
    [HttpPost("travellers")]
    public async Task<IActionResult> PostTravellersAsync(List<TravellerDto> travellers)
    {
        if (travellers == null || travellers.Count == 0)
        {
            return BadRequest("No travellers were provided");
        }

        foreach (var traveller in travellers)
        {
            var holidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(traveller.HolidayBooking.BookingReference);

            var _traveller = _mapper.Map<Traveller>(traveller);
            _traveller.HolidayBookingId = holidayBooking.HolidayBookingId;
            await _travellerRepository.AddAsync(_traveller);
        }
        return Ok($"Successfully added {travellers.Count()} traveller(s) to database");
    }
}