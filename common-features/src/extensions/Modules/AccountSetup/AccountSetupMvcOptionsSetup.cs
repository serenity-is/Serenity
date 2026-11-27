using Microsoft.AspNetCore.Mvc;

namespace Serenity.Extensions;

/// <summary>
/// Adds <see cref="AccountSetupFilter"/> to the global filters of MVC, once.
/// </summary>
public class AccountSetupMvcOptionsSetup : IConfigureOptions<MvcOptions>
{
    /// <inheritdoc/>
    public void Configure(MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Filters.Add<AccountSetupFilter>();
    }
}
