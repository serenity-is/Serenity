namespace Serenity.Data;

public class SqlDeleteTests
{
    [Fact]
    public void Constructor_Throws_For_Null_Table()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlDelete(null!));
    }

    [Fact]
    public void Where_Adds_Conditions_And_Validates()
    {
        var delete = new SqlDelete("T").Where("A = 1").Where("B = 2");

        Assert.Contains("WHERE", delete.ToString());
        Assert.Contains("A = 1", delete.ToString());
        Assert.Contains("B = 2", delete.ToString());

        Assert.Throws<ArgumentNullException>(() => new SqlDelete("T").Where((string)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlDelete("T").Where(""));
    }

    [Fact]
    public void Where_Params_Adds_Conditions_And_Validates()
    {
        var delete = new SqlDelete("T").Where("A = 1", "B = 2");
        Assert.Contains("A = 1", delete.ToString());

        Assert.Throws<ArgumentNullException>(() => new SqlDelete("T").Where((string[])null!));
        Assert.Throws<ArgumentNullException>(() => new SqlDelete("T").Where(Array.Empty<string>()));
    }

    [Fact]
    public void Explicit_Where_Works()
    {
        var delete = new SqlDelete("T");
        ((IFilterableQuery)delete).Where("A = 1");
        Assert.Contains("A = 1", delete.ToString());
    }

    [Fact]
    public void Where_Removes_T0_Reference()
    {
        var delete = new SqlDelete("T").Where("T0.A = 1");
        Assert.Contains("A = 1", delete.ToString());
        Assert.DoesNotContain("T0.", delete.ToString());
    }

    [Fact]
    public void ToString_And_Format_Work()
    {
        Assert.Equal("DELETE FROM [T]", new SqlDelete("T").ToString());
        Assert.Equal("DELETE FROM [T]", SqlDelete.Format("T", ""));
        Assert.Equal("DELETE FROM [T] WHERE A = 1", SqlDelete.Format("T", "A = 1"));
        Assert.Throws<ArgumentNullException>(() => SqlDelete.Format(null!, ""));
        Assert.Throws<ArgumentNullException>(() => SqlDelete.Format("", ""));
    }
}
