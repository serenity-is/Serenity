using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Marks a migration that adds sample data, so it only runs when the migration runner requests
/// either all the sample data via <see cref="AllTag"/>, or the sample data of a module via
/// <see cref="GetTag(string)"/>, e.g. <c>sample:Dashboards</c>.
/// </summary>
/// <param name="key">The key of the module the sample data belongs to, e.g. <c>Dashboards</c>.</param>
public class SampleDataAttribute(string key)
    : TagsAttribute(TagBehavior.RequireAny, GetTag(key), AllTag)
{
    /// <summary>
    /// The tag that requests the sample data of all modules, i.e. <c>sample:All</c>.
    /// </summary>
    public const string AllTag = "sample:All";

    /// <summary>
    /// Gets the tag that requests the sample data of the module with the specified key.
    /// </summary>
    /// <param name="key">The module key, e.g. <c>Dashboards</c>.</param>
    /// <returns>The tag, e.g. <c>sample:Dashboards</c>.</returns>
    public static string GetTag(string key) => "sample:" + key;
}
