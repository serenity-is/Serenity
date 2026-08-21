namespace Serenity.ComponentModel;

/// <summary>
/// Placed on permission keys to define implicitly assigned
/// permissions when a user / role has the permission.
/// Currently only implemented in the premium app.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="ImplicitPermissionAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ImplicitPermissionAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; private set; } = value;
}