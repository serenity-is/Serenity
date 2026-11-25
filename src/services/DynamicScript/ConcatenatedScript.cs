namespace Serenity.Web;

/// <summary>
/// A dynamic script that is formed from the concatenation of other scripts.
/// Parts that the current user does not have permission for are not included,
/// and the cache suffix varies based on which parts are included, so that a
/// separate content is cached for each such combination.
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="parts">The script parts.</param>
/// <param name="separator">The separator.</param>
/// <exception cref="ArgumentNullException">Script parts are null.</exception>
public class ConcatenatedScript(IEnumerable<ConcatenatedScriptPart> parts,
    string separator = "\r\n;\r\n") : DynamicScript, ICacheSuffix
{
    private readonly string separator = separator;
    private readonly ConcatenatedScriptPart[] parts = [.. parts ?? throw new ArgumentNullException(nameof(parts))];

    /// <summary>
    /// Creates a new instance of the class with parts that are always included.
    /// </summary>
    /// <param name="scriptParts">The script parts.</param>
    /// <param name="separator">The separator.</param>
    public ConcatenatedScript(IEnumerable<Func<string>> scriptParts, string separator = "\r\n;\r\n")
        : this((scriptParts ?? throw new ArgumentNullException(nameof(scriptParts)))
            .Select(x => new ConcatenatedScriptPart(x)), separator)
    {
    }

    /// <summary>
    /// Gets a cache suffix that identifies which parts are included for the current user,
    /// combined with the cache suffixes of the parts themselves. Empty if no part has a
    /// permission check or a cache suffix.
    /// </summary>
    public string CacheSuffix
    {
        get
        {
            StringBuilder? sb = null;
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part.HasPermission is null && part.GetCacheSuffix is null)
                    continue;

                sb ??= new StringBuilder();
                sb.Append(i);
                if (part.HasPermission is not null && !part.HasPermission())
                {
                    sb.Append("-;");
                    continue;
                }

                sb.Append('+');
                sb.Append(part.GetCacheSuffix?.Invoke());
                sb.Append(';');
            }

            return sb?.ToString() ?? "";
        }
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        StringBuilder sb = new();

        foreach (var part in parts)
        {
            if (part.HasPermission is not null && !part.HasPermission())
                continue;

            string partSource = part.GetScript();
            if (sb.Length > 0 && !string.IsNullOrEmpty(separator))
                sb.AppendLine(separator);
            sb.AppendLine(partSource);
        }

        return sb.ToString();
    }
}
