using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared;
using Gotorz.Server.Services;
using Gotorz.Shared.Models;
using System.Diagnostics;

namespace Gotorz.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;
    private Airport _airport;
    
    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet("flights/auto-complete")]
    public async Task<IActionResult> GetAutoComplete(string airport)
    {
        var airports = await _flightService.GetAutoComplete(airport);

        if ( airports.Count > 1 ) return BadRequest("Multiple airports were found");
        else if ( airports.Count == 0 ) return BadRequest("No airport was found");
        else if ( airports == null ) return BadRequest("Something went wrong");
        
        Trace.WriteLine($"Airports: {airports[0]}");
        Trace.WriteLine($"Airport SkyId: {airports[0].SkyId}");
        Trace.WriteLine($"Airport EntityId: {airports[0].EntityId}");
        _airport = airports[0];
        return Ok(airports);
    }

    [HttpGet("flights/one-way")]
    public async Task<IActionResult> GetOneWay([FromQuery] string date, [FromQuery] string departureAirport, [FromQuery] string arrivalAirport)
    {
        // New York J.F. Kennedy
        // Hvis den findes i databasen, hent Airport-objekt derfra, noget med airports.GetAll fra et repository
        // Hvis ikke, brug GetAutoComplete() - Find ud af, hvad der er den korrekte måde at gøre det på
        await GetAutoComplete(departureAirport);
        Airport _departureAirport = _airport;
        await GetAutoComplete(arrivalAirport);
        Airport _arrivalAirport = _airport;
        
        DateOnly _date = DateOnly.Parse(date);

        if (_date != null && _departureAirport != null && _arrivalAirport != null)
        {
            Trace.WriteLine($"_departureAirport.SkyId: {_departureAirport.SkyId}");
            Trace.WriteLine($"_departureAirport.EntityId: {_departureAirport.EntityId}");
            Trace.WriteLine($"_arrivalAirport.SkyId: {_arrivalAirport.SkyId}");
            Trace.WriteLine($"_arrivalAirport.EntityId: {_arrivalAirport.EntityId}");

            List<Flight> flights = await _flightService.GetOneWay(_date, _departureAirport, _arrivalAirport);
            if ( flights.Count == 0 ) return BadRequest("No flights were found");
            else if (flights == null) return BadRequest("Something went wrong2");
            else return Ok(flights);
        }

        return BadRequest($"Something went wrong with the parameters"); // {_date}, {_departureAirport.LocalizedName}, {_arrivalAirport.LocalizedName}
    }
}