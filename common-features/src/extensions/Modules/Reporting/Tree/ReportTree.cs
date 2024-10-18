using Serenity.Reporting;

namespace Serenity.Extensions;

public class ReportTree
{
    public Category Root { get; set; }

    private static readonly char[] slashSeparator = ['/'];

    public ReportTree()
    {
        Root = new Category();
    }

    public class Category
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public List<Category> SubCategories { get; private set; }
        public List<ReportRegistry.Report> Reports { get; private set; }

        public Category()
        {
            SubCategories = [];
            Reports = [];
        }
    }

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