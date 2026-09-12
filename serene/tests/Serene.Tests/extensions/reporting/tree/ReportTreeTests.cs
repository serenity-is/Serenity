using Serenity.Reporting;
namespace Serenity.Extensions;

public class ReportTreeTests
{
    [Category("Cat1")]
    private class ReportA : IReport
    {
        public object? GetData() => null;
    }

    [Category("Cat1/Sub")]
    private class ReportB : IReport
    {
        public object? GetData() => null;
    }

    [Category("Cat2")]
    private class ReportC : IReport
    {
        public object? GetData() => null;
    }

    [Category("Cat3")]
    private class ReportD : IReport
    {
        public object? GetData() => null;
    }

    private class ReportNoCategory : IReport
    {
        public object? GetData() => null;
    }

    private static ReportRegistry.Report Report<T>()
    {
        return new ReportRegistry.Report(typeof(T), NullTextLocalizer.Instance);
    }

    [Fact]
    public void Constructor_Creates_Root()
    {
        var tree = new ReportTree();
        Assert.NotNull(tree.Root);
        Assert.Empty(tree.Root.SubCategories);
        Assert.Empty(tree.Root.Reports);
    }

    [Fact]
    public void FromList_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ReportTree.FromList(null!, NullTextLocalizer.Instance));
    }

    [Fact]
    public void FromList_Adds_Reports_Without_Category_To_Root()
    {
        var tree = ReportTree.FromList([Report<ReportNoCategory>()], NullTextLocalizer.Instance);
        Assert.Single(tree.Root.Reports);
    }

    [Fact]
    public void FromList_Builds_Nested_Categories()
    {
        var tree = ReportTree.FromList(
        [
            Report<ReportA>(),
            Report<ReportB>(),
            Report<ReportC>()
        ], NullTextLocalizer.Instance);

        Assert.Equal(2, tree.Root.SubCategories.Count);
        var cat1 = tree.Root.SubCategories.Single(x => x.Key == "Cat1");
        Assert.Single(cat1.Reports);
        Assert.Single(cat1.SubCategories);
        Assert.Equal("Cat1/Sub", cat1.SubCategories[0].Key);
        Assert.Single(cat1.SubCategories[0].Reports);
    }

    [Fact]
    public void FromList_Adds_Multiple_Reports_To_Same_Category()
    {
        var tree = ReportTree.FromList(
        [
            Report<ReportA>(),
            Report<ReportA>()
        ], NullTextLocalizer.Instance);

        var cat = Assert.Single(tree.Root.SubCategories);
        Assert.Equal(2, cat.Reports.Count);
    }

    [Fact]
    public void FromList_Skips_Root_Path()
    {
        var tree = ReportTree.FromList([Report<ReportB>()], NullTextLocalizer.Instance,
            rootPath: "Cat1");

        var cat = Assert.Single(tree.Root.SubCategories);
        Assert.Equal("Cat1/Sub", cat.Key);
    }

    [Fact]
    public void FromList_Sorts_By_CategoryOrder_Then_Title()
    {
        var tree = ReportTree.FromList(
        [
            Report<ReportA>(),
            Report<ReportC>()
        ], NullTextLocalizer.Instance, categoryOrder: "Cat2;Cat1");

        Assert.Equal(["Cat2", "Cat1"], tree.Root.SubCategories.Select(x => x.Key));
    }

    [Fact]
    public void FromList_Sorts_Unordered_Categories_After_Ordered_Ones()
    {
        var tree = ReportTree.FromList(
        [
            Report<ReportA>(),
            Report<ReportC>(),
            Report<ReportD>()
        ], NullTextLocalizer.Instance, categoryOrder: "Cat1;Cat3");

        Assert.Equal(["Cat1", "Cat3", "Cat2"], tree.Root.SubCategories.Select(x => x.Key));
    }

    [Fact]
    public void Category_Properties_Are_Writable()
    {
        var category = new ReportTree.Category
        {
            Key = "k",
            Title = "t"
        };

        Assert.Equal("k", category.Key);
        Assert.Equal("t", category.Title);
        Assert.NotNull(category.SubCategories);
        Assert.NotNull(category.Reports);
    }
}

