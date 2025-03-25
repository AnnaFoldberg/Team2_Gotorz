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

        public async Task<List<Airport>> GetAutoCompleteAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://skyscanner89.p.rapidapi.com/flights/auto-complete?query=New%20York%20John%20F.%20Kennedy"),
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

        public async Task<List<Flight>> GetOneWay(DateOnly date, string originSkyId, int originEntityId, string destinationSkyId, int destinationEntityId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://skyscanner89.p.rapidapi.com/flights/one-way/list?date=2025-03-28&origin={originSkyId}&originId={originEntityId}&destination={destinationSkyId}&destinationId={destinationEntityId}"),
                Headers =
                {
                    { "x-rapidapi-key", "893d2209a2msh7e6ac6660891991p183337jsn16f52b6c3da3" },
                    { "x-rapidapi-host", "skyscanner89.p.rapidapi.com" },
                },
            };
        }
    }
}