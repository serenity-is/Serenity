namespace Serenity.ComponentModel;

/// <summary>
/// Email editor type with two inputs. Please prefer <see cref="EmailAddressEditorAttribute"/>
/// which uses a single input.
/// </summary>
public partial class EmailEditorAttribute : CustomEditorAttribute, ICustomValidator
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Email";

    /// <summary>
    /// Creates a new instance of the email editor
    /// </summary>
    public EmailEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Domain
    /// </summary>
    public string Domain
    {
        get { return GetOption<string>("domain"); }
        set { SetOption("domain", value); }
    }

    /// <summary>
    /// True if the domain should be readonly
    /// </summary>
    public bool ReadOnlyDomain
    {
        get { return GetOption<bool>("readOnlyDomain"); }
        set { SetOption("readOnlyDomain", value); }
    }

    /// <summary>
    /// Default email validation pattern
    /// </summary>
    public static readonly Regex EmailPattern = 
        new(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+" + 
                @"@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

    /// <summary>
    /// Validates the email address
    /// </summary>
    /// <param name="context">Validation context</param>
    public string Validate(IValidationContext context)
    {
        if (context.Value == null)
            return null;

        var value = context.Value.ToString();

        if (!EmailPattern.IsMatch(value))
            return Web.WebTexts.Validation.Email.ToString(context.Localizer);

        return null;
    }
}