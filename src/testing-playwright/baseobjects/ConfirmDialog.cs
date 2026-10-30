namespace Serenity.Testing.Playwright;

public class ConfirmDialog : PageObject
{
    public virtual async Task ClickYesAsync()
    {
        await ClickDialogButtonAsync(0);
    }

    public virtual async Task ClickNoAsync()
    {
        await ClickDialogButtonAsync(1);
    }

    public virtual async Task ClickDialogButtonAsync(int index)
    {
        var buttonSet = Element.Locator((".modal-footer, .dialog-footer, .panel-footer"));
        await buttonSet.Locator("button").Nth(index).ClickAsync();
        await this.WaitUntilAjaxRequestsEndAsync();
    }
}