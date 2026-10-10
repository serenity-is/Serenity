namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_Oracle
{
    [Fact]
    public void Query_SqlQuery_Oracle_TranslatesAndBindsSerenityParameterNames()
    {
        if (SqlIntegrationConnections.ShouldSkip("Oracle"))
            return;

        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("Oracle", OracleDialect.Instance, "DUAL");
    }
}
