namespace Serenity.Data;

/// <summary>
/// Enables capture logging for a row type
/// </summary>
/// <remarks>
/// Creates an instance of the attribute
/// </remarks>
/// <param name="logRow">The log row type used for
/// this row type the attribute is placed on.</param>
/// <exception cref="ArgumentNullException"></exception>
public class CaptureLogAttribute(Type logRow) : Attribute
{

    /// <summary>
    /// Log row type
    /// </summary>
    public Type LogRow { get; private set; } = logRow ?? throw new ArgumentNullException(nameof(logRow));

    /// <summary>
    /// Gets / sets mapped ID field. It is tried to be
    /// automatically determined if not specified.
    /// </summary>
    public string MappedIdField { get; set; }
}