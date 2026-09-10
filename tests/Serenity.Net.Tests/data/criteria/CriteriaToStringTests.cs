namespace Serenity.Data;

public class CriteriaToStringTests
{
    private sealed class UnimplementedToStringCriteria : BaseCriteria
    {
    }

    [Fact]
    public void Criteria_JoinNumberConstructor_NullField_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Criteria(7, (string)null));
    }

    [Fact]
    public void BaseCriteria_ToStringWithQuery_OnSubclassWithoutOverride_ThrowsNotImplementedException()
    {
        var criteria = new UnimplementedToStringCriteria();

        Assert.Throws<NotImplementedException>(() => criteria.ToString(new SqlQuery()));
    }

    [Fact]
    public void BaseCriteria_ToStringStringBuilder_OnSubclassWithoutOverride_ThrowsNotImplementedException()
    {
        var criteria = new UnimplementedToStringCriteria();

        Assert.Throws<NotImplementedException>(() =>
            criteria.ToString(new StringBuilder(), new SqlQuery()));
    }

    [Fact]
    public void ToString_NoParams_CriteriaExpressionWithoutParams_Works()
    {
        Assert.Equal("(A AND B)", (new Criteria("A") & new Criteria("B")).ToString() ?? "");
    }

    [Fact]
    public void ToString_ParamCriteria_ThrowsInvalidOperationException()
    {
        var criteria = new Criteria("A") == new ValueCriteria("ParamValue");

        Assert.Throws<InvalidOperationException>(() => criteria.ToString());
    }

    [Fact]
    public void ToStringIgnoreParams_ParamCriteria_DoesNotThrow()
    {
        var criteria = new Criteria("A") == new ValueCriteria("ParamValue");

        var result = criteria.ToStringIgnoreParams();

        Assert.StartsWith("(", result);
        Assert.EndsWith(")", result);
        Assert.Contains("@p", result);
    }

    [Fact]
    public void ToStringIgnoreParams_EnumCriteria_Works()
    {
        var days = DayOfWeek.Monday.ToString();

        Assert.NotEmpty(days);
    }
}
