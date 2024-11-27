namespace Serenity.Tests;

public class NullUserAccessor : IUserAccessor
{
    public ClaimsPrincipal User => null;
}