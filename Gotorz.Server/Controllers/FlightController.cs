using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;

namespace Gotorz.Server.Controllers;

/// <summary>
/// API controller for flight-related data.
/// </summary>
/// <remarks>
/// Handles incoming HTTP requests related to flight searches.
/// Delegates data retrieval to <see cref="IFlightService"/>.
/// </remarks>
[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;
    private readonly ISimpleKeyRepository<Airport> _airportRepository;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FlightController"/> class.
    /// </summary>
    /// <param name="flightService">The <see cref="IFlightService"/> used for fetching flight-related data.</param>
    /// <param name="airportRepository">The <see cref="ISimpleKeyRepository<Airport>"/> used to access
    /// <see cref="Airport"/> data in the database</param>
    public FlightController(IFlightService flightService, ISimpleKeyRepository<Airport> airportRepository)
    {
        _flightService = flightService;
        _airportRepository = airportRepository;
    }

    /// <summary>
    /// Retrieves names of all <see cref="Airport"/> entities in the database
    /// </summary>
    /// <returns>A collection of <see cref="Airport"/> names.</returns>
    [HttpGet("airports")]
    public IEnumerable<Airport>? GetAllAirports()
    {
        return _airportRepository.GetAll();
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that calls <see cref="FlightService.GetAirport(string)"/>
    /// to retrieve a single matching <see cref="Airport"/>.
    /// </summary>
    /// <param name="airport">The search term to match airport names against in <see cref="FlightService.GetAirport(string)"/>.</param>
    /// <returns>An <see cref="Airport"/> entity if exactly one match is found, otherwise <c>null</c>.</returns>
    [HttpGet("airport")]
    public async Task<IActionResult> GetAirport(string airport)
    {
        var airports = await _flightService.GetAirport(airport);

        if (airports == null) return BadRequest("Something went wrong");
        else if ( airports.Count == 0 ) return BadRequest("No airports were found");
        else if ( airports.Count > 1 ) return BadRequest("More than one airport was found");
        else
        {
            _airportRepository.Add(airports[0]);
            return Ok($"Successfully added {airports[0].LocalizedName} to database");
        }
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that calls <see cref="FlightService.GetFlights(string, string, string)"/>
    /// to retrieve a list of matching <see cref="Flight"/> entities.
    /// </summary>
    /// <param name="date">The departure date to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <param name="departureAirport">The departure airport to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <param name="arrivalAirport">The arrival airport to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <returns>A list of <see cref="Flight"/> entities matching the specified parameters.</returns>
    [HttpGet("flights")]
    public async Task<List<Flight>> GetFlights([FromQuery] string? date, [FromQuery] string departureAirport, [FromQuery] string arrivalAirport)
    {        
        // Ensure departure airport exists in database. If not, add to database with GetAirport.
        var _departureAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == departureAirport);
        if (_departureAirport == null)
        {
            var result = await GetAirport(departureAirport);
            if (result is OkObjectResult okResult)
            {
                _departureAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == departureAirport);
            }
            else return new List<Flight>();
        }

        // Ensure arrival airport exists in database. If not, add to database with GetAirport.
        var _arrivalAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == arrivalAirport);
        if (_arrivalAirport == null)
        {
            var result = await GetAirport(arrivalAirport);
            if (result is OkObjectResult okResult)
            {
                _arrivalAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == arrivalAirport);
            }
            else return new List<Flight>();
        }

        // Define date
        DateOnly? _date = null;
        if (date != null) _date = DateOnly.Parse(date);

        // Retrieve flights
        if (_departureAirport != null && _arrivalAirport != null)
        {
            List<Flight> flights = await _flightService.GetFlights(_date, _departureAirport, _arrivalAirport);
            
            if (flights == null) return new List<Flight>();
            else if ( flights.Count == 0 ) return new List<Flight>();
            else return flights;
        }
        return new List<Flight>();
    }
}