using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTOs;
using AutoMapper;
using System.Linq;
using System.Runtime.InteropServices;

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
    private readonly IBookingService _bookingService;
    private readonly IRepository<HolidayPackage> _holidayPackageRepository;
    private readonly IHolidayBookingRepository _holidayBookingRepository;
    private readonly IRepository<Traveller> _travellerRepository;
    private readonly IUserRepository _userRepository;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingController"/> class.
    /// </summary>
    /// <param name="mapper">The <see cref="IMapper"/> used for mapping DTOs to Models.</param>
    /// <param name="holidayPackageRepository">The <see cref="IRepository<HolidayPackage>"/> used to access
    /// <see cref="HolidayPackage"/> data in the database.</param>
    /// <param name="holidayBookingRepository">The <see cref="IHolidayBookingRepository"/> used to access
    /// <see cref="HolidayBooking"/> data in the database.</param>
    /// <param name="travellerRepository">The <see cref="IRepository<Traveller>"/> used to access
    /// <see cref="Traveller"/> data in the database.</param>
    /// <param name="userRepository">The <see cref="IUserRepository"/> used to access
    /// <see cref="ApplicationUser"/> data in the database.</param>
    public BookingController(IMapper mapper, IBookingService bookingService,
        IRepository<HolidayPackage> holidayPackageRepository, IHolidayBookingRepository holidayBookingRepository,
        IRepository<Traveller> travellerRepository, IUserRepository userRepository)
    {
        _mapper = mapper;
        _bookingService = bookingService;
        _holidayPackageRepository = holidayPackageRepository;
        _holidayBookingRepository = holidayBookingRepository;
        _travellerRepository = travellerRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that generates the next booking reference
    /// for <see cref="HolidayBooking"/> entities.
    /// </summary>
    /// <returns>A <c>string</c> representing the next available booking reference.</returns>
    [HttpGet("booking-reference")]
    public async Task<string> GetNextBookingReferenceAsync()
    {
        var bookingReference = await _bookingService.GenerateNextBookingReferenceAsync();
        return bookingReference;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that retrieves all <see cref="HolidayBooking"/>
    /// entities from the database.
    /// </summary>
    /// <returns>A collection of <see cref="HolidayBookingDto"/> entities.</returns>
    [HttpGet("holiday-bookings")]
    public async Task<IEnumerable<HolidayBookingDto>?> GetAllHolidayBookingsAsync()
    {
        var holidayBookings = await _holidayBookingRepository.GetAllAsync();

        if (holidayBookings == null) return null;

        // Attach customer
        foreach (var holidayBooking in holidayBookings)
        {
            var customer = await _userRepository.GetUserByIdAsync(holidayBooking.CustomerId);
            if (customer == null) return null;
            holidayBooking.Customer = customer;
        }
 
        var holidayBookingDtos = _mapper.Map<List<HolidayBookingDto>>(holidayBookings);
        return holidayBookingDtos;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that retrieves a collection of <see cref="HolidayBooking"/>
    /// entities matching the specified <paramref name="email"/> from the database.
    /// </summary>
    /// <param name="email">The <c>Email</c> to match holiday bookings against.</param>
    /// <returns>A collection of <see cref="HolidayBookingDto"/> entities matching the specified <paramref name="email"</>.</returns>
    [HttpGet("customer-holiday-bookings")]
    public async Task<IEnumerable<HolidayBookingDto>?> GetCustomerHolidayBookingsAsync(string email)
    {
        var customer = await _userRepository.GetUserByEmailAsync(email);
        if (customer == null) return null;

        var userHolidayBookings = await _holidayBookingRepository.GetByCustomerIdAsync(customer.Id);
        if (userHolidayBookings == null) return null;

        // Attach customer
        foreach (var holidayBooking in userHolidayBookings) holidayBooking.Customer = customer;

        var userHolidayBookingDtos = _mapper.Map<List<HolidayBookingDto>>(userHolidayBookings);
        return userHolidayBookingDtos;
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
        if (holidayBooking == null) return null;

        // Attach customer
        var customer = await _userRepository.GetUserByIdAsync(holidayBooking.CustomerId);
        if (customer == null) return null;
        holidayBooking.Customer = customer;

        var holidayBookingDto = _mapper.Map<HolidayBookingDto>(holidayBooking);
        return holidayBookingDto;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP PATCH that updates a <see cref="HolidayBooking"/> entity's status in the database.
    /// </summary>
    /// <param name="holidayBooking">The <see cref="HolidayBookingDto"/> object representing the holiday booking to be updated.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if the <see cref="HolidayBooking"/> entity was
    /// updated in the database successfully, otherwise <c>BadRequest</c>.</returns>
    [HttpPatch("holiday-booking")]
    public async Task<IActionResult> PatchHolidayBookingStatusAsync(HolidayBookingDto holidayBookingPatch)
    {
        if (holidayBookingPatch == null)
        {
            return BadRequest("No holiday booking was provided");
        }

        // Check if a holiday booking with the same booking reference already exists
        var holidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(holidayBookingPatch.BookingReference);
        if (holidayBooking == null)
        {
            return BadRequest("The holiday booking does not exist in the database");
        }

        holidayBooking.Status = (int)holidayBookingPatch.Status;

        await _holidayBookingRepository.UpdateAsync(holidayBooking);
        return Ok($"Successfully updated holiday booking {holidayBooking.BookingReference}");
    }

    /// <summary>
    /// Defines an API endpoint for HTTP POST that adds a <see cref="HolidayBooking"/> entity to the database.
    /// </summary>
    /// <param name="holidayBookingDto">The <see cref="HolidayBookingDto"/> object representing the holiday booking to be added.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if the <see cref="HolidayBooking"/> entity was
    /// added to the database successfully, otherwise <c>BadRequest</c>.</returns>
    [HttpPost("holiday-booking")]
    public async Task<IActionResult> PostHolidayBookingAsync(HolidayBookingDto holidayBookingDto)
    {
        if (holidayBookingDto == null)
        {
            return BadRequest("No holiday booking was provided");
        }

        // Check if a holiday booking with the same booking reference already exists
        var matchingHolidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(holidayBookingDto.BookingReference);
        if (matchingHolidayBooking != null)
        {
            return BadRequest("A holiday booking with the same booking reference already exists in the database");
        }

        var holidayBooking = _mapper.Map<HolidayBooking>(holidayBookingDto);

        // Ensure holiday booking contains the correct HolidayPackageId 
        var holidayPackages = await _holidayPackageRepository.GetAllAsync();
        var holidayPackage = holidayPackages
            .FirstOrDefault(p => p.Title == holidayBookingDto.HolidayPackage.Title &&
            p.Description == holidayBookingDto.HolidayPackage.Description);

        if (holidayPackage == null )
        {
            return BadRequest("Holiday package linked to booking does not exist");
        }

        holidayBooking.HolidayPackageId = holidayPackage.HolidayPackageId;

        // Ensure holiday booking contains the correct CustomerId
        var user = await _userRepository.GetUserByEmailAsync(holidayBookingDto.Customer.Email);

        if (user == null)
        {
            return BadRequest("Customer linked to booking does not exist");
        }

        holidayBooking.CustomerId = user.Id;

        await _holidayBookingRepository.AddAsync(holidayBooking);
        return Ok($"Successfully added holiday booking {holidayBooking.BookingReference} to database");
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
        if (holidayBooking == null) return null;
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

        var bookingReference = travellers.First().HolidayBooking.BookingReference;

        var holidayBooking = await _holidayBookingRepository.GetByBookingReferenceAsync(bookingReference);
        if (holidayBooking == null)
        {
            return BadRequest($"No holiday booking found for booking reference '{bookingReference}'");
        }

        foreach (var traveller in travellers)
        {
            var _traveller = _mapper.Map<Traveller>(traveller);
            _traveller.HolidayBookingId = holidayBooking.HolidayBookingId;
            await _travellerRepository.AddAsync(_traveller);
        }
        return Ok($"Successfully added {travellers.Count()} traveller(s) to database");
    }
}