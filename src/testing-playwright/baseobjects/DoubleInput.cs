namespace Serenity.Testing.Playwright;

public class DoubleInput(ILocator element) : PageObject(element)
{
    public async Task<double?> GetValueAsync()
    {
        var text = await Element.InputValueAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (!double.TryParse(text, out double m))
        {
            var elementId = await Element.GetAttributeAsync("id");
            throw new ArgumentOutOfRangeException(elementId,
                $"Couldn't parse value of input #{elementId} as '{text}' is not a valid double!");
        }

        return m;
    }

    public async Task SetValueAsync(double? value)
    {
        if (value == null)
            await Element.ClearAsync();
        else
            await Element.FillAsync(value.ToString());
    }
}