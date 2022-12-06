namespace Serenity.Tests.Data;

public class SqlQuery_Join_CrossApply_Test
{
    [Fact]
    public void JoinWithCrossApplyWorks()
    {
        var subqueryAlias = new Alias("subquery");
        var query = new SqlQuery()
            .From("TestTable1")
            .CrossApply("OPENJSON(TestColumnJson) WITH(Column int '$')", subqueryAlias)
            .Select("TestColumn")
            .Select("subquery.Column");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn, subquery.Column FROM TestTable1 CROSS APPLY OPENJSON(TestColumnJson) WITH(Column int '$') subquery"),
            Normalize.Sql(
                query.ToString()));
    }

   
}