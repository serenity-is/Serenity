using Microsoft.AspNetCore.DataProtection;

namespace Serenity.Extensions;

public class DefaultElevationHandlerTests
{
    private const string ElevationPurpose = "Elevation";

    private class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = now;
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private static DefaultHttpContext CreateContext(string? identifier)
    {
        var context = new DefaultHttpContext();
        if (identifier != null)
        {
            var identity = new GenericIdentity("user", "Test");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier));
            context.User = new ClaimsPrincipal(identity);
        }

        return context;
    }

    private static string CreateToken(IDataProtectionProvider provider, DateTime expires, int userId)
    {
        return provider.CreateProtector(ElevationPurpose).ProtectBinary(bw =>
        {
            bw.Write(expires.ToBinary());
            bw.Write(userId);
        });
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var provider = new EphemeralDataProtectionProvider();
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultElevationHandler(new NullRequestContext(), null!, provider));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultElevationHandler(new NullRequestContext(), new MockHttpContextAccessor(), null!));
    }

    [Fact]
    public void AppendElevationTokenToCookies_Throws_When_No_HttpContext()
    {
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            new MockHttpContextAccessor(), new EphemeralDataProtectionProvider());
        Assert.Throws<ArgumentNullException>(() => handler.AppendElevationTokenToCookies());
    }

    [Fact]
    public void AppendElevationTokenToCookies_Throws_When_No_Identifier()
    {
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext(null) };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, new EphemeralDataProtectionProvider());
        Assert.Throws<ArgumentNullException>(() => handler.AppendElevationTokenToCookies());
    }

    [Fact]
    public void AppendElevationTokenToCookies_Appends_Cookie()
    {
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, new EphemeralDataProtectionProvider());

        handler.AppendElevationTokenToCookies();

        var setCookie = accessor.HttpContext!.Response.Headers.SetCookie.ToString();
        Assert.Contains("ElevationToken=", setCookie);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateElevationToken_Throws_When_No_HttpContext()
    {
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            new MockHttpContextAccessor(), new EphemeralDataProtectionProvider());
        Assert.Throws<ArgumentNullException>(() => handler.ValidateElevationToken());
    }

    [Fact]
    public void ValidateElevationToken_Throws_When_Token_Missing()
    {
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, new EphemeralDataProtectionProvider());

        var error = Assert.Throws<ValidationError>(() => handler.ValidateElevationToken());
        Assert.Equal("RequiresElevation", error.ErrorCode);
    }

    [Fact]
    public void ValidateElevationToken_Accepts_Valid_Token()
    {
        var provider = new EphemeralDataProtectionProvider();
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, provider, new FixedTimeProvider(DateTimeOffset.UtcNow));

        var token = CreateToken(provider, DateTime.UtcNow.AddMinutes(10), 5);
        accessor.HttpContext!.Request.Headers.Cookie = "ElevationToken=" + Uri.EscapeDataString(token);

        handler.ValidateElevationToken();
    }

    [Fact]
    public void ValidateElevationToken_Throws_For_Expired_Token()
    {
        var provider = new EphemeralDataProtectionProvider();
        var timeProvider = new FixedTimeProvider(DateTimeOffset.UtcNow);
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, provider, timeProvider);

        var token = CreateToken(provider, timeProvider.Now.AddMinutes(-1).DateTime, 5);
        accessor.HttpContext!.Request.Headers.Cookie = "ElevationToken=" + Uri.EscapeDataString(token);

        var error = Assert.Throws<ValidationError>(() => handler.ValidateElevationToken());
        Assert.Equal("Token expired", error.Message);
    }

    [Fact]
    public void ValidateElevationToken_Throws_For_Invalid_Token()
    {
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, new EphemeralDataProtectionProvider());

        accessor.HttpContext!.Request.Headers.Cookie = "ElevationToken=invalidtoken";

        var error = Assert.Throws<ValidationError>(() => handler.ValidateElevationToken());
        Assert.Equal("Invalid elevation token", error.Message);
    }

    [Fact]
    public void ValidateElevationToken_Throws_For_Different_User()
    {
        var provider = new EphemeralDataProtectionProvider();
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("6") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, provider, new FixedTimeProvider(DateTimeOffset.UtcNow));

        var token = CreateToken(provider, DateTime.UtcNow.AddMinutes(10), 5);
        accessor.HttpContext!.Request.Headers.Cookie = "ElevationToken=" + Uri.EscapeDataString(token);

        var error = Assert.Throws<ValidationError>(() => handler.ValidateElevationToken());
        Assert.Equal("Invalid elevation token", error.Message);
    }

    [Fact]
    public void DeleteToken_Throws_When_No_HttpContext()
    {
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            new MockHttpContextAccessor(), new EphemeralDataProtectionProvider());
        Assert.Throws<ArgumentNullException>(() => handler.DeleteToken());
    }

    [Fact]
    public void DeleteToken_Deletes_Cookie()
    {
        var accessor = new MockHttpContextAccessor { HttpContext = CreateContext("5") };
        var handler = new DefaultElevationHandler(new NullRequestContext(),
            accessor, new EphemeralDataProtectionProvider());

        handler.DeleteToken();

        var setCookie = accessor.HttpContext!.Response.Headers.SetCookie.ToString();
        Assert.Contains("ElevationToken=", setCookie);
    }
}

