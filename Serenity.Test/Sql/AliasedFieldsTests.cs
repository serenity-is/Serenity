using Serenity.Data;
using System;
using Xunit;

namespace Serenity.Test.Data
{
    using BasicRow = RowMappingTests.BasicRow;
    using ComplexRow = RowMappingTests.ComplexRow;

    public partial class AliasedFieldsTests
    {
        [Fact]
        public void ThrowsExceptionIfNullOrWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BasicRow.Fields.As(null));

            Assert.Throws<ArgumentException>(() =>
                BasicRow.Fields.As(""));

            Assert.Throws<ArgumentException>(() =>
                BasicRow.Fields.As("  "));
        }

        [Fact]
        public void ThrowsArgumentExceptionForAlreadyAliasedFields()
        {
            var x = BasicRow.Fields.As("x");

            Assert.Throws<ArgumentException>(() =>
                x.As("y"));
        }

        [Fact]
        public void ReplacesFieldsAliasName()
        {
            var x = BasicRow.Fields.As("x");

            Assert.Equal("x", x.AliasName);
        }

        [Fact]
        public void ReplacesTableFieldExpressions()
        {
            var x = BasicRow.Fields.As("x");

            Assert.Equal("x.AInt32", x.AInt32.Expression);
            Assert.Equal("x.AString", x.AString.Expression);

            var y = ComplexRow.Fields.As("y");

            Assert.Equal("y.ComplexID", y.ID.Expression);
            Assert.Equal("y.Name", y.Name.Expression);
            Assert.Equal("y.OverridenExpression", y.Overriden.Expression);
            Assert.Equal("y.CountryID", y.CountryID.Expression);
            Assert.Equal("(y.Name + ' ' + y.Surname)", y.FullName.Expression);
        }

        [Fact]
        public void ReplacesViewFieldExpressions()
        {
            var x = ComplexRow.Fields.As("x");
            Assert.Equal("x_c.Name", x.CountryName.Expression);

            var y = ComplexRow.Fields.As("y");
            Assert.Equal("y_c.Name", y.CountryName.Expression);
        }

        [Fact]
        public void CanAliasSameFieldsWithDifferentNamesTwice()
        {
            var x = ComplexRow.Fields.As("x");
            Assert.Equal("x.ComplexID", x.ID.Expression);

            var y = ComplexRow.Fields.As("y");
            Assert.Equal("y.ComplexID", y.ID.Expression);
        }

        [Fact]
        public void CanAliasSameFieldsWithSameNameTwice()
        {
            var x = ComplexRow.Fields.As("x");
            Assert.Equal("x_c.Name", x.CountryName.Expression);

            var x2 = ComplexRow.Fields.As("x");
            Assert.Equal("x_c.Name", x2.CountryName.Expression);
        }
    }
}
