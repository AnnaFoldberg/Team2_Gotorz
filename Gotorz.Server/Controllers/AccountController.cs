using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Models;
using Gotorz.Server.Models;
using Gotorz.Server.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AccountController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "customer" : dto.Role;

        var result = await _userRepository.RegisterAsync(user, dto.Password, role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userRepository.LoginAsync(dto.Email, dto.Password);

        if (!result.Succeeded)
            return Unauthorized("Invalid login");

        return Ok("Logged in");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _userRepository.LogoutAsync();
        return Ok("Logged out");
    }

    [HttpGet("currentuser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userRepository.GetCurrentUserAsync(User);

        var userDto = new CurrentUserDto
        {
            Name = user?.UserName,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Claims = User.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
        };

        return Ok(userDto);
    }


}
