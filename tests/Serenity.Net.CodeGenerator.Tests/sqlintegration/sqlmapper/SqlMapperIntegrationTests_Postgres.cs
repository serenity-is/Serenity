namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_Postgres
{
    [Fact]
    public void Query_SqlQuery_Postgres_BindsSerenityParameters()
    {
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("Postgres", PostgresDialect.Instance);
    }
}
