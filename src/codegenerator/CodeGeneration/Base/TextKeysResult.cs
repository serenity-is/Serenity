namespace Serenity.CodeGeneration;

/// <summary>
/// What the writer of the text keys (<see cref="TextKeysWriter"/>) wrote: the source of the constants, and the keys
/// that it could not carry as they are, see <see cref="TextKeyDiagnostic"/>.
/// </summary>
public class TextKeysResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextKeysResult"/> class.
    /// </summary>
    /// <param name="source">The source of the constants, which is empty when there are no keys.</param>
    /// <param name="diagnostics">The diagnostics of the keys that are not carried as they are.</param>
    /// <param name="constantCount">The number of the constants that the source has, which is less than the number of
    /// the keys when some of them could not be carried.</param>
    public TextKeysResult(string source, IReadOnlyList<TextKeyDiagnostic> diagnostics, int constantCount)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        ConstantCount = constantCount;
    }

    /// <summary>
    /// Gets the source of the constants, which is empty when there are no keys.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the diagnostics of the keys that are not carried as they are.
    /// </summary>
    public IReadOnlyList<TextKeyDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Gets the number of the constants that the source has, which is less than the number of the keys when some of
    /// them could not be carried, e.g. because they have no name of their own.
    /// </summary>
    public int ConstantCount { get; }

    /// <summary>
    /// Gets a value indicating whether any key is not carried as it is.
    /// </summary>
    public bool HasDiagnostics => Diagnostics.Count > 0;
}
