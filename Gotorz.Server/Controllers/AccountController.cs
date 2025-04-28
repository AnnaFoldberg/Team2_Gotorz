using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Repositories;
using Gotorz.Shared.DTO;
using Microsoft.AspNetCore.Authorization;

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

        var userDto = new CurrentUserDto
        {
            Email = user?.UserName,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Claims = User.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
        };

        return Ok(userDto);
    }

    [HttpGet("user/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var claims = await _userRepository.GetClaimsAsync(user);

        var userDto = new CurrentUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsAuthenticated = true,
            Claims = claims
        };

        return Ok(userDto);
    }

}
