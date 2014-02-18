using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Serenity;
using Serenity.Data;

namespace Serenity.Reporting
{
    public static class ReportRegistry
    {
        private static Dictionary<string, Report> reportByKey;
        private static Dictionary<string, List<Report>> reportsByCategory;

        private static string GetReportKey(Type type)
        {
            var attr = type.GetCustomAttribute<RegisterReportAttribute>(false);
            if (attr == null || attr.ReportKey.IsEmptyOrNull())
            {
                var name = type.Name;
                const string report = "Report";
                if (name.EndsWith(report))
                    name = name.Substring(0, name.Length - report.Length);

                return name;
            }

            return attr.ReportKey;
        }

        private static string GetReportCategory(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(CategoryAttribute), false);
            if (attr.Length == 1)
                return ((CategoryAttribute)attr[0]).Category;

            return String.Empty;
        }

        private static string GetReportCategoryTitle(string key)
        {
            return LocalText.TryGet("Report.Category." + key.Replace("/", ".")) ?? key;
        }

        private static void EnsureTypes()
        {
            if (reportsByCategory != null)
                return;

            var reportByKeyNew = new Dictionary<string, Report>();
            var reportsByCategoryNew = new Dictionary<string, List<Report>>();

            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<RegisterReportAttribute>(false);
                    if (attr != null)
                    {
                        var report = new Report(type);

                        reportByKeyNew[report.Key] = report;

                        var category = report.Category.Key;
                        List<Report> reports;

                        if (!reportsByCategoryNew.TryGetValue(category, out reports))
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

        public static bool HasAvailableReportsInCategory(string categoryKey)
        {
            EnsureTypes();

            List<Report> reports;
            if (!reportsByCategory.TryGetValue(categoryKey, out reports))
                return false;

            foreach (var report in reports)
                if (IoC.Resolve<IPermissionService>().HasPermission(report.Permission))
                    return true;

            return false;
        }

        public static IEnumerable<Report> GetAvailableReportsInCategory(string categoryKey)
        {
            EnsureTypes();

            var list = new List<Report>();
            var permissionService = IoC.Resolve<IPermissionService>();

            foreach (var k in reportsByCategory)
                if (categoryKey.IsEmptyOrNull() ||
                    String.Compare(k.Key, categoryKey, StringComparison.OrdinalIgnoreCase) == 0 ||
                    (categoryKey ?? "").StartsWith(k.Key + "/", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var report in k.Value)
                        if (report.Permission.IsEmptyOrNull() ||
                            permissionService.HasPermission(report.Permission))
                        {
                            list.Add(report);
                        }
                }

            list.Sort((x, y) => (x.Title ?? "").CompareTo(y.Title ?? ""));

            return list;
        }

        public static Report GetReport(string reportKey)
        {
            EnsureTypes();

            if (reportByKey.IsEmptyOrNull())
                throw new ArgumentNullException("reportKey");

            Report report;
            if (reportByKey.TryGetValue(reportKey, out report))
                return report;

            return null;
        }

        public class Report
        {
            public Type Type { get; private set; }
            public string Key { get; private set; }           
            public string Permission { get; private set; }
            public string Title { get; private set; }
            public Category Category { get; private set; }

            public Report(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                this.Type = type;

                this.Key = GetReportKey(type);

                var attr = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (attr.Length == 1)
                    this.Title = ((DisplayNameAttribute)attr[0]).DisplayName;

                var category = GetReportCategory(type);
                this.Category = new ReportRegistry.Category(category, GetReportCategoryTitle(category));

                attr = type.GetCustomAttributes(typeof(RequiredPermissionAttribute), false);
                if (attr.Length == 1)
                    this.Permission = ((RequiredPermissionAttribute)attr[0]).Permission;
            }
        }

        public class Category
        {
            public string Key { get; private set; }
            public string Title { get; private set; }

            public Category(string key, string title)
            {
                this.Key = key;
                this.Title = title;
            }
        }
    }
}