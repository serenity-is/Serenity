using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Email editor type with two inputs. Prefer <see cref="EmailAddressEditorAttribute"/>
/// which uses a single input.
/// </summary>
public partial class EmailEditorAttribute : CustomEditorAttribute, IImpliesValidation
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
    public string? Domain
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
    /// The default email validation pattern, which is the one of <see cref="ValidateEmailAttribute"/>.
    /// </summary>
    public static readonly Regex EmailPattern = ValidateEmailAttribute.Pattern;

    /// <summary>
    /// Gets the rule an email editor implies, so the value is checked as an email address on the server, and by the
    /// forms, see <see cref="ValidateEmailAttribute"/>.
    /// </summary>
    public IEnumerable<ValidationAttribute> GetImpliedValidation()
    {
        yield return new ValidateEmailAttribute();
    }
}