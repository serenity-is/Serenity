namespace Serenity.Testing.Playwright;

public static class PageObjectExtensions
{
    public static async ValueTask<TPageObject> FindPageObjectAsync<TPageObject>(this IFunctionalTestContext testContext, bool topmost = false)
        where TPageObject : PageObject, new()
    {
        var findByClass = typeof(TPageObject).GetCustomAttribute<FindByClassAttribute>();

        string className = findByClass != null ? findByClass.Value : ("s-" + typeof(TPageObject).Name);

        var element = testContext.Page.Locator("." + className);
        if (topmost)
            element = element.Last;

        var pageObject = new TPageObject
        {
            AppUri = testContext.AppUri,
            Element = element
        };

        if (pageObject is PrefixedPageObject prefixedPageObject && 
            (prefixedPageObject.IdPrefix is null ||
             prefixedPageObject.IdPrefix == PrefixedPageObject.UndeterminedIdPrefix))
            prefixedPageObject.IdPrefix = (await DetermineIdAsync(element)) + "_";

        return pageObject;
    }

    private static async Task<string> DetermineIdAsync(ILocator element)
    {
        string id = await element.GetAttributeAsync("id");
        if (string.IsNullOrEmpty(id))
            id = await element.Locator(".modal-body[id], .panel-body[id], .ui-dialog-content[id]").GetAttributeAsync("id");

        Assert.False(string.IsNullOrEmpty(id), "Couldn't determine ID prefix for " +
            $"{element} dialog type context as the dialog element doesn't have an ID attribute!");

        return id;
    }
}