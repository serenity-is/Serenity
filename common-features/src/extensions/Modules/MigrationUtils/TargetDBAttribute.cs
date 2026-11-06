using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Marks a migration to run only on the specified database type.
/// </summary>
/// <remarks>
/// Both the <see cref="TagPrefix"/> and a legacy <c>DatabaseKey + "DB"</c> form of the tag are
/// declared, so data migrations that still generate the requested tags in the legacy form
/// (e.g. <c>DefaultDB</c>) keep matching. As <see cref="TagsAttribute"/> is applied once and the
/// behavior is <see cref="TagBehavior.RequireAny"/>, the extra tag cannot restrict matching for
/// runners that do not group tags by dimension.
/// </remarks>
public class TargetDBAttribute(string db)
    : TagsAttribute(TagBehavior.RequireAny, db + LegacyTagSuffix, GetTag(db))
{
    /// <summary>
    /// Returns the tag for the specified database key.
    /// </summary>
    /// <param name="db">The database key.</param>
    /// <returns>The tag corresponding to the specified database key.</returns>
    public static string GetTag(string db) => TagPrefix + db;

    /// <summary>
    /// The prefix used for database tags, e.g. <c>db:Default</c> for the <c>Default</c> connection.
    /// </summary>
    public const string TagPrefix = "db:";

    /// <summary>
    /// The legacy suffix used for database tags, e.g. <c>DefaultDB</c>. Kept for compatibility.
    /// </summary>
    internal const string LegacyTagSuffix = "DB";

    /// <summary>
    /// The connection key of the database this migration targets.
    /// </summary>
    public string DatabaseKey { get; } = db;
}
