using Serenity.Tests.Entities;

namespace Serenity.Tests.Entity;

public class RowCreationTests
{
    [Fact]
    public void Raises_Exception_For_Unitialized_Fields()
    {
        var fields = new ComplexRow.RowFields();
        Assert.Throws<ArgumentOutOfRangeException>(() => new ComplexRow(fields));
    }

    [Fact]
    public void Can_Create_Row_With_Initialized_Fields()
    {
        var fields = new ComplexRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance);
        new ComplexRow(fields)
        {
            BasicExpression = "test"
        }.ToString();
    }

    [Fact]
    public void Can_Create_Row_With_Initialized_Fields_With_DefaultScope()
    {
        RowFieldsProvider.SetLocal(FallbackRowFieldsProvider.Instance);
        var fields = new ComplexRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance);
        new ComplexRow(fields)
        {
            BasicExpression = "test"
        }.ToString();
    }

    [Fact]
    public void Can_Create_Two_Rows_With_Different_Field_Names()
    {
        var fields1 = new ComplexRow.RowFields();
        fields1.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance);
        fields1.BasicExpression.Expression = "Test1";
        var row1 = new ComplexRow(fields1);
        var fields2 = new ComplexRow.RowFields();
        fields2.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance);
        fields2.BasicExpression.Expression = "Test2";
        var row2 = new ComplexRow(fields2);
        Assert.Equal("Test1", row1.GetFields().BasicExpression.Expression);
        Assert.Equal("Test2", row2.GetFields().BasicExpression.Expression);
    }
}
