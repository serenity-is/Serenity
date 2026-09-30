namespace Serenity.Data;

/// <summary>
/// Parameter struct.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Parameter"/> struct.
/// </remarks>
/// <param name="name">The name. Must start with "@": parameter names are
/// always "@"-prefixed, and dialect translation happens later when the
/// command is built (see SqlHelper.AddParamWithValue).</param>
/// <exception cref="ArgumentNullException">name is null or empty.</exception>
/// <exception cref="ArgumentOutOfRangeException">name doesn't start with "@",
/// or has nothing but whitespace after it.</exception>
public readonly struct Parameter(string name)
{
    private readonly string name = CheckName(name);

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public readonly string Name => name;

    /// <summary>
    /// Validates a parameter name, so that null, empty, "@"-only and
    /// whitespace-only names are rejected in one place. Used by this
    /// constructor and other parameter-name entry points
    /// (AddParam / SetParam / ParamCriteria).
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>The validated name.</returns>
    /// <exception cref="ArgumentNullException">name is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">name doesn't start with "@",
    /// or has nothing but whitespace after it.</exception>
    public static string CheckName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (!name.StartsWith('@') || name.AsSpan(1).IsWhiteSpace())
            throw new ArgumentOutOfRangeException(nameof(name));

        return name;
    }
}
