using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Gotorz.Shared.Models;
using System.Security.Claims;

public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;

    public ServerAuthenticationStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var user = await _http.GetFromJsonAsync<CurrentUserDto>("api/account/currentuser");

            if (user is { IsAuthenticated: true, Name: not null })
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
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
