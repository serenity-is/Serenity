using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

/// <summary>
/// Publishes the sign-in events of the accounts, see <see cref="IAccountEventPublisher"/>, when the application registers
/// one. A username that no user has is not stored, as it may be a password typed in the wrong box.
/// </summary>
public static class SignInEventExtensions
{
    /// <summary>The method of the sign-ins with the password on the sign-in page.</summary>
    public const string PasswordMethod = "Password";

    /// <summary>The method of the sign-ins with the password grant of the token endpoint.</summary>
    public const string TokenMethod = "Token";

    /// <summary>
    /// Publishes that a user signed in.
    /// </summary>
    /// <param name="services">The services, with the publisher.</param>
    /// <param name="username">The username.</param>
    /// <param name="method">How, e.g. <see cref="PasswordMethod"/>, or the name of an external provider.</param>
    public static void PublishSignIn(this IServiceProvider services, string username, string method)
    {
        Publish(services, AccountEventKind.SignedIn, username, method, reason: null);
    }

    /// <summary>
    /// Publishes that a sign-in failed. A username that no user has is not stored.
    /// </summary>
    /// <param name="services">The services, with the publisher.</param>
    /// <param name="username">The username that was entered.</param>
    /// <param name="method">How, e.g. <see cref="PasswordMethod"/>.</param>
    /// <param name="reason">Why, see <see cref="SignInFailureReasons"/>; <see cref="SignInFailureReasons.UnknownUser"/>
    /// when no user has the username.</param>
    public static void PublishSignInFailed(this IServiceProvider services, string? username, string method, string reason)
    {
        Publish(services, AccountEventKind.SignInFailed, username, method, reason);
    }

    /// <summary>
    /// Publishes that a sign-in with the password failed, for a password validation result that is not valid. Empty
    /// usernames are not published.
    /// </summary>
    /// <param name="services">The services, with the publisher.</param>
    /// <param name="username">The username that was entered.</param>
    /// <param name="method">How, e.g. <see cref="PasswordMethod"/>.</param>
    /// <param name="result">The result.</param>
    public static void PublishSignInFailed(this IServiceProvider services, string? username, string method,
        PasswordValidationResult result)
    {
        if (result is PasswordValidationResult.Valid or PasswordValidationResult.EmptyUsername)
            return;

        Publish(services, AccountEventKind.SignInFailed, username, method, SignInFailureReasons.From(result));
    }

    /// <summary>
    /// Publishes that a user signed out.
    /// </summary>
    /// <param name="services">The services, with the publisher.</param>
    /// <param name="user">The user who signs out.</param>
    public static void PublishSignOut(this IServiceProvider services, ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated == true && user.Identity.Name is { Length: > 0 } username)
            Publish(services, AccountEventKind.SignedOut, username, user.Identity.AuthenticationType, reason: null);
    }

    /// <summary>
    /// Publishes an event of a user that is found by the username, e.g. one of the other kinds, like
    /// <see cref="AccountEventKind.Elevated"/>. A username that no user has is not stored.
    /// </summary>
    /// <param name="services">The services, with the publisher.</param>
    /// <param name="kind">What happened.</param>
    /// <param name="username">The username.</param>
    /// <param name="method">How, if any.</param>
    /// <param name="reason">Why it failed, if it did.</param>
    public static void Publish(this IServiceProvider services, AccountEventKind kind, string? username, string? method,
        string? reason)
    {
        ArgumentNullException.ThrowIfNull(services);
        var publisher = services.GetService<IAccountEventPublisher>();
        if (publisher == null)
            return;

        var user = string.IsNullOrWhiteSpace(username) ? null :
            services.GetService<IUserRetrieveService>()?.ByUsername(username);
        if (user == null && kind != AccountEventKind.SignInFailed)
            return;

        publisher.Publish(new AccountEvent
        {
            Kind = kind,
            UserId = user?.Id,
            Username = user?.Username,
            DisplayName = user?.DisplayName,
            Email = user?.Email,
            Method = method,
            Reason = user == null ? SignInFailureReasons.UnknownUser : reason
        });
    }
}
