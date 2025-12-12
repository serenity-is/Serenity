using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Web;

namespace Serenity.Web;

/// <summary>
/// Contains CSP related extension methods for HTML helpers and HTTP contexts.
/// </summary>
public static partial class HtmlCspExtensions
{
    const string NonceKey = "HtmlCspExtensions:Nonce";

    private static string CspNonce(IDictionary<object, object> contextItems)
    {
        ArgumentNullException.ThrowIfNull(contextItems);

        if (contextItems[NonceKey] is string nonce)
            return nonce;

        using var rng = RandomNumberGenerator.Create();
        var nonceBytes = new byte[32];
        rng.GetBytes(nonceBytes);
        contextItems[NonceKey] = nonce = WebEncoders.Base64UrlEncode(nonceBytes);
        return nonce;
    }

    /// <summary>
    /// Gets a nonce value for use in script and style elements.
    /// Automatically generates and stores it in the current HTTP context items and
    /// adds it to the CSP directives.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="addDirectives">True to add CSP directives for script-src, style-src, font-src</param>
    public static string CspNonce(this IHtmlHelper html, bool addDirectives = true)
    {
        ArgumentNullException.ThrowIfNull(html);
        var cspNonce = CspNonce(html.ViewContext?.HttpContext?.Items);
        if (addDirectives)
        {
            var directiveValue = $"'nonce-{cspNonce}'";
            AddCspDirective(html, "script-src", directiveValue);
            AddCspDirective(html, "style-src", directiveValue);
        }
        return cspNonce;
    }

    private static string CspAutoQuote(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        // Auto-quote if it looks like a keyword (alphanumeric, no existing quotes)
        if (!string.IsNullOrEmpty(value) &&
            !value.StartsWith('\'') && !value.StartsWith('\"') &&
            value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            return $"'{value}'";
        }
        return value;
    }

    const string CspDirectivesKey = "HtmlScriptExtensions:CspDirectives";

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this IHtmlHelper html, string directiveName, params string[] values)
    {
        AddCspDirective(html.ViewContext?.HttpContext?.Items, directiveName, values);
    }

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this HttpContext context, string directiveName, params string[] values)
    {
        AddCspDirective(context.Items, directiveName, values);
    }

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this ControllerBase controller, string directiveName, params string[] values)
    {
        AddCspDirective(controller.HttpContext.Items, directiveName, values);
    }

    private static readonly HashSet<string> allowedCspDirectiveNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "default-src",
        "script-src",
        "style-src",
        "img-src",
        "connect-src",
        "font-src",
        "object-src",
        "media-src",
        "frame-src",
        "worker-src",
        "base-uri",
        "form-action",
        "frame-ancestors",
        "manifest-src",
        "prefetch-src",
        "script-src-attr",
        "style-src-attr"
    };

    private static void AddCspDirective(IDictionary<object, object> contextItems, string directiveName, params string[] values)
    {
        ArgumentNullException.ThrowIfNull(contextItems);
        ArgumentNullException.ThrowIfNull(directiveName);
        ArgumentNullException.ThrowIfNull(values);
        if (!allowedCspDirectiveNames.Contains(directiveName))
            throw new ArgumentException($"Directive name '{directiveName}' is not allowed.", nameof(directiveName));

        if (contextItems[CspDirectivesKey] is not Dictionary<string, HashSet<string>> directives)
            contextItems[CspDirectivesKey] = directives = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        if (!directives.TryGetValue(directiveName, out var directiveValues))
            directives[directiveName] = directiveValues = new HashSet<string>(StringComparer.Ordinal);

        foreach (var value in values.Where(x => !string.IsNullOrEmpty(x)))
            directiveValues.Add(CspAutoQuote(value));
    }

    /// <summary>
    /// Adds a Content Security Policy script-src directive for the given URL
    /// and returns the URL. This can be used in script include helpers for external URLs.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="url">Url</param>
    /// <returns></returns>
    public static string AddCspScriptUrl(this IHtmlHelper html, string url)
    {
        AddCspDirective(html, "script-src", url);
        return url;
    }

    /// <summary>
    /// Gets a Content Security Policy directive added via AddCspDirective
    /// merged with any manual values provided. The string includes the final semicolon.
    /// This can be used to render parts of the CSP header or meta tag content.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="manualValues">Manual values to include in the directive.
    /// Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static HtmlString GetCspDirective(this IHtmlHelper html, string directiveName, params string[] manualValues)
    {
        ArgumentNullException.ThrowIfNull(html);
        var directive = GetCspDirective(html.ViewContext?.HttpContext?.Items, directiveName, manualValues);
        return new HtmlString(HttpUtility.HtmlAttributeEncode(directive)
            .Replace("&#39;", "'"));
    }

    private static string GetCspDirective(IDictionary<object, object> contextItems, string directiveName, params string[] manualValues)
    {
        ArgumentNullException.ThrowIfNull(contextItems);
        ArgumentException.ThrowIfNullOrEmpty(directiveName);
        if (!allowedCspDirectiveNames.Contains(directiveName))
            throw new ArgumentException($"Directive name '{directiveName}' is not allowed.", nameof(directiveName));

        var finalSet = new HashSet<string>(StringComparer.Ordinal);
        if (manualValues != null)
            finalSet.UnionWith(manualValues.Where(x => !string.IsNullOrEmpty(x)).Select(CspAutoQuote));

        if ((contextItems[CspDirectivesKey] is Dictionary<string, HashSet<string>> directives) &&
            directives.TryGetValue(directiveName, out var directiveValues))
            finalSet.UnionWith(directiveValues);

        // If 'unsafe-inline' is present, remove any nonce or hash values to avoid CSP ignoring 'unsafe-inline'
        if (finalSet.Contains("'unsafe-inline'"))
            finalSet.RemoveWhere(x => x.StartsWith("'nonce-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha256-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha384-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha512-", StringComparison.Ordinal));

        return directiveName + " " + string.Join(' ', finalSet) + ";";
    }
}