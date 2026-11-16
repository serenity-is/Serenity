using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a text to match a regular expression as a whole, which the forms also check. The expression is matched with
/// ECMAScript semantics on the server, as the browsers match it, so it should only use what both have, e.g. no named
/// groups or lookbehinds. An empty value is valid, as being required is <see cref="RequiredAttribute"/>'s concern.
/// </summary>
/// <param name="pattern">The regular expression, without anchors, as the whole text has to match it.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidatePatternAttribute(string pattern) : RegularExpressionAttribute(pattern), IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.PatternValidationRule</c>.
    /// </summary>
    public const string Key = "Pattern";

    private static readonly ConcurrentDictionary<(string, int), Regex> regexes = new();

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <inheritdoc/>
    public string Rule => Key;

    /// <inheritdoc/>
    public object? RuleParams => Pattern;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(text))
            return true;

        var timeout = MatchTimeoutInMilliseconds;
        var regex = regexes.GetOrAdd((Pattern, timeout), key => new Regex(key.Item1, RegexOptions.ECMAScript,
            key.Item2 > 0 ? TimeSpan.FromMilliseconds(key.Item2) : Regex.InfiniteMatchTimeout));

        // the whole text, as the base attribute and the forms check it
        var match = regex.Match(text);
        return match.Success && match.Index == 0 && match.Length == text.Length;
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value) ? ValidationResult.Success :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.Pattern");
    }
}
