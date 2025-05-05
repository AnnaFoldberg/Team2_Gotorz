using Gotorz.Shared.DTOs;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Gotorz.Client.Services
{
    /// <summary>
    /// Provides methods for retrieving identity and role information about the currently authenticated user.
    /// </summary>
    /// <author>Eske</author>
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="http">The <see cref="HttpClient"/> used to call the backend identity API.</param>
        public UserService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Retrieves the full current user object from the backend.
        /// </summary>
        /// <returns>A <see cref="UserDto"/> containing user details and claims, or <c>null</c> if unauthenticated.</returns>
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<UserDto>("api/account/currentuser");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the user is currently authenticated.
        /// </summary>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsLoggedInAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.IsAuthenticated == true;
        }

        /// <summary>
        /// Checks whether the current user is in the specified role.
        /// </summary>
        /// <param name="role">The role to check for (e.g., "admin", "sales").</param>
        /// <returns><c>true</c> if the user has the specified role; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsUserInRoleAsync(string role)
        {
            var user = await GetCurrentUserAsync();
            return user?.Claims?.Any(c => c.Type == ClaimTypes.Role && c.Value == role) == true;
        }

        /// <summary>
        /// Retrieves the role of the currently authenticated user based on claims.
        /// </summary>
        /// <returns>
        /// The user's role as a string if available; otherwise, <c>null</c>.
        /// </returns>
        public async Task<string?> GetUserRoleAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Retrieves a user's profile information by their unique identifier
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A <see cref="UserDto"/> containing user details if successful; otherwise, <c>null</c>.
        /// </returns>
        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _http.GetFromJsonAsync<UserDto>($"api/account/user/{userId}");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the unique identifier of the current user.
        /// </summary>
        /// <returns>The user ID, or <c>null</c> if not available.</returns>
        public async Task<string?> GetUserIdAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Retrieves the user's first name, or falls back to their email address if not set.
        /// </summary>
        /// <returns>The user's first name or email address.</returns>
        public async Task<string?> GetFirstNameAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.FirstName ?? user?.Email;
        }

        /// <summary>
        /// Retrieves the user's last name.
        /// </summary>
        /// <returns>The user's last name, or <c>null</c> if not set.</returns>
        public async Task<string?> GetLastNameAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.LastName;
        }

        /// <summary>
        /// Retrieves the user's email address.
        /// </summary>
        /// <returns>The user's email address, or <c>null</c> if not available.</returns>
        public async Task<string?> GetEmailAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Email;
        }
    }
}
