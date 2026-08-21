using Serenity.Reporting;

namespace Serenity.Extensions;

/// <summary>
/// A tree of report categories and reports.
/// </summary>
public class ReportTree
{
    /// <summary>
    /// The root category of the tree.
    /// </summary>
    public Category Root { get; set; }

    private static readonly char[] slashSeparator = ['/'];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportTree"/> class.
    /// </summary>
    public ReportTree()
    {
        Root = new Category();
    }

    /// <summary>
    /// A category in the report tree.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The category key.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The category title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The sub categories.
        /// </summary>
        public List<Category> SubCategories { get; private set; }
        /// <summary>
        /// The reports in this category.
        /// </summary>
        public List<ReportRegistry.Report> Reports { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        public Category()
        {
            SubCategories = [];
            Reports = [];
        }
    }

    /// <summary>
    /// Builds a report tree from the specified reports.
    /// </summary>
    /// <param name="reports">The reports.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <param name="rootPath">The root path.</param>
    /// <param name="categoryOrder">The category order.</param>
    /// <returns>The report tree.</returns>
    public static ReportTree FromList(IEnumerable<ReportRegistry.Report> reports, ITextLocalizer localizer,
        string rootPath = null, string categoryOrder = null)
    {
        ArgumentNullException.ThrowIfNull(reports);

        rootPath ??= "";
        categoryOrder ??= "";

        var tree = new ReportTree();

        var categoryByKey = new Dictionary<string, Category>(StringComparer.CurrentCultureIgnoreCase);

        foreach (var report in reports)
        {
            if (categoryByKey.TryGetValue(report.Category.Key ?? "", out Category category))
            {
                category.Reports.Add(report);
                continue;
            }

            var parts = (report.Category.Key ?? "Other")
                .Split(slashSeparator, StringSplitOptions.RemoveEmptyEntries);

            string current = "";
            category = null;
            foreach (var part in parts)
            {
                string prior = current;

                if (current.Length > 0)
                    current += "/";

                current += part;

                if (current.Length <= rootPath.Length)
                    continue;

                if (!categoryByKey.TryGetValue(current ?? "", out category))
                {
                    category = new Category
                    {
                        Key = current,
                        Title = ReportRegistry.GetReportCategoryTitle(current, localizer)
                    };
                    categoryByKey[current] = category;

                    if (!categoryByKey.TryGetValue(prior, out Category value))
                        tree.Root.SubCategories.Add(category);
                    else
                    {
                        var x = value;
                        x.SubCategories.Add(category);
                    }
                }
            }

            if (category == null)
                tree.Root.Reports.Add(report);
            else
                category.Reports.Add(report);
        }

        var order = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var i = 0;
        foreach (var x in categoryOrder.Split([';']))
        {
            var xt = x.TrimToNull();
            if (xt != null)
                order[xt] = i++;
        }

        int sort(Category x, Category y)
        {
            var c = 0;

            if (x.Key != y.Key)
            {
                var c1 = order.TryGetValue(x.Key, out int v1) ? (int?)v1 : null;
                var c2 = order.TryGetValue(y.Key, out int v2) ? (int?)v2 : null;
                if (c1 != null && c2 != null)
                    c = c1.Value - c2.Value;
                else if (c1 != null)
                    c = -1;
                else if (c2 != null)
                    c = 1;
            }

            if (c == 0)
                c = string.Compare(x.Title, y.Title, StringComparison.CurrentCultureIgnoreCase);

            return c;
        }

        foreach (var category in categoryByKey.Values)
            category.SubCategories?.Sort(sort);

        tree.Root.SubCategories.Sort(sort);

        return tree;
    }
}