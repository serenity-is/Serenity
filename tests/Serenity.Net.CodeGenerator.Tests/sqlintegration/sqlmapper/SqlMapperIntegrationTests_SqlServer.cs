namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_SqlServer
{
    [Fact]
    public void Query_SqlQuery_SqlServer_BindsSerenityParameters()
    {
        if (SqlIntegrationConnections.ShouldSkip("SqlServer"))
            return;

        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("SqlServer", SqlServer2012Dialect.Instance);
    }
}
