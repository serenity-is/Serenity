namespace Serenity.Reporting;

/// <summary>
/// Default report registry implementation
/// </summary>
public class ReportRegistry : IReportRegistry
{
    private Dictionary<string, Report> reportByKey;
    private Dictionary<string, List<Report>> reportsByCategory;
    private readonly IEnumerable<Type> types;
    private readonly IPermissionService permissions;
    private readonly ITextLocalizer localizer;

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="typeSource">The type source to search report types in</param>
    /// <param name="permissions">Permission service</param>
    /// <param name="localizer">Text localizer</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ReportRegistry(ITypeSource typeSource, IPermissionService permissions, ITextLocalizer localizer)
    {
        types = (typeSource ?? throw new ArgumentNullException(nameof(types)))
            .GetTypesWithAttribute(typeof(ReportAttribute));
        this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    /// <summary>
    /// Gets report key for the report type by looking at its ReportAttribute,
    /// returning type full name if it does not have a report key or the attribute.
    /// </summary>
    /// <param name="type">The report type</param>
    public static string GetReportKey(Type type)
    {
        var attr = type.GetCustomAttribute<ReportAttribute>(false);
        if (attr == null || attr.ReportKey.IsNullOrEmpty())
            return type.FullName;

        return attr.ReportKey;
    }

    private static string GetReportCategory(Type type)
    {
        var attr = type.GetCustomAttributes(typeof(CategoryAttribute), false);
        if (attr.Length == 1)
            return ((CategoryAttribute)attr[0]).Category;

        return string.Empty;
    }

    /// <summary>
    /// Gets category title for a category key
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <param name="localizer">Text localizer</param>
    public static string GetReportCategoryTitle(string key, ITextLocalizer localizer)
    {
        var title = localizer?.TryGet("Report.Category." + key.Replace("/", "."));
        if (title == null)
        {
            key ??= "";
            var idx = key.LastIndexOf('/');
            if (idx >= 0 && idx < key.Length - 1)
                key = key[(idx + 1)..];
            return key;
        }

        return title;
    }

    private void EnsureTypes()
    {
        if (reportsByCategory != null)
            return;

        var reportByKeyNew = new Dictionary<string, Report>();
        var reportsByCategoryNew = new Dictionary<string, List<Report>>();

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ReportAttribute>(false);
            if (attr != null)
            {
                var report = new Report(type, localizer);
                var key = report.Key.TrimToNull() ?? type.FullName;

                reportByKeyNew[key] = report;

                var category = report.Category.Key;

                if (!reportsByCategoryNew.TryGetValue(category, out List<Report> reports))
                {
                    reports = new List<Report>();
                    reportsByCategoryNew[category] = reports;
                }

                reports.Add(report);
            }
        }

        reportsByCategory = reportsByCategoryNew;
        reportByKey = reportByKeyNew;
    }

    /// <inheritdoc/>
    public bool HasAvailableReportsInCategory(string categoryKey)
    {
        EnsureTypes();

        if (!reportsByCategory.TryGetValue(categoryKey, out List<Report> reports))
            return false;

        foreach (var report in reports)
            if (report.Permission == null || permissions.HasPermission(report.Permission))
                return true;

        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<Report> GetAvailableReportsInCategory(string categoryKey)
    {
        EnsureTypes();

        var list = new List<Report>();

        foreach (var k in reportsByCategory)
            if (categoryKey.IsNullOrEmpty() ||
                string.Compare(k.Key, categoryKey, StringComparison.OrdinalIgnoreCase) == 0 ||
                (k.Key + "/").StartsWith((categoryKey ?? ""), StringComparison.OrdinalIgnoreCase))
            {
                foreach (var report in k.Value)
                    if (report.Permission == null || permissions.HasPermission(report.Permission))
                    {
                        list.Add(report);
                    }
            }

        list.Sort((x, y) => (x.Title ?? "").CompareTo(y.Title ?? ""));

        return list;
    }

    /// <summary>
    /// Returns report with the report key, 
    /// optionally validating its permissions.
    /// </summary>
    /// <param name="reportKey">Report key</param>
    /// <param name="validatePermission">Validate permission. Default true.</param>
    /// <exception cref="ArgumentNullException">reportKey is n ull</exception>
    public Report GetReport(string reportKey, bool validatePermission = true)
    {
        if (string.IsNullOrEmpty(reportKey))
            throw new ArgumentNullException(nameof(reportKey));

        EnsureTypes();

        if (reportByKey.TryGetValue(reportKey, out Report report))
        {
            if (validatePermission && report.Permission != null)
                permissions.ValidatePermission(report.Permission, localizer);

            return report;
        }

        return null;
    }

    /// <summary>
    /// Metadata for a registered report
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Type of the report
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Report key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Report permission
        /// </summary>
        public string Permission { get; private set; }

        /// <summary>
        /// Report title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The category
        /// </summary>
        public Category Category { get; private set; }

        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="localizer">Text localizer</param>
        /// <exception cref="ArgumentNullException">Type is null</exception>
        public Report(Type type, ITextLocalizer localizer)
        {
            Type = type ?? throw new ArgumentNullException("type");

            Key = GetReportKey(type);

            var attr = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attr.Length == 1)
                Title = ((DisplayNameAttribute)attr[0]).DisplayName;

            var category = GetReportCategory(type);
            Category = new Category(category, GetReportCategoryTitle(category, localizer));

            attr = type.GetCustomAttributes(typeof(RequiredPermissionAttribute), false);
            if (attr.Length > 0)
                Permission = ((RequiredPermissionAttribute)attr[0]).Permission ?? "?";
        }
    }

    /// <summary>
    /// Model for a report category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Key for the category
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Category title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="key">Category key</param>
        /// <param name="title">Category title</param>
        public Category(string key, string title)
        {
            Key = key;
            Title = title;
        }
    }
}