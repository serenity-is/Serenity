namespace Serenity.ComponentModel;

/// <summary>
/// Email editor type with two inputs. Prefer <see cref="EmailAddressEditorAttribute"/>
/// which uses a single input.
/// </summary>
public partial class EmailEditorAttribute : CustomEditorAttribute, ICustomValidator
{
    /// <summary>
    /// The editor type key.
    /// </summary>
    public const string Key = "Email";

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailEditorAttribute"/> class.
    /// </summary>
    public EmailEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    public string Domain
    {
        get { return GetOption<string>("domain"); }
        set { SetOption("domain", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the domain should be read-only.
    /// </summary>
    public bool ReadOnlyDomain
    {
        get { return GetOption<bool>("readOnlyDomain"); }
        set { SetOption("readOnlyDomain", value); }
    }

    /// <summary>
    /// The default email validation pattern.
    /// </summary>
    public static readonly Regex EmailPattern =
        EmailPatternRegexGen();

    /// <summary>
    /// Validates the email address.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <returns>The validation error text, or <c>null</c> if the value is valid.</returns>
    public string Validate(IValidationContext context)
    {
        if (context.Value == null)
            return null;

        var value = context.Value.ToString();

        if (!EmailPattern.IsMatch(value))
            return Web.FormValidationTexts.Email.ToString(context.Localizer);

        return null;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
    private static partial Regex EmailPatternRegexGen();
}