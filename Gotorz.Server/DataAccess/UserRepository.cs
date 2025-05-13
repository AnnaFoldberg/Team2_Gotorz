using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// Provides methods for managing user accounts including registration, login, logout, and user queries.
    /// </summary>
    /// <remarks>
    /// Wraps ASP.NET Core Identity functionality to simplify account management logic.
    /// </remarks>
    /// <author>Eske</author>
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{ApplicationUser}"/> for user-related operations.</param>
        /// <param name="signInManager">The <see cref="SignInManager{ApplicationUser}"/> for authentication operations.</param>
        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Registers a new user with the specified role.
        /// </summary>
        /// <param name="user">The user entity to be registered.</param>
        /// <param name="password">The plain-text password for the user.</param>
        /// <param name="role">The role to assign to the user.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating success or containing validation errors.
        /// Returns a custom error if a user with the same email already exists.
        /// </returns>
        public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser is not null)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "A user with that email already exists."
                });

                return error;
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                    return roleResult;
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));

            return result;
        }

        /// <summary>
        /// Attempts to log in the user using the provided credentials.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain-text password.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the outcome of the login attempt.</returns>
        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(email, password, true, false);
        }

        /// <summary>
        /// Signs out the currently logged-in user.
        /// </summary>
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Retrieves the currently authenticated user based on the provided claims principal.
        /// </summary>
        /// <param name="userPrincipal">The <see cref="ClaimsPrincipal"/> representing the current user context.</param>
        /// <returns>The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            return await _userManager.GetUserAsync(userPrincipal);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Retrieves the claims associated with a user, including roles.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var allClaims = claims.ToList();
            allClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return allClaims;
        }


        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to find.</param>
        /// <returns>The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Deletes a user from the system based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating whether the deletion succeeded or failed.
        /// </returns>
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed();

            return await _userManager.DeleteAsync(user);
        }

        /// <summary>
        /// Updates the user information for a given user ID, and based on the given parameters
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? Error)> UpdateUserAsync(string userId, string email, string firstName, string lastName, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null && existingUser.Id != user.Id)
                return (false, "Email is already in use.");

            user.Email = email;
            user.UserName = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? (true, null) : (false, result.Errors.FirstOrDefault()?.Description ?? "Update failed.");
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
