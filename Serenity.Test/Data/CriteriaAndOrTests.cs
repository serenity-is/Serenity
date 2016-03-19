using Serenity.Data;
using Xunit;

namespace Serenity.Test.Data
{
    public class CriteriaAndOrTests
    {
        [Fact]
        public void BaseCriteria_And_WithTwoCriteria_GeneratesBinaryCriteria()
        {
            var a = new Criteria("x = 1");
            var b = new Criteria("y = 2");

            var c = a & b;
            var actual = Assert.IsType<BinaryCriteria>(c);
            Assert.Equal(CriteriaOperator.AND, actual.Operator);
            Assert.Equal(a, actual.LeftOperand);
            Assert.Equal(b, actual.RightOperand);
        }


        [Fact]
        public void BaseCriteria_And_WithEmptyLeftCriteria_ReturnsRightCriteria()
        {
            var a = Criteria.Empty;
            var b = new Criteria("y = 2");

            var c = a & b;
            Assert.Equal(b, c);
        }

        [Fact]
        public void BaseCriteria_And_WithEmptyRightCriteria_ReturnsLeftCriteria()
        {
            var a = new Criteria("x == 1");
            var b = Criteria.Empty;

            var c = a & b;
            Assert.Equal(a, c);
        }

        [Fact]
        public void BaseCriteria_And_WithNullLeftCriteria_ReturnsRightCriteria()
        {
            Criteria a = null;
            var b = new Criteria("y = 2");

            var c = a & b;
            Assert.Equal(b, c);
        }

        [Fact]
        public void BaseCriteria_And_WithNullRightCriteria_ReturnsLeftCriteria()
        {
            var a = new Criteria("x == 1");
            Criteria b = null;

            var c = a & b;
            Assert.Equal(a, c);
        }

        [Fact]
        public void BaseCriteria_Or_WithTwoCriteria_GeneratesBinaryCriteria()
        {
            var a = new Criteria("x = 1");
            var b = new Criteria("y = 2");

            var c = a | b;
            var actual = Assert.IsType<BinaryCriteria>(c);
            Assert.Equal(CriteriaOperator.OR, actual.Operator);
            Assert.Equal(a, actual.LeftOperand);
            Assert.Equal(b, actual.RightOperand);
        }

        [Fact]
        public void BaseCriteria_Or_WithEmptyLeftCriteria_ReturnsRightCriteria()
        {
            var a = Criteria.Empty;
            var b = new Criteria("y = 2");

            var c = a | b;
            Assert.Equal(b, c);
        }

        [Fact]
        public void BaseCriteria_Or_WithEmptyRightCriteria_ReturnsLeftCriteria()
        {
            var a = new Criteria("x == 1");
            var b = Criteria.Empty;

            var c = a | b;
            Assert.Equal(a, c);
        }

        [Fact]
        public void BaseCriteria_Or_WithNullLeftCriteria_ReturnsRightCriteria()
        {
            Criteria a = null;
            var b = new Criteria("y = 2");

            var c = a | b;
            Assert.Equal(b, c);
        }

        [Fact]
        public void BaseCriteria_Or_WithNullRightCriteria_ReturnsLeftCriteria()
        {
            var a = new Criteria("x == 1");
            Criteria b = null;

            var c = a | b;
            Assert.Equal(a, c);
        }

    }
}
