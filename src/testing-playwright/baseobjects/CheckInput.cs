namespace Serenity.Testing.Playwright;

public class CheckInput(ILocator element) : PageObject(element)
{
    public async Task<bool?> GetCheckedAsync()
    {
        return await Element.IsCheckedAsync();
    }

    public async Task SetCheckedAsync(bool value)
    {
        await Element.SetCheckedAsync(value);
    }
}