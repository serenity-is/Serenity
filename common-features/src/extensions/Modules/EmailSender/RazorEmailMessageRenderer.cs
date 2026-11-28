using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Renders an email that code builds with the <c>EmailMessage</c> view, in the layout of the emails
/// (<c>~/Areas/Serenity.Extensions/Shared/_EmailLayout.cshtml</c>), which an application can replace with views of the same
/// paths in its own project.
/// </summary>
/// <param name="serviceProvider">The services, for rendering outside a request.</param>
/// <param name="httpContextAccessor">Gives the services of the request, when there is one.</param>
public class RazorEmailMessageRenderer(IServiceProvider serviceProvider, IHttpContextAccessor? httpContextAccessor = null)
    : IEmailMessageRenderer
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc/>
    public virtual string Render(EmailMessageModel message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return TemplateHelper.RenderViewToString(httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider,
            MVC.Views.Shared.EmailMessage, message);
    }
}
