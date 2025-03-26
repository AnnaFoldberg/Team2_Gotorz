using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Gotorz.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Gotorz.Shared.Models;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace Gotorz.Server.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FlightService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Airport>> GetAutoComplete(string airport)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://skyscanner89.p.rapidapi.com/flights/auto-complete?query={Uri.EscapeDataString(airport)}"),
                Headers =
                {
                    { "x-rapidapi-key", "893d2209a2msh7e6ac6660891991p183337jsn16f52b6c3da3" },
                    { "x-rapidapi-host", "skyscanner89.p.rapidapi.com" },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<AutoCompleteResponse>(body, options);

                if (result?.InputSuggest != null)
                {
                    var airports = result.InputSuggest
                        .Where(s => s.Navigation?.RelevantFlightParams != null)
                        .Select(s => s.Navigation.RelevantFlightParams)
                        .ToList();
                    
                    return airports;
                }

                return new List<Airport>();
            }
        }

        public async Task<List<Flight>> GetOneWay(DateOnly date, Airport departureAirport, Airport arrivalAirport)
        {
            Trace.WriteLine($"departureAirport.SkyId: {departureAirport.SkyId}");
            Trace.WriteLine($"departureAirport.EntityId: {departureAirport.EntityId}");
            Trace.WriteLine($"arrivalAirport.SkyId: {arrivalAirport.SkyId}");
            Trace.WriteLine($"arrivalAirport.EntityId: {arrivalAirport.EntityId}");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://skyscanner89.p.rapidapi.com/flights/one-way/list?origin={departureAirport.SkyId}&originId={departureAirport.EntityId}&destination={arrivalAirport.SkyId}&destinationId={arrivalAirport.EntityId}"),
                Headers =
                {
                    { "x-rapidapi-key", "893d2209a2msh7e6ac6660891991p183337jsn16f52b6c3da3" },
                    { "x-rapidapi-host", "skyscanner89.p.rapidapi.com" },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                JsonNode root = JsonNode.Parse(body);
                Trace.WriteLine($"root: {root}");
                Trace.WriteLine($"data: {root?["data"]}");
                Trace.WriteLine($"flightQuotes: {root?["flightQuotes"]}");
                JsonArray results = root?["data"]?["flightQuotes"]?["results"]?.AsArray();

                var flights = new List<Flight>();

                if (results != null)
                {            
                    Trace.WriteLine("results is not null!");
                    foreach (var result in results)
                    {
                        JsonNode content = result["content"];
                        if (content["direct"].GetValue<bool>() == false) break;

                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        
                        string id = results?["id"].ToString();
                        
                        string departureDate = content?["outboundLeg"]?["localDepartureDate"].ToString();
                        DateOnly _date = DateOnly.Parse(departureDate);

                        JsonNode departureAirportData = content?["outboundLeg"]?["originAirport"];
                        var _departureAirport = JsonSerializer.Deserialize<Airport>(departureAirportData, options);

                        JsonNode arrivalAirportData = content?["outboundLeg"]?["destinationAirport"];
                        var _arrivalAirport = JsonSerializer.Deserialize<Airport>(arrivalAirportData, options);

                        Trace.WriteLine($"Departure Airport: {_departureAirport}");
                        Trace.WriteLine($"Arrival Airport: {_arrivalAirport}");
                        Trace.WriteLine($"Date: {_date}");
                        
                        decimal _price = content["rawPrice"].GetValue<decimal>();

                        flights.Add(new Flight { FlightNumber = id, DepartureDate = _date, Price = _price, DepartureAirportId = _departureAirport.AirportId, ArrivalAirportId = _arrivalAirport.AirportId });
                    }
                }
                Trace.WriteLine("Fail!");
                return flights;
            }
        }
    } 
}

// "direct": false,
// "outboundLeg": {
//     "destinationAirport": {
//         "id": "95565050",
//         "name": "LHR",
//         "skyCode": "LHR",
//         "type": "Airport"
//     },
//     "localDepartureDate": "2025-04-27",
//     "localDepartureDateLabel": "Sun, Apr 27",
//     "originAirport": {
//         "id": "95565058",
//         "name": "JFK",
//         "skyCode": "JFK",
//         "type": "Airport"
//     }
// },
// "price": "$188",
// "rawPrice": 188.0