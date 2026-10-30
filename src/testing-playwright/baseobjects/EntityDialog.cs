namespace Serenity.Testing.Playwright;

public class EntityDialog : PropertyDialog
{
    public virtual async Task ClickSaveButtonAsync()
    {
        await ClickToolButtonAsync("save-and-close-button");
    }

    public virtual async Task ClickCloneButton()
    {
        await ClickToolButtonAsync("clone-button");
    }

    public virtual async Task ClickEditButton()
    {
        await ClickToolButtonAsync("edit-button");
    }

    public virtual async Task ClickDeleteButtonAsync()
    {
        await Element.Page.DisableAnimationsAsync();
        await ClickToolButtonAsync("delete-button");
    }

    public virtual async Task ClickOkButtonset()
    {
        await ClickButtonsetAsync("OK");
    }
    public virtual async Task ClickCancelButtonset()
    {
        await ClickButtonsetAsync("Cancel");
    }

    public virtual async Task ClickToolButtonAsync(string cssClass)
    {
        await Element.Locator("#" + IdPrefix + "Toolbar ." + cssClass).ClickAsync();

        await this.WaitUntilAjaxRequestsEndAsync();
    }

    public virtual async Task ClickButtonsetAsync(string text)
    {
        var buttons = Element.Locator(".ui-dialog-buttonset").Locator("button");
        await buttons.GetByText(text).ClickAsync();
        await this.WaitUntilAjaxRequestsEndAsync();
    }

    public virtual async Task SwitchToTabAsync(string tab)
    {
        await this.DisableAnimationsAsync();
        await Element.GetByText(tab).ClickAsync();
    }

    public async Task ClickSubToolButtonAsync(string tb, string className)
    {
        var button = Element.Locator("." + tb + " ." + className);
        await button.ClickAsync();
        await Page.WaitForAjaxAsync();
    }
}