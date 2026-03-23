using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Veterinary.WEB.Helpers;

namespace Veterinary.WEB.Auth;

public class AuthenticationProviderJWT(IJSRuntime jsRuntime, HttpClient httpClient)
    : AuthenticationStateProvider, ILoginService
{
    private const string TokenKey = "TOKEN_KEY";
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.GetLocalStorage(TokenKey);
        var tokenString = token?.ToString();

        if (string.IsNullOrWhiteSpace(tokenString))
        {
            return _anonymous;
        }

        return BuildAuthenticationState(tokenString);
    }

    public async Task LoginAsync(string token)
    {
        await _jsRuntime.SetLocalStorage(TokenKey, token);
        var authState = BuildAuthenticationState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.RemoveLocalStorage(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private AuthenticationState BuildAuthenticationState(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var claims = ParseClaimsFromJwt(token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        return jwtSecurityToken.Claims;
    }
}
