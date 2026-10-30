using Microsoft.AspNetCore.Authentication;

namespace Serenity.Testing;

public class MockAuthenticationService(Func<HttpContext, string, Task<AuthenticateResult>> authenticateAsync) : IAuthenticationService
{
    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    {
        return authenticateAsync?.Invoke(context, scheme);
    }

    public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal,
        AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }
}