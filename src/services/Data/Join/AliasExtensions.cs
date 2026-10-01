namespace Serenity.Data;

/// <summary>
/// Contains extension methods for Alias objects.
/// </summary>
public static class AliasExtensions
{
    /// <summary>
    /// An alias carrying a table hint. The name stays clean so column references
    /// render normally; join renderers append the hint via JoinNameWithHints.
    /// </summary>
    private sealed class HintedAlias : Alias
    {
        public HintedAlias(string alias, string[] hints)
            : base(alias)
        {
            Hints = hints;
        }

        public HintedAlias(string table, string alias, string[] hints)
            : base(table, alias)
        {
            Hints = hints;
        }

        public string[] Hints { get; }
    }

    /// <summary>
    /// Gets the alias name as rendered in FROM / JOIN clauses, with the table
    /// hint appended when the alias carries one (see <see cref="WithTableHint(IAlias, string[])"/>).
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>The name for FROM / JOIN rendering.</returns>
    internal static string JoinNameWithHints(IAlias alias)
    {
        var hint = GetTableHint(alias);
        if (hint is not null)
            return alias.Name + " WITH(" + hint + ")";

        return alias.Name;
    }

    internal static string? GetTableHint(IAlias alias)
    {
        return alias is HintedAlias hinted ? string.Join(", ", hinted.Hints) : null;
    }

    /// <summary>
    /// Adds a WITH(...) table hint to the alias and returns a new alias.
    /// The alias name itself stays clean, so column references are unaffected;
    /// only FROM / JOIN rendering includes the hint.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="hints">Table hint keywords, e.g. "NOLOCK", "READUNCOMMITTED".
    /// Multiple hints are comma-separated in the rendered WITH clause. Hints
    /// from previous calls are retained, without adding duplicates.</param>
    /// <returns>A new alias carrying the table hint.</returns>
    /// <remarks>
    /// Table hints are SQL Server specific and are emitted as-is on all dialects.
    /// </remarks>
    /// <exception cref="ArgumentNullException">alias is null.</exception>
    /// <exception cref="ArgumentException">No hint is provided, or a hint is
    /// null, empty or whitespace.</exception>
    public static Alias WithTableHint(this IAlias alias, params string[] hints)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (hints is null || hints.Length == 0)
            throw new ArgumentException("At least one table hint must be provided!", nameof(hints));

        foreach (var hint in hints)
        {
            if (string.IsNullOrWhiteSpace(hint))
                throw new ArgumentException("Table hints can't be null, empty or whitespace!", nameof(hints));
        }

        var combined = (alias is HintedAlias hinted ? hinted.Hints : Array.Empty<string>())
            .Concat(hints)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (string.IsNullOrEmpty(alias.Table))
            return new HintedAlias(alias.Name, combined);

        return new HintedAlias(alias.Table, alias.Name, combined);
    }

    /// <summary>
    /// Adds a WITH(NOLOCK) to the alias and returns a new alias.
    /// The alias name itself stays clean, so column references are unaffected;
    /// only FROM / JOIN rendering includes the hint.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>A new alias with the WITH(NOLOCK) hint appended.</returns>
    /// <remarks>
    /// Table hints are SQL Server specific and are emitted as-is on all dialects.
    /// </remarks>
    /// <exception cref="ArgumentNullException">alias is null.</exception>
    public static Alias WithNoLock(this IAlias alias)
    {
        return (alias ?? throw new ArgumentNullException(nameof(alias))).WithTableHint("NOLOCK");
    }
}
