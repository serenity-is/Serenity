using Serenity.Data;
using Xunit;

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
            var actual = JSON.Parse<BaseCriteria>("[\"(null)\",[\"x\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.IsNull, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("x", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesIsNotNullCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"(!null)\",[\"b\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.IsNotNull, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("b", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesExistsCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"(exists)\",[\"some expression\"]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<UnaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.Exists, criteria.Operator);
            var inner = Assert.IsType<Criteria>(criteria.Operand);
            Assert.Equal("some expression", inner.Expression);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b")).ToJson();
            Assert.Equal("[\"&\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b") & new Criteria("c")).ToJson();
            Assert.Equal("[\"&\",[\"&\",[\"a\"],[\"b\"]],[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesOrCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.Equal("[\"|\",[\"c\"],[\"d\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleOrCriteriaProperly()
        {
            var actual = (new Criteria("a") | new Criteria("b") | new Criteria("c")).ToJson();
            Assert.Equal("[\"|\",[\"|\",[\"a\"],[\"b\"]],[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesXorCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.Equal("[\"|\",[\"c\"],[\"d\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleXorCriteriaProperly()
        {
            var actual = (new Criteria("a") ^ new Criteria("b") ^ new Criteria("c")).ToJson();
            Assert.Equal("[\"^\",[\"^\",[\"a\"],[\"b\"]],[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesEQCriteriaProperly()
        {
            var actual = (new Criteria("a") == new Criteria("b")).ToJson();
            Assert.Equal("[\"=\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNECriteriaProperly()
        {
            var actual = (new Criteria("a") != new Criteria("b")).ToJson();
            Assert.Equal("[\"!=\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesGTCriteriaProperly()
        {
            var actual = (new Criteria("a") > new Criteria("b")).ToJson();
            Assert.Equal("[\">\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesGECriteriaProperly()
        {
            var actual = (new Criteria("a") >= new Criteria("b")).ToJson();
            Assert.Equal("[\">=\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLTCriteriaProperly()
        {
            var actual = (new Criteria("a") < new Criteria("b")).ToJson();
            Assert.Equal("[\"<\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLECriteriaProperly()
        {
            var actual = (new Criteria("a") <= new Criteria("b")).ToJson();
            Assert.Equal("[\"<=\",[\"a\"],[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesINCriteriaProperly()
        {
            var actual = (new Criteria("a").In("b", "c", "d")).ToJson();
            Assert.Equal("[\"(in)\",[\"a\"],[[\"b\",\"c\",\"d\"]]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNotINCriteriaProperly()
        {
            var actual = (new Criteria("a").NotIn("b")).ToJson();
            Assert.Equal("[\"(!in)\",[\"a\"],[[\"b\"]]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").StartsWith("b")).ToJson();
            Assert.Equal("[\"~\",[\"a\"],\"b%\"]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNotLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").NotLike("%b")).ToJson();
            Assert.Equal("[\"!~\",[\"a\"],\"%b\"]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesValueCriteriaProperly()
        {
            var actual = (new Criteria("a") >= 5).ToJson();
            Assert.Equal("[\">=\",[\"a\"],5]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_DeserializesComplexCriteriaProperly()
        {
            var actual = JSON.Parse<BaseCriteria>("[\"|\",[\"()\",[\"&\",[\">=\",[\"a\"],5],[\"(!null)\",[\"c\"]]]],[\"!=\",[\"(in)\",[\"x\"],[[4,5,6]]],7]]");
            Assert.NotNull(actual);
            var criteria = Assert.IsType<BinaryCriteria>(actual);
            Assert.Equal(CriteriaOperator.OR, criteria.Operator);
            var left = Assert.IsType<UnaryCriteria>(criteria.LeftOperand);
            var right = Assert.IsType<BinaryCriteria>(criteria.RightOperand);
            /*var actual = (
                (~(new Criteria("a") >= 5 & new Criteria("c").IsNotNull())) |
                  (new Criteria("x").In(4, 5, 6)) != 7).ToJson();
            Assert.Equal("", actual);*/
        }
    }
}