using System.Security.Principal;

namespace Serenity.TestUtils;

public class MockUserAccessor : IUserAccessor
{
    private readonly Func<ClaimsPrincipal> getUser;
    private readonly Func<string> getUsername;
    private readonly Func<string> getIdentifier;
    private readonly Action<ClaimsIdentity> configureIdentity;

    public MockUserAccessor(Func<string> getUsername, Func<string> getIdentifier = null, Action<ClaimsIdentity> configureIdentity = null)
    {
        this.getUsername = getUsername ?? throw new ArgumentNullException(nameof(getUsername));
        this.getIdentifier = getIdentifier;
        this.configureIdentity = configureIdentity;
    }

    public MockUserAccessor(Func<ClaimsPrincipal> getUser)
    {
        this.getUser = getUser ?? throw new ArgumentNullException(nameof(getUser));
    }

    public static string MockUserId(string username)
    {
        return username.GetHashCode().ToString();
    }

    public MockUserAccessor(Func<string> getUsername)
    {
        ArgumentNullException.ThrowIfNull(getUsername);

        getUser = () =>
        {
            var username = getUsername();
            if (username == null)
                return null;

            var identity = new GenericIdentity(username, "Testing");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username.GetHashCode().ToString()));
            return new ClaimsPrincipal(identity);
        };
    }

    public ClaimsPrincipal User
    {
        get
        {
            if (getUser != null)
                return getUser();

            var username = getUsername();
            if (username == null)
                return null;

            var identity = new GenericIdentity(username, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            if (getIdentifier != null)
            {
                var identifier = getIdentifier();
                if (identifier != null)
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, getIdentifier()));
            }

            configureIdentity?.Invoke(identity);

            return claimsPrincipal;
        }
    }
}
