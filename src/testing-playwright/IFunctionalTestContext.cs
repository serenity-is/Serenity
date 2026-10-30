namespace Serenity.Testing.Playwright;

public interface IFunctionalTestContext
{
    IPage Page { get; }
    Uri AppUri { get; }
}