using System.Collections;

namespace Serenity.Data;

public partial class CriteriaFieldExpressionReplacerTests
{
    private static CriteriaFieldExpressionReplacer CreateValueReplacer()
    {
        return new CriteriaFieldExpressionReplacer(new TestRow(), new NullPermissions());
    }

    [Fact]
    public void Converts_String_Value_For_IntField()
    {
        var result = CreateValueReplacer().Process(new Criteria("Id") == "5");

        var binary = Assert.IsType<BinaryCriteria>(result);
        var value = Assert.IsType<ValueCriteria>(binary.RightOperand);
        Assert.Equal(5, value.Value);
    }

    [Fact]
    public void Converts_Existing_Value_For_IntField()
    {
        var result = CreateValueReplacer().Process(new Criteria("Id") == 7);

        var binary = Assert.IsType<BinaryCriteria>(result);
        var value = Assert.IsType<ValueCriteria>(binary.RightOperand);
        Assert.Equal(7, value.Value);
    }

    [Fact]
    public void Converts_Enumerable_Values_For_IntField()
    {
        var criteria = new BinaryCriteria(new Criteria("Id"), CriteriaOperator.EQ,
            new ValueCriteria(new object[] { "1", "2" }));

        var result = CreateValueReplacer().Process(criteria);

        var binary = Assert.IsType<BinaryCriteria>(result);
        var value = Assert.IsType<ValueCriteria>(binary.RightOperand);
        var values = Assert.IsAssignableFrom<IEnumerable>(value.Value);
        Assert.Equal([1, 2], values.Cast<int>().ToList());
    }

    [Fact]
    public void Does_Not_Convert_Empty_String_Value()
    {
        var result = CreateValueReplacer().Process(new Criteria("Id") == "");

        Assert.NotNull(result);
    }

    [Fact]
    public void Does_Not_Convert_StringField()
    {
        var result = CreateValueReplacer().Process(new Criteria("Name") == "abc");

        Assert.NotNull(result);
    }

    [Fact]
    public void Does_Not_Convert_When_Right_Operand_Is_Not_A_Value()
    {
        var criteria = new BinaryCriteria(new Criteria("Id"), CriteriaOperator.EQ,
            new Criteria("Name"));

        var result = CreateValueReplacer().Process(criteria);

        Assert.NotNull(result);
    }

    [Fact]
    public void Swallows_Conversion_Errors()
    {
        var result = CreateValueReplacer().Process(new Criteria("Id") == "not-a-number");

        Assert.NotNull(result);
    }
}
