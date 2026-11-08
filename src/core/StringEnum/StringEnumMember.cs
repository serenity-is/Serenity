namespace Serenity;

/// <summary>
/// A member of a <see cref="StringEnum"/>, with its name, value and optional text information.
/// </summary>
public sealed class StringEnumMember
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumMember"/> class.
    /// </summary>
    /// <param name="name">The name of the member, which is the name of its field.</param>
    /// <param name="value">The string value of the member.</param>
    /// <param name="textKey">The local text key of the member, if it has a text attribute with a key.</param>
    /// <param name="text">The invariant text of the member, if it has a text attribute.</param>
    /// <param name="isVisible">Whether the member is sent to the client, i.e. it is not hidden with
    /// <see cref="IgnoreUIFieldAttribute"/> or <see cref="TransformIgnoreAttribute"/>.</param>
    public StringEnumMember(string name, string value, string? textKey, string? text, bool isVisible)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        TextKey = textKey;
        Text = text;
        IsVisible = isVisible;
    }

    /// <summary>
    /// Gets the name of the member, which is the name of its field.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the string value of the member.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the local text key of the member, if it has a text attribute with a key.
    /// </summary>
    public string? TextKey { get; }

    /// <summary>
    /// Gets the invariant text of the member, if it has a text attribute.
    /// </summary>
    public string? Text { get; }

    /// <summary>
    /// Gets whether the member is sent to the client, i.e. it is not hidden with
    /// <see cref="IgnoreUIFieldAttribute"/> or <see cref="TransformIgnoreAttribute"/>. The server still accepts
    /// and displays hidden members, as they are values like a system assigned status that a UI never edits.
    /// </summary>
    public bool IsVisible { get; }
}
