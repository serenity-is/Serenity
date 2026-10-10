namespace Serenity.Data;

[Collection(SqlIntegrationCollections.Firebird)]
[Trait("tag", "sqldb")]
public class SqlMapperIntegrationTests_Firebird
{
    [Fact]
    public void Query_SqlQuery_Firebird_BindsSerenityParameters()
    {
        if (SqlIntegrationConnections.ShouldSkip("Firebird"))
            return;
            
        SqlMapperIntegrationTestHelper.AssertParameterizedQueryWorks("Firebird", FirebirdDialect.Instance, "RDB$DATABASE");
    }
}
