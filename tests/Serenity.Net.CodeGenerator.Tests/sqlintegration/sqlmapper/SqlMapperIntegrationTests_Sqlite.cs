
namespace Serenity.Data;

public class SqlMapperIntegrationTests_Sqlite
{
    [Fact]
    public void Query_SqlQuery_Sqlite_BindsSerenityParameters()
    {
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("Sqlite", SqliteDialect.Instance);
    }
}
