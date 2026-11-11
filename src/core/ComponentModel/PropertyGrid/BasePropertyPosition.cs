namespace Serenity.ComponentModel;

/// <summary>
/// Where properties declared in a type are placed, relative to the properties of another type.
/// </summary>
/// <seealso cref="BasePropertyOrderAttribute"/>
public enum BasePropertyPosition
{
    /// <summary>
    /// No preference, which is before for the properties of the base types of a type, and the position the type that
    /// inherits a type gives it for the properties of the type itself.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Before the other properties.
    /// </summary>
    Before = 1,

    /// <summary>
    /// After the other properties.
    /// </summary>
    After = 2
}
