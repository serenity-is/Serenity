namespace Serenity.Data;

/// <summary>
/// Enables capture logging for a row type.
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="logRow">The log row type used for
/// this row type the attribute is placed on.</param>
/// <exception cref="ArgumentNullException"><paramref name="logRow"/> is <c>null</c>.</exception>
public class CaptureLogAttribute(Type logRow) : Attribute
{

    /// <summary>
    /// Gets the log row type.
    /// </summary>
    public Type LogRow { get; private set; } = logRow ?? throw new ArgumentNullException(nameof(logRow));

    /// <summary>
    /// Gets or sets the mapped ID field. It is tried to be
    /// automatically determined if not specified.
    /// </summary>
    public string MappedIdField { get; set; }
}