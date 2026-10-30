namespace Serenity.Testing;

public class MockUrlHelperFactory(Func<string, bool> isLocalUri) : IUrlHelperFactory
{
    public IUrlHelper GetUrlHelper(ActionContext context)
    {
        return new MockUrlHelper(isLocalUri);
    }
}