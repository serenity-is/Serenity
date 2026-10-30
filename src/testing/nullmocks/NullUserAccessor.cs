namespace Serenity.Testing;

public class NullUserAccessor : IUserAccessor
{
    public ClaimsPrincipal User => null;
}