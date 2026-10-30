namespace Serenity.Testing.Playwright;

public class TextInput(ILocator element) : PageObject(element)
{
    public async Task<string> GetValueAsync()
    {
        return await Element.InputValueAsync();
    }

    public async Task SetValueAsync(string value)
    {
        await Element.FillAsync(value);
    }
}