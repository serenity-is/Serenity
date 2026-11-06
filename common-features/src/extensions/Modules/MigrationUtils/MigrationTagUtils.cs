using System.Reflection;
using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Tag matching utilities for migrations that use dimensioned tags, e.g. <c>db:Default</c> for
/// the database a migration targets, or <c>env:Staging</c> for the environments it may run in.
/// </summary>
/// <remarks>
/// FluentMigrator treats all tags of a migration as a single flat set, so tags coming from
/// different dimensions interfere with each other: a <see cref="TagBehavior.RequireAny"/>
/// database tag makes the migration run regardless of any <see cref="TagBehavior.RequireAll"/>
/// environment tag on the same class, as the two attributes are OR'ed.
/// <para>
/// This matcher groups the tags by their dimension (the part before the <c>:</c>), applies the
/// same rule as FluentMigrator within each group, but requires all groups to match. A dimension
/// that a migration declares must also be requested by the migration runner, otherwise the
/// migration is skipped, which is what makes e.g. an <c>env:Staging</c> tag a real gate.
/// </para>
/// <para>
/// The database dimension is a partition and not a requirement: a migration matches when any
/// of the databases it declares is requested, regardless of the <see cref="TagsAttribute"/>
/// behavior, so a hand written <c>[Tags("db:Default")]</c> and <see cref="TargetDBAttribute"/>
/// behave the same, even when the runner requests several databases at once, e.g. all the
/// connection keys that fall back to the one being migrated.
/// </para>
/// <para>
/// <see cref="TargetDBAttribute"/> also declares its tag in a legacy <c>DatabaseKey + "DB"</c>
/// form, e.g. <c>DefaultDB</c>, for migration runners that do not group tags by dimension (e.g.
/// ones using the default FluentMigrator matching). A legacy tag is treated like its
/// <see cref="TargetDBAttribute.TagPrefix"/> form, e.g. <c>[Tags("DefaultDB")]</c> like
/// <c>db:Default</c>, but only when that database is actually requested; otherwise it stays an
/// ordinary tag without a dimension, as it was before tags had dimensions. Tags of other
/// dimensions, e.g. <c>env:BackupDB</c>, are never treated as database tags.
/// </para>
/// <para>
/// Tag comparison is case sensitive, as in FluentMigrator. See <see cref="CreateMatcher"/>
/// for a function that can be assigned to <c>IMigrationRunnerConventions.TypeHasMatchingTags</c>.
/// </para>
/// </remarks>
public static class MigrationTagUtils
{
    /// <summary>
    /// The internal name of the database dimension, e.g. <c>db</c> for the tags starting with
    /// <see cref="TargetDBAttribute.TagPrefix"/>.
    /// </summary>
    private static readonly string DatabaseGroup = GetGroup(TargetDBAttribute.TagPrefix);

    /// <summary>
    /// Gets the dimension of a tag as written, e.g. <c>db</c> for <c>db:Default</c>, <c>env</c>
    /// for <c>env:Staging</c>, and an empty string for a tag that has no dimension, e.g.
    /// <c>Core</c> or the legacy <c>DefaultDB</c>.
    /// </summary>
    /// <param name="tag">The tag. Can be <c>null</c> or empty.</param>
    /// <returns>The dimension of the tag.</returns>
    /// <remarks>
    /// This is the dimension of the tag's syntax, so a legacy <c>DefaultDB</c> tag is reported
    /// as having no dimension, even though <see cref="MatchesTags"/> treats it as <c>db:Default</c>
    /// when that database is requested by the migration runner.
    /// </remarks>
    public static string GetGroup(string? tag)
    {
        if (string.IsNullOrEmpty(tag))
            return string.Empty;

        var colon = tag.IndexOf(':', StringComparison.Ordinal);
        return colon > 0 ? tag[..colon] : string.Empty;
    }

    /// <summary>
    /// Creates a function that can be assigned to
    /// <c>IMigrationRunnerConventions.TypeHasMatchingTags</c> to match migrations by dimensions.
    /// </summary>
    /// <returns>The tag matching function.</returns>
    public static Func<Type, IEnumerable<string>, bool> CreateMatcher()
    {
        return static (type, tags) => MatchesTags(type, tags);
    }

    /// <summary>
    /// Determines whether the tags declared by a migration type match the requested tags.
    /// </summary>
    /// <param name="type">The migration type.</param>
    /// <param name="requestedTags">The tags passed to the migration runner, e.g. <c>db:Default</c>.</param>
    /// <returns><c>true</c> if the type matches the requested tags.</returns>
    /// <remarks>
    /// A type without any <see cref="TagsAttribute"/> always returns <c>false</c>, even if
    /// no tags are requested, so it is up to the caller (e.g. <c>IncludeUntaggedMigrations</c>)
    /// to decide whether untagged migrations should run.
    /// </remarks>
    public static bool MatchesTags(Type type, IEnumerable<string>? requestedTags)
    {
        ArgumentNullException.ThrowIfNull(type);

        var requested = requestedTags?.Select(NormalizeTag).OfType<string>().ToArray() ?? [];
        if (requested.Length == 0)
            return false;

        // a legacy tag, e.g. DefaultDB, is only treated as a database tag when that database is
        // actually requested, otherwise it stays an ordinary tag without a dimension
        var declared = GetDeclaredTags(type, new HashSet<string>(requested, StringComparer.Ordinal));
        if (declared.Count == 0)
            return false;

        foreach (var group in declared.Select(x => x.Group).Distinct(StringComparer.Ordinal))
        {
            if (!MatchesGroup(declared, group, requested))
                return false;
        }

        return true;
    }

