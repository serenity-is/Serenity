namespace Serenity.Data;

public partial class CriteriaFieldExpressionReplacerTests
{
    private class MyDialect : PostgresDialect
    {
        public override bool IsLikeCaseSensitive => true;
    }

    [Fact]
    public void ShouldUse_AutoQuoted_FieldName_ForUpper()
    {
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), new NullPermissions(), dialect: new MyDialect());
        var criteria = new Criteria(nameof(TestRow.Name)).Contains("a");
        var result = replacer.Process(criteria);
        Assert.Equal("(UPPER(T0.[Name]) LIKE UPPER(@p0))", result.ToStringIgnoreParams());
    }
}