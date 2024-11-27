using System.Security.Principal;

namespace Serenity.Tests;

public class MockUserAccessor : IUserAccessor
{
    private readonly Func<ClaimsPrincipal> getUser;

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

    public ClaimsPrincipal User => getUser();
}