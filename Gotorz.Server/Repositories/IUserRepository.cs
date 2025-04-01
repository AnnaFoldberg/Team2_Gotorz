using System.Security.Claims;
using Gotorz.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace Gotorz.Server.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role);
        Task<SignInResult> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IdentityResult> UpdateUserAsync(string userId, string newEmail);
        Task<IdentityResult> DeleteUserAsync(string userId);

    }

}
