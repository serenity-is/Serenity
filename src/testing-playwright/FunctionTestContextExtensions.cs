namespace Serenity.Testing.Playwright;

public static class FunctionalTestContextExtensions
{
    public static async Task WaitUntilAjaxRequestsEndAsync(this IFunctionalTestContext testContext, int seconds = 30)
    {
        await testContext.Page.WaitForAjaxAsync(seconds);
    }

    public static async Task DisableAnimationsAsync(this IFunctionalTestContext testContext)
    {
        await testContext.Page.DisableAnimationsAsync();
    }

    public static async Task AssertNoWarningOrErrorInLogAsync(this IFunctionalTestContext testContext)
    {
        await testContext.Page.AssertNoWarningOrErrorInLogAsync();
    }
}