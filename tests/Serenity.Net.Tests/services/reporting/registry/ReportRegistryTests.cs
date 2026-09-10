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
}
