namespace Serenity.Testing.Playwright;

public static class ILocatorHelper
{
    public static async Task<bool> ContainsClassAsync(this ILocator element, string value)
    {
        return (await element.GetAttributeAsync("class")).Contains(value);
    }
}