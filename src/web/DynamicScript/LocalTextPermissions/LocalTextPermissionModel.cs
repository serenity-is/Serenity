namespace Serenity.Web;

/// <summary>
/// Which permissions local text keys require, as determined once by <see cref="LocalTextPermissionFilter"/>.
/// Permissions are referred to by their index in <see cref="Permissions"/>, so that the access of a user can be
/// represented as a boolean per permission.
/// </summary>
/// <param name="permissions">Distinct permissions referred to by gates, grants and restrictions</param>
/// <param name="gates">Key prefixes (ending with a dot) and the permissions any of which allows the keys starting with them</param>
/// <param name="grants">Exact keys and the permissions any of which allows them, regardless of restrictions and gates,
/// e.g. the texts used by forms and columns</param>
/// <param name="restrictions">Exact keys and the permissions any of which allows them, which decides for the key
/// instead of the gates, e.g. the texts of a nested local texts class with a permission</param>
/// <param name="alwaysInclude">Key prefixes that are always allowed</param>
public class LocalTextPermissionModel(string[] permissions, Dictionary<string, int[]> gates,
    Dictionary<string, int[]> grants, Dictionary<string, int[]> restrictions, string[] alwaysInclude)
{
    /// <summary>Distinct permissions referred to by gates, grants and restrictions</summary>
    public string[] Permissions { get; } = permissions ?? throw new ArgumentNullException(nameof(permissions));

    /// <summary>Key prefixes and the permissions any of which allows the keys starting with them</summary>
    public IReadOnlyDictionary<string, int[]> Gates { get; } = gates ?? throw new ArgumentNullException(nameof(gates));

    /// <summary>Exact keys and the permissions any of which allows them, regardless of restrictions and gates</summary>
    public IReadOnlyDictionary<string, int[]> Grants { get; } = grants ?? throw new ArgumentNullException(nameof(grants));

    /// <summary>Exact keys and the permissions any of which allows them, deciding instead of the gates</summary>
    public IReadOnlyDictionary<string, int[]> Restrictions { get; } = restrictions ?? throw new ArgumentNullException(nameof(restrictions));

    /// <summary>Key prefixes that are always allowed</summary>
    public string[] AlwaysInclude { get; } = alwaysInclude ?? throw new ArgumentNullException(nameof(alwaysInclude));

    private static bool HasAny(int[] indexes, bool[] granted)
    {
        foreach (var index in indexes)
            if (granted[index])
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if a user with the granted permissions may see the key. A key is allowed when it starts with
    /// an always included prefix, or when it has a grant the user has. Otherwise its restriction decides if it has
    /// one, or else the longest gate prefix it starts with, and it is allowed when neither exists.
    /// </summary>
    /// <param name="key">Local text key</param>
    /// <param name="granted">Whether the user has each permission in <see cref="Permissions"/></param>
    public bool IsAllowed(string key, bool[] granted)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(granted);

        foreach (var prefix in AlwaysInclude)
            if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;

        // texts used by a form or columns script the user can access are always allowed
        if (Grants.TryGetValue(key, out var grant) && HasAny(grant, granted))
            return true;

        if (Restrictions.TryGetValue(key, out var restriction))
            return HasAny(restriction, granted);

        var dot = key.Length;
        while ((dot = key.LastIndexOf('.', dot - 1)) > 0)
        {
            if (Gates.TryGetValue(key[..(dot + 1)], out var gate))
                return HasAny(gate, granted);
        }

        return true;
    }
}
