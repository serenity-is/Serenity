namespace Serenity.Data;

public partial class CriteriaFieldExpressionReplacerTests
{
    [Fact]
    public void ShouldUse_AutoQuoted_FieldName_ForUpper()
    {
        // PostgresDialect.IsLikeCaseSensitive is true (LIKE is case-sensitive,
        // ILIKE is the insensitive variant), so LIKE gets UPPER() on both sides.
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), new NullPermissions(), dialect: PostgresDialect.Instance);
        var criteria = new Criteria(nameof(TestRow.Name)).Contains("a");
        var result = replacer.Process(criteria);
        // ToStringIgnoreParams numbering restarts at @p0 on every render.
        Assert.Equal("(UPPER(T0.[Name]) LIKE UPPER(@p0))", result.ToStringIgnoreParams());
    }
}