﻿using Gotorz.Shared.DTOs;
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

        /// <summary>
        /// Registers a new user with the provided registration model.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto registerModel)
        {
            var response = await _http.PostAsJsonAsync("api/account/register", registerModel);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("DuplicateEmail") || errorJson.Contains("DuplicateUserName"))
                return (false, "This email is already in use");

            return (false, "Something unexpected happened");
        }

        /// <summary>
        /// Authenticates the user with the provided login credentials.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? ErrorMessage)> LoginAsync(LoginDto loginModel)
        {
            var response = await _http.PostAsJsonAsync("api/account/login", loginModel);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        /// <summary>
        /// Signs out the currently logged-in user.
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            await _http.PostAsync("api/account/logout", null);
        }

        /// <summary>
        /// Deletes the user with the specified unique identifier.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var response = await _http.DeleteAsync($"api/account/user/{userId}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes the currently logged-in user.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteCurrentUserAsync()
        {
            var response = await _http.DeleteAsync("api/account/user/self");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates the profile of the currently logged-in user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(UpdateUserDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/account/update-profile", dto);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        /// <summary>
        /// Updates the user information for a given user ID, and based on the given parameters.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? ErrorMessage)> UpdateUserByIdAsync(string userId, UpdateUserDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/account/update-user/{userId}", dto);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        /// <summary>
        /// Retrieves a list of all users in the system.
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("api/account/users")
                ?? new List<UserDto>();
        }


    }
}
