using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Gotorz.Shared.DTOs;

namespace Gotorz.Server.Controllers;

/// <summary>
/// Handles user account operations such as registration, login, logout, and retrieving the current user.
/// </summary>
/// <remarks>
/// Acts as the API entry point for authentication and identity-related actions. 
/// Uses <see cref="IUserRepository"/> to manage user operations.
/// </remarks>
/// <author>Eske</author>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userRepository">The <see cref="IUserRepository"/> used to handle user-related operations.</param>
    public AccountController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user with the specified registration data.
    /// </summary>
    /// <param name="dto">The <see cref="RegisterDto"/> containing user information and credentials.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the registration process.
    /// Returns 200 OK if successful, otherwise 400 Bad Request with validation errors.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser { 
            UserName = dto.Email, 
            Email = dto.Email, 
            FirstName = dto.FirstName, 
            LastName = dto.LastName, 
            PhoneNumber = dto.PhoneNumber };
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "customer" : dto.Role;

        var result = await _userRepository.RegisterAsync(user, dto.Password, role);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("User registered");
    }

    /// <summary>
    /// Authenticates the user with the provided login credentials.
    /// </summary>
    /// <param name="dto">The <see cref="LoginDto"/> containing email and password.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the login attempt.
    /// Returns 200 OK if successful, otherwise 401 Unauthorized.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userRepository.LoginAsync(dto.Email, dto.Password);

        if (!result.Succeeded)
            return Unauthorized("Invalid login");

        return Ok("Logged in");
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> confirming the user has been logged out.
    /// </returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _userRepository.LogoutAsync();
        return Ok("Logged out");
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a <see cref="CurrentUserDto"/> with the user's identity and claims information.
    /// </returns>
    [HttpGet("currentuser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userRepository.GetCurrentUserAsync(User);

        var userDto = new UserDto
        {
            Email = user?.UserName,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            PhoneNumber = user?.PhoneNumber,
            Claims = User.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the <see cref="CurrentUserDto"/> if found; otherwise, 404 Not Found.
    /// </returns>
    [HttpGet("user/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var rawClaims = await _userRepository.GetClaimsAsync(user);
        var claims = rawClaims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList();

        var userDto = new UserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsAuthenticated = true,
            Claims = claims
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("user/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _userRepository.DeleteUserAsync(id);
        if (result.Succeeded)
            return Ok("User deleted");

        return BadRequest("Deletion failed");
    }

    /// <summary>
    /// Deletes the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("user/self")]
    [Authorize]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var currentUser = await _userRepository.GetCurrentUserAsync(User);
        if (currentUser == null) return Unauthorized();

        var result = await _userRepository.DeleteUserAsync(currentUser.Id);
        if (result.Succeeded)
            return Ok("Account deleted");

        return BadRequest("Failed to delete account");
    }

    /// <summary>
    /// Updates the profile of the currently authenticated user.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        var user = await _userRepository.GetCurrentUserAsync(User);
        if (user == null) return Unauthorized();

        var result = await _userRepository.UpdateUserAsync(user.Id, dto.Email, dto.FirstName, dto.LastName, dto.PhoneNumber);

        if (!result.Success)
            return BadRequest(result.Error ?? "Profile update failed.");

        return Ok("Profile updated");
    }

    /// <summary>
    /// Updates the profile of a user by their unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("update-user/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUserById(string id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userRepository.UpdateUserAsync(id, dto.Email, dto.FirstName, dto.LastName, dto.PhoneNumber);

        if (!result.Success)
            return BadRequest(result.Error ?? "Profile update failed.");

        return Ok("Profile updated");
    }

    /// <summary>
    /// Retrieves all users in the system.
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var rawClaims = await _userRepository.GetClaimsAsync(user);
            var claims = rawClaims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList();

            result.Add(new UserDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Claims = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
            });
        }

        return Ok(result);
    }


}
