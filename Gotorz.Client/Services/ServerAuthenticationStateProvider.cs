using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services;

/// <summary>
/// Provides the authentication state for the Blazor WebAssembly client by retrieving user data from the backend server.
/// </summary>
/// <remarks>
/// This implementation fetches the current user via an HTTP call to the server's identity endpoint and builds a ClaimsPrincipal for the Blazor authentication system.
/// </remarks>
/// <author>Eske</author>
public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="http"></param>
    public ServerAuthenticationStateProvider(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Retrieves the current authentication state of the user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{AuthenticationState}"/> representing the asynchronous operation. 
    /// Returns a populated <see cref="ClaimsPrincipal"/> if the user is authenticated; otherwise, an empty identity.
    /// </returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var user = await _http.GetFromJsonAsync<CurrentUserDto>("api/account/currentuser");

            if (user is { IsAuthenticated: true, Email: not null })
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

                foreach (var claim in user.Claims)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }

                var identity = new ClaimsIdentity(claims, "serverauth");
                var principal = new ClaimsPrincipal(identity);
                return new AuthenticationState(principal);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get user info: " + ex.Message);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
