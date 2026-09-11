namespace Serenity.Web;

public class DistinctValuesScriptTests
{
    private static MockSqlConnections Connections()
    {
        return new MockSqlConnections
        {
            OnNewByKey = _ => new MockDbConnection()
                .InterceptExecuteReader(_ => new MockDbDataReader())
        };
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => new DistinctValuesScript<IdNameRow>(null!, "Name"));
        Assert.Throws<ArgumentNullException>(() => new DistinctValuesScript<IdNameRow>(Connections(), null!));
    }

    [Fact]
    public void Constructor_Throws_When_Property_Not_Found()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new DistinctValuesScript<IdNameRow>(Connections(), "NotAField"));
    }

    [Fact]
    public void Constructor_Sets_Fields()
    {
        var script = new DistinctValuesScript<IdNameRow>(Connections(), "Name");

        Assert.Equal("v", script.IdField);
        Assert.Equal("v", script.TextField);
    }

    [Fact]
    public void GetScript_Produces_Distinct_Lookup()
    {
        var script = new DistinctValuesScript<IdNameRow>(Connections(), "Name")
        {
            LookupKey = "Distinct.Test"
        };

        var result = script.GetScript();

        Assert.Contains("Lookup.Distinct.Test", result);
        Assert.Contains(".map", result);
    }

    [Fact]
    public void GetScriptData_Returns_Data()
    {
        var script = new DistinctValuesScript<IdNameRow>(Connections(), "Name");
        Assert.NotNull(script.GetScriptData());
    }
}
