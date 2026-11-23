namespace Serenity.CodeGeneration;

/// <summary>
/// A key that the writer of the text keys (<see cref="TextKeysWriter"/>) could not turn into a constant of its own,
/// or turned into one with a name other than the key implies, which the generators report as a diagnostic.
/// </summary>
public class TextKeyDiagnostic
{
    /// <summary>
    /// A segment of the key has no characters that an identifier can have, so the key has no constant of its own.
    /// </summary>
    public const string UnmapableSegment = "SRTXT001";

    /// <summary>
    /// The name of a key is the name of another key of the same class already, and the key can not keep a name of
    /// its own, so it has no constant, e.g. "X.x y" and "X.x-y" next to "X.xy".
    /// </summary>
    public const string NameTaken = "SRTXT002";

    /// <summary>
    /// Initializes a new instance of the <see cref="TextKeyDiagnostic"/> class.
    /// </summary>
    /// <param name="id">The id of the diagnostic, e.g. <see cref="UnmapableSegment"/>.</param>
    /// <param name="message">The message, which names the key.</param>
    public TextKeyDiagnostic(string id, string message)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// Gets the id of the diagnostic, one of the constants of this class.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the message of the diagnostic.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc/>
    public override string ToString() => Id + ": " + Message;
}
