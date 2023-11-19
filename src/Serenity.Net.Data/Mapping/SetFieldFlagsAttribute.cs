namespace Serenity.Data.Mapping;

/// <summary>
/// Used to turn on (include) or turn off (exclude) field flags.
/// </summary>
/// <remarks>
/// Turn on or off field flags.
/// </remarks>
/// <param name="add">Set of flags to turn on (include)</param>
/// <param name="remove">Set of flags to turn off (exclude)</param>
public class SetFieldFlagsAttribute(FieldFlags add, FieldFlags remove = FieldFlags.None) : Attribute
{

    /// <summary>
    /// Gets the include flags.
    /// </summary>
    /// <value>
    /// The include flags.
    /// </value>
    public FieldFlags Add { get; private set; } = add;

    /// <summary>
    /// Gets the exclude flags.
    /// </summary>
    /// <value>
    /// The exclude flags.
    /// </value>
    public FieldFlags Remove { get; private set; } = remove;
}