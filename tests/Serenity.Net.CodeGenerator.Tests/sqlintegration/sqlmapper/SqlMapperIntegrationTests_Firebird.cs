namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_Firebird
{
    [Fact]
    public void Query_SqlQuery_Firebird_BindsSerenityParameters()
    {
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("Firebird", FirebirdDialect.Instance, "RDB$DATABASE");
    }
}
