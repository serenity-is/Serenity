namespace Serenity.Data;

[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_SqlServer
{
    [Fact]
    public void Query_SqlQuery_SqlServer_BindsSerenityParameters()
    {
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("SqlServer", SqlServer2012Dialect.Instance);
    }
}
