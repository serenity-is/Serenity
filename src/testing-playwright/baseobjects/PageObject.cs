namespace Serenity.Testing.Playwright;

public abstract class PageObject : IFunctionalTestContext
{
    public PageObject()
    {
    }

    public PageObject(ILocator element)
    {
        Element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public ILocator Element { get; set; }
    
    public Uri AppUri { get; set; }
    public IPage Page => Element?.Page;
}