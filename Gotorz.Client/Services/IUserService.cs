using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    /// <summary>
    /// Provides methods for accessing information about the currently authenticated user in the client.
    /// </summary>
    /// <remarks>
    /// Typically used in Blazor WebAssembly to retrieve and interact with identity and role information exposed by the backend.
    /// </remarks>
    /// <author>Eske</author>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves the full current user object from the backend.
        /// </summary>
        /// <returns>
        /// A <see cref="CurrentUserDto"/> containing user details and claims, or <c>null</c> if the user is not authenticated.
        /// </returns>
        Task<CurrentUserDto?> GetCurrentUserAsync();

        /// <summary>
        /// Determines whether the user is currently authenticated.
        /// </summary>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        Task<bool> IsLoggedInAsync();

        /// <summary>
        /// Checks whether the current user is in the specified role.
        /// </summary>
        /// <param name="role">The name of the role to check for (e.g., "admin", "sales").</param>
        /// <returns><c>true</c> if the user has the role; otherwise, <c>false</c>.</returns>
        Task<bool> IsUserInRoleAsync(string role);

        /// <summary>
        /// Retrieves the role of the currently authenticated user based on claims.
        /// </summary>
        /// <returns>
        /// The user's role as a string if available; otherwise, <c>null</c>.
        /// </returns>
        Task<string?> GetUserRoleAsync();

        /// <summary>
        /// Retrieves a user's profile information by their unique identifier
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A <see cref="CurrentUserDto"/> containing user details if successful; otherwise, <c>null</c>.
        /// </returns>
        Task<CurrentUserDto?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retrieves the unique identifier of the current user.
        /// </summary>
        /// <returns>The user ID as a string, or <c>null</c> if not found.</returns>
        Task<string?> GetUserIdAsync();

        /// <summary>
        /// Retrieves the user's first name, or falls back to email if not available.
        /// </summary>
        /// <returns>The first name or email address of the current user.</returns>
        Task<string?> GetFirstNameAsync();

        /// <summary>
        /// Retrieves the user's last name.
        /// </summary>
        /// <returns>The last name of the current user, or <c>null</c> if not set.</returns>
        Task<string?> GetLastNameAsync();

        /// <summary>
        /// Retrieves the user's email address.
        /// </summary>
        /// <returns>The email address of the current user, or <c>null</c> if not set.</returns>
        Task<string?> GetEmailAsync();
    }
}
