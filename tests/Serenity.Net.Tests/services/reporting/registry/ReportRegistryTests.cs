namespace Serenity.Reporting;

public class ReportRegistryTests
{
    [Report]
    [Category("Cat1")]
    [DisplayName("Report One")]
    private class ReportOne
    {
    }

    [Report("Custom.Key")]
    [Category("Cat1/Sub")]
    [DisplayName("Report Two")]
    [RequiredPermission("Perm1")]
    private class ReportTwo
    {
    }

    [Report("Report.Three")]
    [Category("Cat2")]
    private class ReportThree
    {
    }

    [Report("Report.Four")]
    [Category("Cat2")]
    [DisplayText("Report title fallback", TextKey = "Site.ReportTitle")]
    private class ReportWithTextKey
    {
    }

    [Report("Report.Five")]
    [Category("Cat2")]
    [DisplayText("Site.LiteralReportTitle")]
    private class ReportWithKeyLikeLiteralTitle
    {
    }

    [Report("Report.Six")]
    [Category("Cat2")]
    [DisplayName("Site.LegacyReportTitle")]
    private class ReportWithLegacyKeyLikeTitle
    {
    }

    [Report("Report.Eight")]
    [CategoryName("Invariant category", TextKey = "Site.ReportCategory")]
    private class ReportWithCategoryTextKey
    {
    }

    [Report("Report.Nine")]
    [CategoryName("Site.LiteralReportCategory")]
    private class ReportWithKeyLikeLiteralCategory
    {
    }

    [Report("Report.Ten")]
    [Category("Site.LegacyReportCategory")]
    private class ReportWithLegacyKeyLikeCategory
    {
    }

    [Report("Report.Eleven", Order = 3)]
    [Category("Cat1")]
    [DescriptionText("What it shows", TextKey = "Site.ReportDescription")]
    [IconClass("fa fa-chart")]
    private class ReportWithDescription
    {
    }

    [Report("Report.Seven")]
    private class NoCategoryReport
    {
    }

    private class NotAReport
    {
    }

