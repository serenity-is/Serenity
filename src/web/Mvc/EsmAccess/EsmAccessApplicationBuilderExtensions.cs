using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serenity.Web;
using Serenity.Web.Middleware;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains the extension to add the ESM access control to the request pipeline
/// </summary>
public static class EsmAccessApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the ESM access control, which serves the files under <c>esm</c> folders only to users who may
    /// load them, if it is enabled with <see cref="EsmAccessControlOptions.Enabled"/> (the <c>EsmAccessControl</c> section
    /// of appsettings.json). Call it before <c>UseStaticFiles</c>, as it must run before the files are served.
    /// See <see cref="EsmAccessControlOptions"/> for the rules.
    /// </summary>
    /// <param name="app">The application builder</param>
    public static IApplicationBuilder UseEsmAccessControl(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.ApplicationServices.GetRequiredService<IOptions<EsmAccessControlOptions>>().Value;
        if (!options.Enabled)
            return app;

        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var policy = new EsmAccessPolicy(environment.WebRootFileProvider, options,
            app.ApplicationServices.GetService<ILayoutScriptFileProvider>(),
            app.ApplicationServices.GetService<ILoggerFactory>()?.CreateLogger<EsmAccessPolicy>());

        return app.UseMiddleware<EsmAccessMiddleware>(policy);
    }
}
