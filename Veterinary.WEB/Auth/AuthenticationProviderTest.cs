using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Veterinary.WEB.Auth;

public class AuthenticationProviderTest : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(3000);

        var oapUser = new ClaimsIdentity(
        [
            new Claim("FirstName", "Orlando"),
            new Claim("LastName", "Oap"),
            new Claim(ClaimTypes.Name, "oap@yopmail.com"),
            new Claim(ClaimTypes.Role, "Admin")
        ], "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(oapUser)));
    }
}