    private class FixedTypeSource(params Type[] types) : ITypeSource
    {
        public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType) => [];
        public IEnumerable<Type> GetTypes() => types;
        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType) => types;
        public IEnumerable<Type> GetTypesWithInterface(Type interfaceType) => [];
    }

    private static ReportRegistry.Report GetReportMeta<T>(ITextLocalizer? localizer = null)
    {
        return new ReportRegistry.Report(typeof(T), localizer ?? NullTextLocalizer.Instance);
    }

    [Fact]
    public void GetReportKey_Returns_FullName_When_No_Attribute()
    {
        Assert.Equal(typeof(NotAReport).FullName, ReportRegistry.GetReportKey(typeof(NotAReport)));
    }

    [Fact]
    public void GetReportKey_Returns_FullName_When_ReportKey_Is_Empty()
    {
        Assert.Equal(typeof(ReportOne).FullName, ReportRegistry.GetReportKey(typeof(ReportOne)));
    }

    [Fact]
    public void GetReportKey_Returns_Attribute_ReportKey()
    {
        Assert.Equal("Custom.Key", ReportRegistry.GetReportKey(typeof(ReportTwo)));
    }

    [Fact]
    public void ReportAttribute_Defaults_ReportKey_To_Null()
    {
        Assert.Null(new ReportAttribute().ReportKey);
    }

    [Fact]
    public void ReportAttribute_Uses_Passed_ReportKey()
    {
        Assert.Equal("abc", new ReportAttribute("abc").ReportKey);
    }

    [Fact]
    public void GetReportCategoryTitle_Returns_Localized_Title()
    {
        var localizer = new MockTextLocalizer(_ => "Localized Title");
        Assert.Equal("Localized Title",
            ReportRegistry.GetReportCategoryTitle("Cat1/Sub", localizer));
    }

    [Fact]
    public void GetReportCategoryTitle_Falls_Back_To_Last_Segment()
    {
        var localizer = new MockTextLocalizer(_ => null);
        Assert.Equal("Sub", ReportRegistry.GetReportCategoryTitle("Cat1/Sub", localizer));
    }

    [Fact]
    public void GetReportCategoryTitle_Returns_Key_When_No_Slash()
    {
        var localizer = new MockTextLocalizer(_ => null);
        Assert.Equal("Cat1", ReportRegistry.GetReportCategoryTitle("Cat1", localizer));
    }

    [Fact]
    public void Report_Metadata_Comes_From_Attributes()
    {
        var report = GetReportMeta<ReportTwo>();

        Assert.Equal(typeof(ReportTwo), report.Type);
        Assert.Equal("Custom.Key", report.Key);
        Assert.Equal("Report Two", report.Title);
        Assert.Equal("Perm1", report.Permission);
        Assert.Equal("Cat1/Sub", report.Category.Key);
        Assert.Equal("Sub", report.Category.Title);
    }

    [Fact]
    public void Report_Title_Uses_DisplayText_Key_And_Falls_Back_To_Invariant_Text()
    {
        var report = new ReportRegistry.Report(typeof(ReportWithTextKey),
            new MockTextLocalizer(key => key == "Site.ReportTitle" ? "Localized report title" : null));
        var fallbackReport = GetReportMeta<ReportWithTextKey>();

        Assert.Equal("Localized report title", report.Title);
        Assert.Equal("Report title fallback", fallbackReport.Title);
    }

    [Fact]
    public void Report_Title_Does_Not_Infer_DisplayText_Key_Looking_Value()
    {
        var report = GetReportMeta<ReportWithKeyLikeLiteralTitle>();

        Assert.Equal("Site.LiteralReportTitle", report.Title);
    }

    [Fact]
    public void Report_Title_Localizes_Key_Looking_DisplayName()
    {
        var report = new ReportRegistry.Report(typeof(ReportWithLegacyKeyLikeTitle),
            new MockTextLocalizer(key => key == "Site.LegacyReportTitle" ?
                "Localized legacy report title" : null));

        Assert.Equal("Localized legacy report title", report.Title);
    }

    [Fact]
    public void Report_Without_DisplayName_Or_Permission_Has_Nulls()
    {
        var report = GetReportMeta<ReportThree>();

        Assert.Null(report.Permission);
        Assert.Null(report.Title);
        Assert.Equal("Cat2", report.Category.Key);
    }

    [Fact]
    public void Report_Without_Category_Uses_Empty_Category_Key()
    {
        var report = GetReportMeta<NoCategoryReport>();
        Assert.Equal(string.Empty, report.Category.Key);
        Assert.Equal(string.Empty, report.Category.Title);
    }

    [Fact]
    public void Report_Category_Uses_Explicit_TextKey()
    {
        var report = new ReportRegistry.Report(typeof(ReportWithCategoryTextKey),
            new MockTextLocalizer(key => key == "Site.ReportCategory" ? "Localized category" : null));

        Assert.Equal("Invariant category", report.Category.Key);
        Assert.Equal("Localized category", report.Category.Title);
    }

    [Fact]
    public void Report_Category_Does_Not_Infer_Key_Looking_CategoryName_Value()
    {
        var report = new ReportRegistry.Report(typeof(ReportWithKeyLikeLiteralCategory),
            new MockTextLocalizer(key => key == "Site.LiteralReportCategory" ? "Should not localize" : null));

        Assert.Equal("Site.LiteralReportCategory", report.Category.Key);
        Assert.Equal("Site.LiteralReportCategory", report.Category.Title);
    }

    [Fact]
    public void Report_Category_Continues_To_Infer_Legacy_Category_Key()
    {
        var report = new ReportRegistry.Report(typeof(ReportWithLegacyKeyLikeCategory),
            new MockTextLocalizer(key => key == "Report.Category.Site.LegacyReportCategory" ?
                "Localized legacy category" : null));

        Assert.Equal("Site.LegacyReportCategory", report.Category.Key);
        Assert.Equal("Localized legacy category", report.Category.Title);
    }

    [Fact]
    public void Report_Constructor_Throws_For_Null_Type()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ReportRegistry.Report(null, NullTextLocalizer.Instance));
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var typeSource = new MockTypeSource(typeof(ReportOne));
        var permissions = new MockPermissions(_ => true);

        Assert.Throws<ArgumentNullException>(() =>
            new ReportRegistry(null, permissions, NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            new ReportRegistry(typeSource, null, NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            new ReportRegistry(typeSource, permissions, null));
    }

    [Fact]
    public void HasAvailableReportsInCategory_Returns_True_For_Open_Report()
    {
        var registry = new ReportRegistry(new MockTypeSource(typeof(ReportOne)), new MockPermissions(_ => false), NullTextLocalizer.Instance);
        Assert.True(registry.HasAvailableReportsInCategory("Cat1"));
    }

    [Fact]
    public void HasAvailableReportsInCategory_Respects_Permission()
    {
        var denied = new ReportRegistry(new MockTypeSource(typeof(ReportTwo)), new MockPermissions(_ => false), NullTextLocalizer.Instance);
        Assert.False(denied.HasAvailableReportsInCategory("Cat1/Sub"));

        var granted = new ReportRegistry(new MockTypeSource(typeof(ReportTwo)), new MockPermissions(_ => true), NullTextLocalizer.Instance);
        Assert.True(granted.HasAvailableReportsInCategory("Cat1/Sub"));
    }

    [Fact]
    public void HasAvailableReportsInCategory_Returns_False_For_Unknown_Category()
    {
        var registry = new ReportRegistry(new MockTypeSource(typeof(ReportOne)), new MockPermissions(_ => true), NullTextLocalizer.Instance);
        Assert.False(registry.HasAvailableReportsInCategory("Unknown"));
    }

    [Fact]
    public void EnsureTypes_Skips_Types_Without_ReportAttribute()
    {
        var registry = new ReportRegistry(new FixedTypeSource(typeof(NotAReport), typeof(ReportOne)),
            new MockPermissions(_ => true), NullTextLocalizer.Instance);

        var reports = registry.GetAvailableReportsInCategory(null).ToList();
        Assert.Single(reports);
        Assert.Equal(typeof(ReportOne), reports[0].Type);
    }

    [Fact]
    public void GetAvailableReportsInCategory_Returns_All_Sorted_By_Title()
    {
        var registry = new ReportRegistry(
            new MockTypeSource(typeof(ReportTwo), typeof(ReportOne), typeof(ReportThree)),
            new MockPermissions(_ => true), NullTextLocalizer.Instance);

        var reports = registry.GetAvailableReportsInCategory(null).ToList();

        Assert.Equal(3, reports.Count);
        Assert.Equal(typeof(ReportThree), reports[0].Type);
        Assert.Equal(typeof(ReportOne), reports[1].Type);
        Assert.Equal(typeof(ReportTwo), reports[2].Type);
    }

    [Fact]
    public void GetAvailableReportsInCategory_Filters_By_Category()
    {
        var registry = new ReportRegistry(
            new MockTypeSource(typeof(ReportOne), typeof(ReportThree)),
            new MockPermissions(_ => true), NullTextLocalizer.Instance);

        var cat2 = registry.GetAvailableReportsInCategory("Cat2").ToList();
        Assert.Single(cat2);
        Assert.Equal(typeof(ReportThree), cat2[0].Type);
    }

    [Fact]
    public void GetAvailableReportsInCategory_Matches_Subcategories()
    {
        var registry = new ReportRegistry(
            new MockTypeSource(typeof(ReportOne), typeof(ReportTwo)),
            new MockPermissions(_ => true), NullTextLocalizer.Instance);

        var reports = registry.GetAvailableReportsInCategory("Cat1").ToList();
        Assert.Equal(2, reports.Count);
    }

    [Fact]
    public void GetAvailableReportsInCategory_Filters_By_Permission()
    {
        var registry = new ReportRegistry(
            new MockTypeSource(typeof(ReportOne), typeof(ReportTwo)),
            new MockPermissions(_ => false), NullTextLocalizer.Instance);

        var reports = registry.GetAvailableReportsInCategory(null).ToList();
        Assert.Single(reports);
        Assert.Equal(typeof(ReportOne), reports[0].Type);
    }

    [Fact]
    public void GetReport_Throws_For_Null_Or_Empty_Key()
    {
        var registry = new ReportRegistry(new MockTypeSource(), new MockPermissions(_ => true), NullTextLocalizer.Instance);
        Assert.Throws<ArgumentNullException>(() => registry.GetReport(null));
        Assert.Throws<ArgumentNullException>(() => registry.GetReport(""));
    }

    [Fact]
    public void GetReport_Returns_Null_For_Unknown_Key()
    {
        var registry = new ReportRegistry(new MockTypeSource(typeof(ReportOne)), new MockPermissions(_ => true), NullTextLocalizer.Instance);
        Assert.Null(registry.GetReport("Unknown"));
    }

    [Fact]
    public void GetReport_Returns_Report_By_Key()
    {
        var registry = new ReportRegistry(new MockTypeSource(typeof(ReportTwo)), new MockPermissions(_ => true), NullTextLocalizer.Instance);
        var report = registry.GetReport("Custom.Key");
        Assert.NotNull(report);
        Assert.Equal(typeof(ReportTwo), report.Type);
    }

    [Fact]
    public void GetReport_Validates_Permission_When_Requested()
    {
        var registry = new ReportRegistry(new MockTypeSource(typeof(ReportTwo)), new MockPermissions(_ => false), NullTextLocalizer.Instance);

        var ex = Assert.Throws<ValidationError>(() => registry.GetReport("Custom.Key"));
        Assert.Equal("AccessDenied", ex.ErrorCode);

        Assert.NotNull(registry.GetReport("Custom.Key", validatePermission: false));
    }

    private class CategoryTypeSource(params Attribute[] attributes) : ITypeSource
    {
        public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType) => attributes.Where(attributeType.IsInstanceOfType);
        public IEnumerable<Type> GetTypes() => [];
        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType) => [];
        public IEnumerable<Type> GetTypesWithInterface(Type interfaceType) => [];
    }

    [Fact]
    public void Report_Has_The_Description_Icon_And_Order_Of_Its_Attributes()
    {
        var report = GetReportMeta<ReportWithDescription>();
        Assert.Equal("What it shows", report.Description);
        Assert.Equal("fa fa-chart", report.IconClass);
        Assert.Equal(3, report.Order);

        var localized = new ReportRegistry.Report(typeof(ReportWithDescription),
            new MockTextLocalizer(key => key == "Site.ReportDescription" ? "Localized description" : null));
        Assert.Equal("Localized description", localized.Description);

        var plain = GetReportMeta<ReportOne>();
        Assert.Null(plain.Description);
        Assert.Null(plain.IconClass);
        Assert.Equal(0, plain.Order);
    }

    [Fact]
    public void GetCategory_Uses_The_ReportCategory_Attribute_Of_The_Category()
    {
        var typeSource = new CategoryTypeSource(
            new ReportCategoryAttribute("Northwind/Sales") { Description = "Revenue over time", IconClass = "fa fa-chart", Order = 2 },
            new ReportCategoryAttribute("Northwind/Stock") { Title = "Inventory" });
        var registry = new ReportRegistry(typeSource, new MockPermissions(_ => true), NullTextLocalizer.Instance);

        var sales = registry.GetCategory("northwind/sales");
        Assert.Equal("northwind/sales", sales.Key);
        Assert.Equal("sales", sales.Title);
        Assert.Equal("Revenue over time", sales.Description);
        Assert.Equal("fa fa-chart", sales.IconClass);
        Assert.Equal(2, sales.Order);

        Assert.Equal("Inventory", registry.GetCategory("Northwind/Stock").Title);

        var other = registry.GetCategory("Northwind/Other");
        Assert.Equal("Other", other.Title);
        Assert.Null(other.Description);
        Assert.Null(other.IconClass);
        Assert.Equal(0, other.Order);
    }

    [Fact]
    public void GetCategory_Prefers_The_Localized_Texts_Of_The_Category()
    {
        var typeSource = new CategoryTypeSource(
            new ReportCategoryAttribute("Northwind/Sales") { Title = "Sales", Description = "Revenue over time" });
        var registry = new ReportRegistry(typeSource, new MockPermissions(_ => true), new MockTextLocalizer(key => key switch
        {
            "Report.Category.Northwind.Sales" => "Verkauf",
            "Report.Category.Northwind.Sales.Description" => "Umsatz",
            _ => null
        }));

        var sales = registry.GetCategory("Northwind/Sales");
        Assert.Equal("Verkauf", sales.Title);
        Assert.Equal("Umsatz", sales.Description);
    }
}
