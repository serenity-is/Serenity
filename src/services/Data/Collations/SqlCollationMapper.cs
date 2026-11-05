namespace Serenity.Data;

/// <summary>
/// Maps an engine independent <see cref="SqlCollationSpec"/> to the name of a collation of a database engine.
/// </summary>
/// <remarks>
/// A culture without a collation never has a fallback. With tolerant mapping, a collation that
/// only differs in case or accent sensitivity can be used, when the engine has no exact match. Names
/// are returned as is, so callers should quote them where the engine requires it, e.g. for Postgres.
/// </remarks>
public static class SqlCollationMapper
{
    /// <summary>Maps the comparer of the connection, throws if there is no exact match.</summary>
    public const string AutoMap = "automap";
    /// <summary>Maps the comparer of the connection, throws only if the culture can't be mapped.</summary>
    public const string AutoMapTolerant = "automaptolerant";
    /// <summary>Maps the comparer of the connection, uses no collation if there is no exact match.</summary>
    public const string TryAutoMap = "tryautomap";
    /// <summary>Maps the comparer of the connection, uses no collation only if the culture can't be mapped.</summary>
    public const string TryAutoMapTolerant = "tryautomaptolerant";

    private static readonly Dictionary<string, string> sqlServerPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        [""] = "Latin1_General_100",
        ["ar"] = "Arabic_100",
        ["az"] = "Azeri_Latin_100",
        ["cs"] = "Czech_100",
        ["da"] = "Danish_Greenlandic_100",
        ["de"] = "Latin1_General_100",
        ["el"] = "Greek_100",
        ["en"] = "Latin1_General_100",
        ["es"] = "Modern_Spanish_100",
        ["fi"] = "Finnish_Swedish_100",
        ["fr"] = "French_100",
        ["he"] = "Hebrew_100",
        ["hu"] = "Hungarian_100",
        ["it"] = "Latin1_General_100",
        ["ja"] = "Japanese_XJIS_100",
        ["ko"] = "Korean_100",
        ["nb"] = "Norwegian_100",
        ["nl"] = "Latin1_General_100",
        ["nn"] = "Norwegian_100",
        ["pl"] = "Polish_100",
        ["pt"] = "Latin1_General_100",
        ["ro"] = "Romanian_100",
        ["ru"] = "Cyrillic_General_100",
        ["sv"] = "Finnish_Swedish_100",
        ["tr"] = "Turkish_100",
        ["uk"] = "Ukrainian_100",
        ["zh"] = "Chinese_Simplified_Pinyin_100",
        ["zh-CN"] = "Chinese_Simplified_Pinyin_100",
        ["zh-TW"] = "Chinese_Traditional_Stroke_Count_100"
    };

    /// <summary>The cultures MySQL sorts by the root collation, as they have no tailored one.</summary>
    private static readonly HashSet<string> mySqlRootCultures = new(StringComparer.OrdinalIgnoreCase)
    {
        "", "de", "en", "fr", "it", "nl", "pt"
    };

    /// <summary>The cultures with a tailored MySQL 8 collation, and the name of their older collation if any.</summary>
    private static readonly Dictionary<string, string?> mySqlCultures = new(StringComparer.OrdinalIgnoreCase)
    {
        ["cs"] = "czech",
        ["da"] = "danish",
        ["es"] = "spanish",
        ["hu"] = "hungarian",
        ["pl"] = "polish",
        ["ro"] = "romanian",
        ["ru"] = null,
        ["sv"] = "swedish",
        ["tr"] = "turkish"
    };

    /// <summary>
    /// Returns true if the value is one of the automap keywords, which
    /// can be used as a collation setting of a connection.
    /// </summary>
    /// <param name="collation">The collation setting.</param>
    public static bool IsAutoMap(string? collation)
    {
        return collation is not null &&
            (collation.Equals(AutoMap, StringComparison.OrdinalIgnoreCase) ||
             collation.Equals(AutoMapTolerant, StringComparison.OrdinalIgnoreCase) ||
             collation.Equals(TryAutoMap, StringComparison.OrdinalIgnoreCase) ||
             collation.Equals(TryAutoMapTolerant, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Maps the specified comparer specification according to an automap keyword.
    /// </summary>
    /// <param name="autoMap">One of the automap keywords.</param>
    /// <param name="comparer">The comparer specification of the connection.</param>
    /// <param name="serverType">The server type of the dialect.</param>
    /// <param name="target">What the collation is mapped for.</param>
    /// <param name="exists">Optional check for whether a collation exists on the server.</param>
    /// <returns>The collation name, or null if it can't be mapped and the keyword is a try variant.</returns>
    /// <exception cref="NotSupportedException">The comparer can't be mapped and the keyword is not a try variant.</exception>
    public static string? ResolveAutoMap(string autoMap, string? comparer, string serverType,
        SqlCollationTarget target, Func<string, bool>? exists = null)
    {
        ArgumentNullException.ThrowIfNull(autoMap);

        var isTry = autoMap.StartsWith("try", StringComparison.OrdinalIgnoreCase);
        var tolerant = autoMap.EndsWith("tolerant", StringComparison.OrdinalIgnoreCase);

        if (!SqlCollationSpec.TryParse(comparer, out var spec))
            return isTry ? null : throw new NotSupportedException(
                $"Collation '{autoMap}' requires the comparer of the connection to be Ordinal, Invariant or " +
                $"a specific culture, as a collation can't depend on the current culture. Comparer is '{comparer}'.");

        return isTry
            ? TryMap(serverType, spec, target, tolerant, exists)
            : Map(serverType, spec, target, tolerant, exists);
    }

    /// <summary>
    /// Maps the specified comparison to a collation name.
    /// </summary>
    /// <param name="serverType">The server type of the dialect.</param>
    /// <param name="spec">The comparison.</param>
    /// <param name="target">What the collation is mapped for.</param>
    /// <param name="tolerant">True to allow a collation that only differs in case or accent sensitivity.</param>
    /// <param name="exists">Optional check for whether a collation exists on the server.</param>
    /// <returns>The collation name.</returns>
    /// <exception cref="NotSupportedException">There is no such collation.</exception>
    public static string Map(string serverType, SqlCollationSpec spec, SqlCollationTarget target,
        bool tolerant = false, Func<string, bool>? exists = null)
    {
        var collation = TryMap(serverType, spec, target, tolerant, exists);
        if (collation is not null)
            return collation;

        var hasApproximate = TryMap(serverType, spec, target, tolerant: true, exists) is not null;
        throw new NotSupportedException($"There is no {target.ToString().ToLowerInvariant()} collation for " +
            $"'{spec}' on {serverType}." + (hasApproximate ? " A tolerant mapping would find one that differs " +
            "in case or accent sensitivity." : ""));
    }

    /// <summary>
    /// Tries to map the specified comparison to a collation name.
    /// </summary>
    /// <param name="serverType">The server type of the dialect.</param>
    /// <param name="spec">The comparison.</param>
    /// <param name="target">What the collation is mapped for.</param>
    /// <param name="tolerant">True to allow a collation that only differs in case or accent sensitivity.</param>
    /// <param name="exists">Optional check for whether a collation exists on the server.</param>
    /// <returns>The collation name, or null if there is no such collation.</returns>
    public static string? TryMap(string serverType, SqlCollationSpec spec, SqlCollationTarget target,
        bool tolerant = false, Func<string, bool>? exists = null)
    {
        ArgumentNullException.ThrowIfNull(serverType);

        foreach (var (name, exact) in GetCandidates(serverType, spec, target))
        {
            if ((exact || tolerant) && (exists is null || exists(name)))
                return name;
        }

        return null;
    }

    /// <summary>
    /// Gets the possible collations in the order of preference, which is exact ones first, then the
    /// ones that keep the case sensitivity with different accent sensitivity, then the rest.
    /// </summary>
    private static List<(string Name, bool Exact)> GetCandidates(string serverType, SqlCollationSpec spec,
        SqlCollationTarget target)
    {
        if (serverType.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase))
            return GetSqlServerCandidates(spec);

        if (serverType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase))
            return GetPostgresCandidates(spec);

        if (serverType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase))
            return target == SqlCollationTarget.Database ? GetMySqlDatabaseCandidates(spec) : GetMySqlColumnCandidates(spec);

        if (serverType.StartsWith("Firebird", StringComparison.OrdinalIgnoreCase))
            return GetFirebirdCandidates(spec);

        if (serverType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase) && target == SqlCollationTarget.Column)
            return GetSqliteCandidates(spec);

        // a column collation on Oracle fails unless MAX_STRING_SIZE=EXTENDED is
        // set on the server, and it has no database level collation to create with
        return [];
    }

    private static IEnumerable<string> GetCultureLookups(string culture)
    {
        yield return culture;

        var separator = culture.IndexOf('-', StringComparison.Ordinal);
        if (separator > 0)
            yield return culture[..separator];
    }

    private static List<(string, bool)> GetSqlServerCandidates(SqlCollationSpec spec)
    {
        if (spec.Culture is null)
            return [(spec.IgnoreCase ? "Latin1_General_100_CI_AS" : "Latin1_General_100_BIN2", !spec.IgnoreAccents)];

        foreach (var lookup in GetCultureLookups(spec.Culture))
        {
            if (sqlServerPrefixes.TryGetValue(lookup, out var prefix))
                return [($"{prefix}_{(spec.IgnoreCase ? "CI" : "CS")}_{(spec.IgnoreAccents ? "AI" : "AS")}", true)];
        }

        return [];
    }

    /// <summary>
    /// All predefined Postgres collations are deterministic, so
    /// the ones here can only compare case and accent sensitively.
    /// </summary>
    private static List<(string, bool)> GetPostgresCandidates(SqlCollationSpec spec)
    {
        var exact = !spec.IgnoreCase && !spec.IgnoreAccents;
        if (spec.Culture is null)
            return [("C", exact)];

        if (spec.Culture.Length == 0)
            return [("und-x-icu", exact)];

        return [.. GetCultureLookups(spec.Culture).Select(culture => (culture + "-x-icu", exact))];
    }

    private static List<(string, bool)> GetMySqlDatabaseCandidates(SqlCollationSpec spec)
    {
        if (spec.Culture is null)
        {
            return spec.IgnoreCase
                ? [("utf8mb4_0900_as_ci", true), ("utf8mb4_general_ci", false), ("utf8mb4_bin", false)]
                : [("utf8mb4_bin", true)];
        }

        string prefix, legacy;
        var language = GetCultureLookups(spec.Culture).Last();
        if (mySqlRootCultures.Contains(language))
        {
            prefix = "utf8mb4_0900";
            legacy = "utf8mb4_unicode_ci";
        }
        else if (mySqlCultures.TryGetValue(language, out var legacyName))
        {
            prefix = $"utf8mb4_{language.ToLowerInvariant()}_0900";
            legacy = legacyName is null ? "" : $"utf8mb4_{legacyName}_ci";
        }
        else
            return [];

        var result = new List<(string, bool)>();
        void add(string suffix, bool exact)
        {
            result.Add(($"{prefix}_{suffix}", exact));
        }
        void addLegacy(bool exact)
        {
            if (legacy.Length > 0)
                result.Add((legacy, exact));
        }

        if (spec.IgnoreCase && spec.IgnoreAccents)
        {
            add("ai_ci", true);
            addLegacy(true);
        }
        else if (spec.IgnoreCase)
        {
            if (prefix == "utf8mb4_0900")
                add("as_ci", true);
            add("ai_ci", false);
            addLegacy(false);
            add("as_cs", false);
        }
        else if (spec.IgnoreAccents)
        {
            add("as_cs", false);
        }
        else
        {
            add("as_cs", true);
        }

        return result;
    }

    /// <summary>
    /// FluentMigrator generates string columns as NVARCHAR on MySQL, which
    /// means utf8mb3, so the utf8mb4 collations are not valid for them.
    /// </summary>
    private static List<(string, bool)> GetMySqlColumnCandidates(SqlCollationSpec spec)
    {
        if (spec.Culture is null)
            return spec.IgnoreCase ? [("utf8_general_ci", false), ("utf8_bin", false)] : [("utf8_bin", true)];

        var language = GetCultureLookups(spec.Culture).Last();
        string? name = null;
        if (mySqlRootCultures.Contains(language))
            name = "utf8_unicode_ci";
        else if (mySqlCultures.TryGetValue(language, out var legacyName) && legacyName is not null)
            name = $"utf8_{legacyName}_ci";

        return name is null ? [] : [(name, spec.IgnoreCase && spec.IgnoreAccents)];
    }

    /// <summary>These are the collations of the UTF8 character set.</summary>
    private static List<(string, bool)> GetFirebirdCandidates(SqlCollationSpec spec)
    {
        if (spec.Culture is null)
            return [(spec.IgnoreCase ? "UNICODE_CI" : "UCS_BASIC", !spec.IgnoreAccents)];

        if (spec.Culture.Length == 0)
            return [(spec.IgnoreCase ? (spec.IgnoreAccents ? "UNICODE_CI_AI" : "UNICODE_CI") : "UNICODE",
                !spec.IgnoreAccents || spec.IgnoreCase)];

        return [];
    }

    private static List<(string, bool)> GetSqliteCandidates(SqlCollationSpec spec)
    {
        if (spec.Culture is not null)
            return [];

        return [(spec.IgnoreCase ? "NOCASE" : "BINARY", !spec.IgnoreAccents)];
    }
}
