namespace Serenity.Testing.Playwright;

public class Int16Input(ILocator element) : PageObject(element)
{
    public async Task<short?> GetValueAsync()
    {
        var text = await Element.InputValueAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (!short.TryParse(text, out short s))
        {
            var elementId = await Element.GetAttributeAsync("id");
            throw new ArgumentOutOfRangeException(elementId,
                $"Couldn't parse value of input #{elementId} as '{text}' is not a valid Int16!");
        }

        return s;
    }

    public async Task SetValueAsync(short? value)
    {
        if (value == null)
            await Element.ClearAsync();
        else
            await Element.FillAsync(value.ToString());
    }
}