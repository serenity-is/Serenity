namespace Serenity.Testing.Playwright;

public class Int32Input(ILocator element) : PageObject(element)
{
    public async Task<int?> GetValueAsync()
    {
        var text = await Element.InputValueAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (!int.TryParse(text, out int i))
        {
            var elementId = await Element.GetAttributeAsync("id");
            throw new ArgumentOutOfRangeException(elementId,
                $"Couldn't parse value of input #{elementId} as '{text}' is not a valid Int32!");
        }

        return i;
    }

    public async Task SetValueAsync(int? value)
    {
        if (value == null)
            await Element.ClearAsync();
        else
            await Element.FillAsync(value.ToString());
    }
}