namespace Serenity.TestUtils;

public class NullUserAccessor : IUserAccessor
{
    public ClaimsPrincipal User => null;
}