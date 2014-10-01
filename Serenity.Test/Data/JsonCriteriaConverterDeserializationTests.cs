using Serenity.Data;
using System.Collections;
using Xunit;
using Xunit.Extensions;

namespace Serenity.Test.Data
{
    public class JsonCriteriaConverterDeserializationTests
    {
        [Fact]
        public void JsonCriteriaConverter_DeserializesStringCriteriaAsIs()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"a\"]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<Criteria>(actual);
            Assert.Equal("a", criteria.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesParamCriteriaAsIs()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"@a\"]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<ParamCriteria>(actual);
            Assert.Equal("@a", criteria.Name);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesParenCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"()\",[\"a\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.Paren, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("a", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesIsNullCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"is null\",[\"x\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.IsNull, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("x", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesIsNotNullCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"is not null\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.IsNotNull, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("b", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesExistsCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"exists\",[\"some expression\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.Exists, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("some expression", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesAndCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"and\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.AND, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("b", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesMultipleAndCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[[\"a\"],\"and\",[\"b\"]],\"and\",[\"c\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.AND, criteria.Operator);
            var left = Assert.IsType<BinaryCriteria>(criteria.LeftOperand);
            Assert.Equal(CriteriaOperator.AND, left.Operator);
            var leftLeft = Assert.IsType<Criteria>(left.LeftOperand);
            Assert.Equal("a", leftLeft.Expression);
            var leftRight = Assert.IsType<Criteria>(left.RightOperand);
            Assert.Equal("b", leftRight.Expression);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("c", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesOrCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"or\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.OR, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("b", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesMultipleOrCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[[\"a\"],\"or\",[\"b\"]],\"or\",[\"c\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.OR, criteria.Operator);
            var left = Assert.IsType<BinaryCriteria>(criteria.LeftOperand);
            Assert.Equal(CriteriaOperator.OR, left.Operator);
            var leftLeft = Assert.IsType<Criteria>(left.LeftOperand);
            Assert.Equal("a", leftLeft.Expression);
            var leftRight = Assert.IsType<Criteria>(left.RightOperand);
            Assert.Equal("b", leftRight.Expression);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("c", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesXorCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"xor\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.XOR, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("b", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesMultipleXorCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[[\"a\"],\"xor\",[\"b\"]],\"xor\",[\"c\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.XOR, criteria.Operator);
            var left = Assert.IsType<BinaryCriteria>(criteria.LeftOperand);
            Assert.Equal(CriteriaOperator.XOR, left.Operator);
            var leftLeft = Assert.IsType<Criteria>(left.LeftOperand);
            Assert.Equal("a", leftLeft.Expression);
            var leftRight = Assert.IsType<Criteria>(left.RightOperand);
            Assert.Equal("b", leftRight.Expression);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("c", right.Expression);
        }

        [Theory]
        [InlineData("=", CriteriaOperator.EQ)]
        [InlineData("!=", CriteriaOperator.NE)]
        [InlineData(">", CriteriaOperator.GT)]
        [InlineData(">=", CriteriaOperator.GE)]
        [InlineData("<", CriteriaOperator.LT)]
        [InlineData("<=", CriteriaOperator.LE)]
        public void JsonCriteriaConverter_DeserializesComparisonCriteriasProperly(string opStr,
            CriteriaOperator op)
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"" + opStr + "\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(op, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<Criteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("b", right.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesINCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"in\",[[\"b\",\"c\",\"d\"]]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.In, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.IsType<object[]>(right.Value);
            Assert.Equal(new object[] { "b", "c", "d" }, (IEnumerable)right.Value);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesNotINCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"not in\",[[\"b\"]]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.NotIn, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.IsType<object[]>(right.Value);
            Assert.Equal(new object[] { "b" }, (IEnumerable)right.Value);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesLikeCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"like\",\"b%\"]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.Like, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("b%", right.Value);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesNotLikeCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\"not like\",\"%b\"]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.NotLike, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal("%b", right.Value);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesValueCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"a\"],\">=\",5]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.GE, criteria.Operator);
            var left = Assert.IsType<Criteria>(criteria.LeftOperand);
            var right = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal("a", left.Expression);
            Assert.Equal((long)5, right.Value);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesComplexCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[[\"()\",[[[\"a\"],\">=\",5],\"and\",[\"is not null\",[\"c\"]]]],\"or\",[[[\"x\"],\"in\",[[4,5,6]]],\"!=\",7]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.OR, criteria.Operator);
            var left = Assert.IsType<UnaryCriteria>(criteria.LeftOperand);
            Assert.Equal(CriteriaOperator.Paren, left.Operator);
            var leftOperand = Assert.IsType<BinaryCriteria>(left.Operand);
            Assert.Equal(CriteriaOperator.AND, leftOperand.Operator);
            var leftLeft = Assert.IsType<BinaryCriteria>(leftOperand.LeftOperand);
            Assert.Equal(CriteriaOperator.GE, leftLeft.Operator);
            var leftLeftOperand = Assert.IsType<Criteria>(leftLeft.LeftOperand);
            Assert.Equal("a", leftLeftOperand.Expression);
            var leftRight = Assert.IsType<UnaryCriteria>(leftOperand.RightOperand);
            Assert.Equal(CriteriaOperator.IsNotNull, leftRight.Operator);
            var leftRightOperand = Assert.IsType<Criteria>(leftRight.Operand);
            Assert.Equal("c", leftRightOperand.Expression);

            var right = Assert.IsType<BinaryCriteria>(criteria.RightOperand);
            Assert.Equal(CriteriaOperator.NE, right.Operator);
            var rightLeft = Assert.IsType<BinaryCriteria>(right.LeftOperand);
            Assert.Equal(CriteriaOperator.In, rightLeft.Operator);
            var x = Assert.IsType<Criteria>(rightLeft.LeftOperand);
            Assert.Equal("x", x.Expression);
            var v = Assert.IsType<ValueCriteria>(rightLeft.RightOperand);
            Assert.IsType<object[]>(v.Value);
            Assert.Equal(new object[] { 4L, 5L, 6L }, (IEnumerable)v.Value);
            var rightRight = Assert.IsType<ValueCriteria>(right.RightOperand);
            Assert.Equal((long)7, rightRight.Value);
        }
    }
}