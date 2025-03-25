using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gotorz.Shared;
using Gotorz.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Gotorz.Server.Services
{
    public interface IFlightService
    {
        Task<List<Airport>> GetAutoCompleteAsync();
        Task<List<Flight>> GetOneWay()
    }
}