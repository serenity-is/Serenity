using Serenity.Data;
using Xunit;
using System.Linq;
using System;

namespace Serenity.Test.Data
{
    public class CriteriaEnumTests
    {
        private enum MyEnum : int
        {
            Value1 = 1,
            Value2 = 2
        }

        [Fact]
        public void BaseCriteria_EqualOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left == MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.EQ, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }

        [Fact]
        public void BaseCriteria_NotEqualOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left != MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.NE, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }

        [Fact]
        public void BaseCriteria_GreaterThanOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left > MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.GT, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }

        [Fact]
        public void BaseCriteria_GreaterThanOrEqualOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left >= MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.GE, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }

        [Fact]
        public void BaseCriteria_LessThanOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left < MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.LT, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }

        [Fact]
        public void BaseCriteria_LessThanOrEqualOperatorOverload_WorksWithEnums()
        {
            var left = new Criteria("x");
            var actual = left <= MyEnum.Value1;

            Assert.NotNull(actual);
            Assert.IsType<BinaryCriteria>(actual);

            var binary = actual as BinaryCriteria;

            Assert.Equal(CriteriaOperator.LE, binary.Operator);
            Assert.Equal(binary.LeftOperand, left);
            Assert.IsType<ValueCriteria>(binary.RightOperand);

            var value = binary.RightOperand as ValueCriteria;

            Assert.IsType<MyEnum>(value.Value);
            Assert.Equal(MyEnum.Value1, value.Value);
        }
    }
}
