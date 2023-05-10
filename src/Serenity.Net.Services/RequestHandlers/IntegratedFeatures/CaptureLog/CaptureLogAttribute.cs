namespace Serenity.Data;

/// <summary>
/// Enables capture logging for a row type
/// </summary>
public class CaptureLogAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="logRow">The log row type used for
    /// this row type the attribute is placed on.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CaptureLogAttribute(Type logRow)
    {
        LogRow = logRow ?? throw new ArgumentNullException(nameof(logRow));
    }

    /// <summary>
    /// Log row type
    /// </summary>
    public Type LogRow { get; private set; }

    /// <summary>
    /// Gets / sets mapped ID field. It is tried to be
    /// automatically determined if not specified.
    /// </summary>
    public string MappedIdField { get; set; }
}