    private static bool MatchesGroup(IReadOnlyList<DeclaredTag> declared, string group, string[] requestedTags)
    {
        var requested = requestedTags.Where(x => GetGroup(x) == group).ToArray();
        if (requested.Length == 0)
            return false;

        // the database dimension is a partition, so any of the declared databases matching any
        // of the requested ones is enough, regardless of the attribute behavior. Otherwise a
        // hand written [Tags("db:Default")] would require all the requested databases, e.g. the
        // connection keys that fall back to the default one, which it can never declare all of
        if (group == DatabaseGroup)
            return declared.Any(x => x.Group == group && requested.Contains(x.Name));

        // same rules as FluentMigrator's DefaultMigrationConventions.TypeHasMatchingTags,
        // but applied to the tags of this group only
        var requireAll = GetTagNames(declared, group, TagBehavior.RequireAll);
        if (requireAll.Count > 0 && requested.All(requireAll.Contains))
            return true;

        var requireAny = GetTagNames(declared, group, TagBehavior.RequireAny);
        return requireAny.Count > 0 && requested.Any(requireAny.Contains);
    }

    private static HashSet<string> GetTagNames(IReadOnlyList<DeclaredTag> declared,
        string group, TagBehavior behavior)
    {
        return [.. declared
            .Where(x => x.Group == group && x.Behavior == behavior)
            .Select(x => x.Name)];
    }

    private static List<DeclaredTag> GetDeclaredTags(Type type, HashSet<string> requestedTags)
    {
        var result = new List<DeclaredTag>();
        foreach (var attribute in GetTagsAttributes(type))
        {
            // a TargetDBAttribute declares its canonical db: tag itself, so its legacy tag is
            // only there for runners that do not group tags by dimension
            var legacyTag = attribute is TargetDBAttribute targetDB ?
                targetDB.DatabaseKey + TargetDBAttribute.LegacyTagSuffix : null;

            foreach (var tag in attribute.TagNames ?? [])
            {
                if (string.IsNullOrEmpty(tag) || tag == legacyTag)
                    continue;

                // a hand written legacy tag is only treated as a database tag when that database
                // is requested, otherwise it stays an ordinary tag, as before the tag dimensions
                var name = NormalizeTag(tag)!;
                if (name != tag && !requestedTags.Contains(name))
                    name = tag;

                result.Add(new DeclaredTag(name, GetGroup(name), attribute.Behavior));
            }
        }

        return [.. result.Distinct()];
    }

    /// <summary>
    /// Converts a legacy database tag, e.g. <c>DefaultDB</c>, to its canonical form, e.g.
    /// <c>db:Default</c>. Tags that already have a dimension are never converted, so e.g.
    /// <c>env:BackupDB</c> stays an environment tag, and other tags are returned unchanged.
    /// </summary>
    /// <remarks>
    /// Only one suffix is stripped, so a migration targeting the <c>AnalyticsDB</c> connection,
    /// which is declared as <c>AnalyticsDBDB</c>, is normalized back to <c>db:AnalyticsDB</c>.
    /// A hand written legacy tag has to include the appended suffix, i.e. <c>SomeDB</c> for the
    /// <c>Some</c> connection, exactly as <see cref="TargetDBAttribute"/> declares it.
    /// </remarks>
    private static string? NormalizeTag(string? tag)
    {
        if (string.IsNullOrEmpty(tag))
            return tag;

        if (tag.StartsWith(TargetDBAttribute.TagPrefix, StringComparison.Ordinal))
            return tag;

        // a tag that already has a dimension is not a legacy database tag, e.g. env:BackupDB
        if (tag.IndexOf(':', StringComparison.Ordinal) >= 0)
            return tag;

        if (tag.Length > TargetDBAttribute.LegacyTagSuffix.Length &&
            tag.EndsWith(TargetDBAttribute.LegacyTagSuffix, StringComparison.Ordinal))
            return TargetDBAttribute.TagPrefix + tag[..^TargetDBAttribute.LegacyTagSuffix.Length];

        return tag;
    }

    private static List<TagsAttribute> GetTagsAttributes(Type type)
    {
        return [.. type.GetCustomAttributes<TagsAttribute>(inherit: true)
            .Concat(type.GetInterfaces().SelectMany(x => x.GetCustomAttributes<TagsAttribute>(inherit: true)))
            .Distinct()];
    }

    private sealed record DeclaredTag(string Name, string Group, TagBehavior Behavior);
}
