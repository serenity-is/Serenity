using System.Security.Principal;

namespace Serenity.Tests;

public class MockUserAccessor : IUserAccessor
{
    private readonly Func<ClaimsPrincipal> getUser;

    public MockUserAccessor(Func<ClaimsPrincipal> getUser)
    {
        this.getUser = getUser ?? throw new ArgumentNullException(nameof(getUser));
    }

    public MockUserAccessor(Func<string> getUsername)
    {
        if (getUsername == null)
            throw new ArgumentNullException(nameof(getUsername));

        getUser = () =>
        {
            var username = getUsername();
            if (username == null)
                return null;

            return new ClaimsPrincipal(new GenericIdentity(username, "Test"));
        };
    }

    public ClaimsPrincipal User => getUser();
}