namespace Serenity.ComponentModel;

/// <summary>
/// Where the properties declared in the base types of a type are placed, relative to the properties it declares itself.
/// </summary>
/// <seealso cref="BasePropertyOrderAttribute"/>
public enum BasePropertyPosition
{
    /// <summary>
    /// The properties of the base types come before the ones of the type.
    /// </summary>
    Before = 0,

    /// <summary>
    /// The properties of the base types come after the ones of the type.
    /// </summary>
    After = 1
}
