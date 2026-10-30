namespace Serenity.Testing.Playwright;

public static class WebDriverExtensions
{
    public static async Task WaitForAjaxAsync(this IPage browser, int seconds = 30)
    {
        await browser.WaitForFunctionAsync("() => (typeof jQuery !== 'undefined' ? jQuery.active == 0 : (typeof Serenity !== 'undefined' && Serenity.getActiveRequests ? Serenity.getActiveRequests() == 0 : true))", TimeSpan.FromSeconds(seconds));
    }

    public static async Task QuickSearchAsync(this ILocator input, string text)
    {
        await input.FillAsync(text);
        await input.EvaluateAsync("el => typeof jQuery !== 'undefined' ? jQuery(el).triggerHandler('execute-search') : el.dispatchEvent(new Event('execute-search'))");
        await input.Page.WaitForAjaxAsync();
    }

    public static async Task<string> GetBeforeCssValueAsync(this ILocator element, string key)
    {
        return await element.EvaluateAsync<string>("(el, key) => window.getComputedStyle(el, '::before').getPropertyValue(key)", key);
    }

    public static async Task AssertNoWarningOrErrorInLogAsync(this IPage browser)
    {
        var entries = (await browser.ConsoleMessagesAsync()).Where(x => x.Type == "warning" || x.Type == "error").ToList();
        var error = entries.FirstOrDefault();
        Assert.Null(error?.Text);
    }

    public static async Task DisableAnimationsAsync(this IPage browser)
    {
        await browser.EvaluateAsync(/*lang=javascript*/ """
            () => {
                        if (document.querySelector('style#disable-animations') != null)
                            return;
                        const style = document.createElement('style');
                        style.type = 'text/css';
                        const nonce = document.querySelector('meta[name=csp-nonce]')?.getAttribute('content');
                        if (nonce)
                            style.setAttribute('nonce', nonce);
                        style.textContent = '*, ::before, ::after { transition-duration: 0s !important; animation-duration: 0s !important;'
                        document.head.appendChild(style);
                    }
            """);
    }
}