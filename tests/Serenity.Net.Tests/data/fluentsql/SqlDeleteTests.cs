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
    public void Explicit_Where_Works()
    {
        var delete = new SqlDelete("T");
        var criteria = new Criteria("A = 1");
        ((IFilterableQuery)delete).Where(criteria);
        Assert.Same(criteria, Assert.Single(((IFilterableQuery)delete).GetWhereCriteria()));
        Assert.Contains("A = 1", delete.ToString());
    }

    [Fact]
    public void Where_StringExtension_CreatesCriteria()
    {
        var delete = new SqlDelete("T").Where("A = 1");

        Assert.IsType<Criteria>(Assert.Single(((IFilterableQuery)delete).GetWhereCriteria()));
    }

    [Fact]
    public void Where_Removes_T0_Reference()
    {
        var delete = new SqlDelete("T").Where("T0.A = 1");
        Assert.Equal("A = 1", ((IFilterableQuery)delete).GetWhereClause());
        Assert.Contains("A = 1", delete.ToString());
        Assert.DoesNotContain("T0.", delete.ToString());
    }

    [Fact]
    public void Where_Removes_BracketQuoted_T0_Reference()
    {
        var delete = new SqlDelete("T").Where("[T0].[A] = 1");

        Assert.Equal("[A] = 1", ((IFilterableQuery)delete).GetWhereClause());
        Assert.Contains("WHERE [A] = 1", delete.ToString());
        Assert.DoesNotContain("[T0].", delete.ToString());
    }

    [Fact]
    public void Where_Renders_Parameters_Once()
    {
        var delete = new SqlDelete("T").Where(new Criteria("A") == 5);
        Assert.Equal(1, delete.ParamCount);

        var where = ((IFilterableQuery)delete).GetWhereClause();

        Assert.Equal(1, delete.ParamCount);
        Assert.Equal(where, ((IFilterableQuery)delete).GetWhereClause());
        Assert.Contains(where, delete.ToString());
        Assert.Equal(1, delete.ParamCount);
    }

    [Fact]
    public void ToString_Caches_And_Invalidates_When_Where_Changes()
    {
        var delete = new SqlDelete("T");
        var initial = delete.ToString();

        Assert.Same(initial, delete.ToString());

        delete.Where(new Criteria("A") == 5);
        var withWhere = delete.ToString();

        Assert.NotSame(initial, withWhere);
        Assert.Contains("WHERE (A =", withWhere);
        Assert.Same(withWhere, delete.ToString());

        var parameterName = Assert.Single(delete.Params!).Key;
        delete.SetParam(parameterName, 6);

        Assert.Same(withWhere, delete.ToString());
    }

    [Fact]
    public void ToString_And_Format_Work()
    {
        Assert.Equal("DELETE FROM [T]", new SqlDelete("T").ToString());
        Assert.Equal("DELETE FROM [T]", SqlDelete.Format("T", ""));
        Assert.Equal("DELETE FROM [T]", SqlDelete.Format("T", null));
        Assert.Equal("DELETE FROM [T] WHERE A = 1", SqlDelete.Format("T", "A = 1"));
        Assert.Throws<ArgumentNullException>(() => SqlDelete.Format(null!, ""));
        Assert.Throws<ArgumentException>(() => SqlDelete.Format("", ""));
    }

    [Fact]
    public void Freeze_Prevents_Modification()
    {
        var delete = new SqlDelete("T").Where(new Criteria("A") == 5);
        delete.Freeze();

        Assert.Throws<InvalidOperationException>(() => delete.Where(new Criteria("B") == 6));
    }

    [Fact]
    public void Clone_Copies_Where_Params_And_Text()
    {
        var delete = new SqlDelete("T").Where(new Criteria("A") == 5);

        var clone = delete.Clone();

        Assert.Equal(delete.ToString(), clone.ToString());
        Assert.Equal(delete.ParamCount, clone.ParamCount);

        clone.Where(new Criteria("B") == 6);

        Assert.NotEqual(delete.ToString(), clone.ToString());
        Assert.Equal(delete.ParamCount + 1, clone.ParamCount);
    }
}
