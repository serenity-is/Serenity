namespace Serenity.Testing.Playwright;

public static class AuthExtensions
{
    public static async Task LoginAsAsync(this IFunctionalTestContext context, string username)
    {
        await context.Page.GotoAsync(
            new Uri(context.AppUri, "test-session-login/" + Uri.EscapeDataString(username)).ToString());
    }

    public static async Task LoginAsSysAdminAsync(this IFunctionalTestContext context)
    {
        await LoginAsAsync(context, TestUser.SysAdmin);
    }
}