using System.Runtime.CompilerServices;

namespace Serenity.Testing.Playwright;

public abstract class PrefixedPageObject : PageObject
{
    public const string UndeterminedIdPrefix = "__UNDETERMINED__IDPREFIX__";

    public string IdPrefix { get; set; } = UndeterminedIdPrefix;

    public CheckInput GetCheckInput([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new CheckInput(Element.Locator("#" + IdPrefix + property));
    }

    public DecimalInput GetDecimalInput([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new DecimalInput(Element.Locator("#" + IdPrefix + property));
    }

    public DoubleInput GetDoubleInput([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new DoubleInput(Element.Locator("#" + IdPrefix + property));
    }

    public Int16Input GetInt16Input([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new Int16Input(Element.Locator("#" + IdPrefix + property));
    }

    public Int32Input GetInt32Input([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new Int32Input(Element.Locator("#" + IdPrefix + property));
    }

    public TextInput GetTextInput([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new TextInput(Element.Locator("#" + IdPrefix + property));
    }

    public Select2Input GetSelect2Input([CallerMemberName] string property = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(property);
        return new Select2Input(Element.Locator("#" + IdPrefix + property));
    }

    public async Task ClickPanelCloseAsync()
    {
        await Element.Locator(".panel-titlebar-close").ClickAsync();
    }

    public async Task<string> GetValidationErrorAsync(string property)
    {
        if (property == null)
            throw new ArgumentNullException(property);

        var errors = Page.Locator(($"label.error[for='{IdPrefix + property}']"));
        if (await errors.CountAsync() == 0)
            return null;

        return (await errors.CountAsync()) == 1 ? await errors.First.TextContentAsync()
            : string.Join(Environment.NewLine, 
                await Task.WhenAll((await errors.AllAsync()).Select(async e => await e.TextContentAsync()).ToArray()));
    }

    public ILocator GetElementByFormField(string property)
    {
        if (property == null)
            throw new ArgumentNullException(property);

        return Page.Locator("#" + IdPrefix + property);
    }

    public async Task ClearFocusAsync()
    {
        await Page.Locator("body").ClickAsync();
    }
}