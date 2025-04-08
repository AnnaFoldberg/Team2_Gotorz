using Gotorz.Shared.Models;

namespace Gotorz.Client.Services
{
    public interface IUserService
    {
        Task<CurrentUserDto?> GetCurrentUserAsync();
    }
}
