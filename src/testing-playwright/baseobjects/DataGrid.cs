namespace Serenity.Testing.Playwright;

public abstract class DataGrid : PageObject
{
    public DataGrid()
    {
    }

    public virtual ILocator GetQuickSearchBar()
    {
        return GetToolbar().Locator(".s-QuickSearchBar");
    }

    public virtual async Task QuickSearchAsync(string text)
    {
        var input = GetQuickSearchBar().Locator(".s-QuickSearchInput");
        await input.QuickSearchAsync(text);
    }

    public virtual async Task SearchAndClickAsync(string text, bool exact = false)
    {
        await QuickSearchAsync(text);
        await Element.GetByText(text, new() { Exact = exact }).ClickAsync();
        await Page.WaitForAjaxAsync();
    }

    public virtual async Task SearchAndCheckAsync(string text)
    {
        await QuickSearchAsync(text);

        var inputs = Element.Locator(".check-box");
        foreach (var i in await inputs.AllAsync())
        {
            await i.ClickAsync();
        }

        await Page.WaitForAjaxAsync();
    }
    
    public async Task ClickToolButtonAsync(string className)
    {
        var button = GetToolButtonByClass(className);
        await button.ClickAsync();
        await Page.WaitForAjaxAsync();
    }

    public ILocator GetToolbar()
    {
        return Element.Locator(".grid-toolbar");
    }

    public ILocator GetToolButtonByClass(string className)
    {
        return GetToolbar().Locator("." + className);
    }

    public ILocator GetToolButtonByAction(string action)
    {
        return GetToolbar().Locator(".tool-button[data-action=" + action + "]");
    }

    public ILocator GetAddButton()
    {
        return GetToolButtonByAction("add");
    }

    public virtual async Task ClickAddButtonAsync()
    {
        await GetAddButton().ClickAsync();
        await this.WaitUntilAjaxRequestsEndAsync();
    }

    public virtual EntityInfo EntityInfo => new(GetType());
    public EntityInfo BaseEntityInfo => new(GetType().BaseType);
}