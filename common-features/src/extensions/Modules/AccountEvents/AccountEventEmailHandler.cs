using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Emails a user when the account changes, e.g. the password, so the user learns of a change that the user did not do. The
/// email of a changed email address goes to the old address. See <see cref="AccountEventEmailOptions"/> to turn it off, or
/// leave some kinds out.
/// </summary>
/// <param name="options">The options.</param>
/// <param name="localizer">The localizer of the texts.</param>
/// <param name="serviceProvider">The services, to render the email when there is no request.</param>
/// <param name="emailSender">Sends the emails, without which nothing is sent.</param>
/// <param name="httpContextAccessor">Gives the request, whose services render the email.</param>
public class AccountEventEmailHandler(IOptions<AccountEventEmailOptions> options,
    ITextLocalizer localizer,
    IServiceProvider serviceProvider,
    IEmailSender? emailSender = null,
    IHttpContextAccessor? httpContextAccessor = null) : IAccountEventHandler
{
    /// <inheritdoc/>
    public virtual Task HandleAsync(AccountEvent accountEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(accountEvent);
        if (emailSender == null || options?.Value?.IsEnabledFor(accountEvent.Kind) == false)
            return Task.CompletedTask;

        var mailTo = GetRecipient(accountEvent);
        if (string.IsNullOrWhiteSpace(mailTo))
            return Task.CompletedTask;

        var model = CreateModel(accountEvent);
        var body = RenderBody(model);
        emailSender.Send(subject: model.Subject, body: body, mailTo: mailTo);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets who the email goes to: the old address of a changed email address, else the address of the user.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual string? GetRecipient(AccountEvent accountEvent)
    {
        if (accountEvent.Kind == AccountEventKind.EmailChanged && !string.IsNullOrWhiteSpace(accountEvent.OldValue))
            return accountEvent.OldValue;

        return accountEvent.Email;
    }

    /// <summary>
    /// Creates the model of the email, with its subject and the sentence of what changed.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual AccountEventEmailModel CreateModel(AccountEvent accountEvent)
    {
        var (subject, message) = accountEvent.Kind switch
        {
            AccountEventKind.PasswordChanged => (AccountEventEmailTexts.PasswordChangedSubject, AccountEventEmailTexts.PasswordChanged),
            AccountEventKind.EmailChanged => (AccountEventEmailTexts.EmailChangedSubject, AccountEventEmailTexts.EmailChanged),
            AccountEventKind.UsernameChanged => (AccountEventEmailTexts.UsernameChangedSubject, AccountEventEmailTexts.UsernameChanged),
            AccountEventKind.TwoFactorEnabled => (AccountEventEmailTexts.TwoFactorEnabledSubject, AccountEventEmailTexts.TwoFactorEnabled),
            AccountEventKind.TwoFactorDisabled => (AccountEventEmailTexts.TwoFactorDisabledSubject, AccountEventEmailTexts.TwoFactorDisabled),
            AccountEventKind.TwoFactorReset => (AccountEventEmailTexts.TwoFactorResetSubject, AccountEventEmailTexts.TwoFactorReset),
            _ => throw new ArgumentOutOfRangeException(nameof(accountEvent), accountEvent.Kind, null)
        };

        var siteTitle = localizer.TryGet("Navigation.SiteTitle") ?? "";
        var subjectText = subject.ToString(localizer);
        return new AccountEventEmailModel
        {
            Event = accountEvent,
            SiteTitle = siteTitle,
            Subject = string.IsNullOrEmpty(siteTitle) ? subjectText : siteTitle + ": " + subjectText,
            // the two-factor events have the method instead of the old and the new values
            Message = accountEvent.Kind is AccountEventKind.TwoFactorEnabled or AccountEventKind.TwoFactorDisabled ?
                string.Format(CultureInfo.CurrentCulture, message.ToString(localizer), accountEvent.Method) :
                string.Format(CultureInfo.CurrentCulture, message.ToString(localizer), accountEvent.OldValue, accountEvent.NewValue)
        };
    }

    /// <summary>
    /// Renders the body of the email, from the AccountEventEmail view.
    /// </summary>
    /// <param name="model">The model.</param>
    protected virtual string RenderBody(AccountEventEmailModel model)
    {
        return TemplateHelper.RenderViewToString(httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider,
            MVC.Views.Membership.AccountEvents.AccountEventEmail, model);
    }
}
