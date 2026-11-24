namespace Serenity.Reporting;

/// <summary>
/// Describes a report category, e.g. "Northwind/Sales", for a page that lists reports in groups. It is placed on an
/// assembly, as a category has no type of its own.
/// </summary>
/// <param name="key">The key of the category, e.g. "Northwind/Sales".</param>
/// <example>[assembly: ReportCategory("Northwind/Sales", Description = "Revenue over time", IconClass = "fa fa-chart-bar", Order = 1)]</example>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ReportCategoryAttribute(string key) : Attribute
{
    /// <summary>
    /// The key of the category.
    /// </summary>
    public string Key { get; } = key ?? throw new ArgumentNullException(nameof(key));

    /// <summary>
    /// The title of the category, if it is not the last part of its key. A "Report.Category.{Key}" text, with dots in
    /// place of the slashes of its key, overrides it.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// A short description of the category. A "Report.Category.{Key}.Description" text overrides it.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The icon class of the category, which is also the icon of its reports that have none.
    /// </summary>
    public string? IconClass { get; set; }

    /// <summary>
    /// The order of the category among its siblings.
    /// </summary>
    public int Order { get; set; }
}
