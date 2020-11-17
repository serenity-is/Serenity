using Serenity.Data;
using System;
using Xunit;

namespace Serenity.Net.Data.Entity.Tests
{
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
            var fields = new ComplexRow.RowFields().Init(annotations: null);
            var row = new ComplexRow(fields);
            row.BasicExpression = "test";
        }

        [Fact]
        public void Can_Create_Row_With_Initialized_Fields_With_DefaultScope()
        {
            RowFieldsProvider.TestScope(() =>
            {
                var fields = new ComplexRow.RowFields().Init(annotations: null);
                var row = new ComplexRow(fields);
                row.BasicExpression = "test";
            }, new DefaultRowFieldsProvider());
        }

        [Fact]
        public void Can_Create_Two_Rows_With_Different_Field_Names()
        {
            var fields1 = new ComplexRow.RowFields().Init(annotations: null);
            fields1.BasicExpression.Expression = "Test1";
            var row1 = new ComplexRow(fields1);
            var fields2 = new ComplexRow.RowFields().Init(annotations: null);
            fields2.BasicExpression.Expression = "Test2";
            var row2 = new ComplexRow(fields2);
            Assert.Equal("Test1", row1.Fields.BasicExpression.Expression);
            Assert.Equal("Test2", row2.Fields.BasicExpression.Expression);
        }
    }
}
