namespace Serenity.Testing.Playwright;

public class DecimalInput(ILocator element) : PageObject(element)
{
    public async Task<decimal?> GetValueAsync()
    {
        var text = await Element.InputValueAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (!decimal.TryParse(text, out decimal m))
        {
            var elementId = await Element.GetAttributeAsync("id");
            throw new ArgumentOutOfRangeException(elementId,
                $"Couldn't parse value of input #{elementId} as '{text}' is not a valid decimal!");
        }

        return m;
    }

    public async Task SetValueAsync(decimal? value)
    {
        if (value == null)
            await Element.ClearAsync();
        else
            await Element.FillAsync(value.ToString());
    }
}