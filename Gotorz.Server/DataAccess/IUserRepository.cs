using System.Security.Claims;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// Defines the contract for user-related operations such as registration, login, retrieval, update, and deletion.
    /// </summary>
    /// <remarks>
    /// This abstraction allows different implementations for managing application users, typically using ASP.NET Core Identity.
    /// </remarks>
    /// <author>Eske</author>
    public interface IUserRepository
    {
        /// <summary>
        /// Registers a new user with the specified password and role.
        /// </summary>
        /// <param name="user">The user entity to register.</param>
        /// <param name="password">The user's plain-text password.</param>
        /// <param name="role">The role to assign to the new user.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> representing the result of the registration process.
        /// </returns>
        Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role);

        /// <summary>
        /// Attempts to sign in the user using the provided email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain-text password.</param>
        /// <returns>
        /// A <see cref="SignInResult"/> representing the result of the login attempt.
        /// </returns>
        Task<SignInResult> LoginAsync(string email, string password);

        /// <summary>
        /// Signs out the currently logged-in user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task LogoutAsync();

        /// <summary>
        /// Retrieves the currently authenticated user based on the provided claims principal.
        /// </summary>
        /// <param name="userPrincipal">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
        /// <returns>The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ApplicationUser?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retrieves the claims associated with a user, including roles.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// A list of <see cref="ClaimDto"/> containing claims and roles.
        /// </returns>
        Task<List<Claim>> GetClaimsAsync(ApplicationUser user);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Deletes a user from the system based on their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>An <see cref="IdentityResult"/> indicating whether the deletion succeeded.</returns>
        Task<IdentityResult> DeleteUserAsync(string userId);

        /// <summary>
        /// Updates the user information for a given user ID, and based on the given parameters.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<(bool Success, string? Error)> UpdateUserAsync(string userId, string email, string firstName, string lastName, string phoneNumber);

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns></returns>
        Task<List<ApplicationUser>> GetAllUsersAsync();


    }
}
