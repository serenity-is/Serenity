namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_MySql
{
    [Fact]
    public void Query_SqlQuery_MySql_BindsSerenityParameters()
    {
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("MySql", MySqlDialect.Instance);
    }
}
