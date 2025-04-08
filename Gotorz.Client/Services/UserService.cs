using Gotorz.Shared.Models;
using System.Net.Http.Json;

namespace Gotorz.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync()
        {
            return await _http.GetFromJsonAsync<CurrentUserDto>("api/account/currentuser");
        }
    }
}
