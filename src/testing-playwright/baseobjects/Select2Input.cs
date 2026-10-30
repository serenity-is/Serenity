namespace Serenity.Testing.Playwright;

public class Select2Input(ILocator element) : PageObject(element)
{
    public async Task<string> GetTextValueAsync()
    {
        var id = await Element.GetAttributeAsync("id");
        var choice = Page.Locator("#s2id_" + id + " .select2-choice.select2-default");
        return await choice.TextContentAsync();
    }

    public async Task SetValueByTextAsync(string text)
    {
        var id = await Element.GetAttributeAsync("id");
        var select2 = Element.Page.Locator("#s2id_" + id);

        if (string.IsNullOrEmpty(text))
        {
            var close = select2.Locator("." + "select2-search-choice-close");
            await close.ClickAsync();
        }

        await select2.Locator("." + "select2-choice").ClickAsync();

        var dropdown = Page.Locator(".select2-drop-active:not(.select2-display-none):not([style*='display: none'])");
        await dropdown.Locator(".select2-input").FillAsync(text);

        await Element.EvaluateAsync("el => typeof jQuery !== 'undefined' ? jQuery(el).triggerHandler('execute-search') : el.dispatchEvent(new Event('execute-search'))");

        await Page.WaitForAjaxAsync();

        var labels = dropdown.Locator(".select2-result-selectable").Filter(new() { HasTextString = text.Trim() });
        var label = labels.First;

        await label.ClickAsync();
    }

    public async Task<string[]> GetTextValuesAsync()
    {
        var id = await Element.GetAttributeAsync("id");
        var choices = await Page.Locator("#s2id_" + id + " .select2-search-choice").AllTextContentsAsync();
        return [.. choices];
    }

    public async Task SetValuesByTextAsync(string[] texts)
    {
        var id = await Element.GetAttributeAsync("id");
        var select2 = Element.Page.Locator("#s2id_" + id);

        while (true)
        {
            var close = await select2.Locator(".select2-search-choice-close").AllAsync();
            if (close.Count > 0)
                await close[0].ClickAsync();
            else
                break;
        }

        if (texts == null || texts.Length == 0)
            return;

        foreach (var text in texts)
        {
            await select2.Locator(".select2-choices").ClickAsync();

            var dropdown = Page.Locator(".select2-drop-active:not(.select2-display-none):not([style*='display: none'])");
            await select2.Locator(".select2-input").FillAsync(text);

            await Element.EvaluateAsync("el => typeof jQuery !== 'undefined' ? jQuery(el).triggerHandler('execute-search') : el.dispatchEvent(new Event('execute-search'))");

            await Page.WaitForAjaxAsync();

            var labels = dropdown.Locator(".select2-result-selectable").Filter(new() { HasTextString = text.Trim() });
            var label = labels.First;

            await label.ClickAsync();
        }
    }
}