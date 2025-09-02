namespace Serenity.ComponentModel;

/// <summary>
/// Determines if field is required in form.
/// </summary>
/// <seealso cref="Attribute" />
public sealed class RequiredAttribute : Attribute
{

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredAttribute"/> class.
    /// </summary>
    public RequiredAttribute()
    {
        IsRequired = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredAttribute"/> class.
    /// </summary>
    /// <param name="isRequired">if set to <c>true</c> [is required].</param>
    public RequiredAttribute(bool isRequired)
    {
        IsRequired = isRequired;
    }

    /// <summary>
    /// Gets the target property required value.
    /// </summary>
    /// <value>
    ///   <c>true</c> if target property is required; otherwise, <c>false</c>.
    /// </value>
    public bool IsRequired { get; private set; }
}
