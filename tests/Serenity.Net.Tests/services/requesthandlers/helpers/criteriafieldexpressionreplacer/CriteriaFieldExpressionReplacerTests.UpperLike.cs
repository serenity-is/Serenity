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
        // ToStringIgnoreParams uses a static auto-param counter, so the exact
        // @pN number depends on how many calls were made before in the process.
        Assert.Matches(@"^\(UPPER\(T0\.\[Name\]\) LIKE UPPER\(@p\d+\)\)$", result.ToStringIgnoreParams());
    }
}