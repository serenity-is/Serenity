using Serenity.Data;
using Xunit;

namespace Serenity.Test.Data
{
    public class JsonCriteriaConverterSerializationTests
    {
        [Fact]
        public void JsonCriteriaConverter_SerializesStringCriteriaAsIs()
        {
            var actual = (new Criteria("a")).ToJson();
            Assert.Equal("[\"a\"]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesParenCriteriaProperly()
        {
            var actual = (~(new Criteria("a"))).ToJson();
            Assert.Equal("[\"()\",[\"a\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesIsNullCriteriaProperly()
        {
            var actual = (new Criteria("x")).IsNull().ToJson();
            Assert.Equal("[\"(null)\",[\"x\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesIsNotNullCriteriaProperly()
        {
            var actual = (new Criteria("b")).IsNotNull().ToJson();
            Assert.Equal("[\"(!null)\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesExistsCriteriaProperly()
        {
            var actual = Criteria.Exists("some expression").ToJson();
            Assert.Equal("[\"(exists)\",[\"some expression\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\"&\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b") & new Criteria("c")).ToJson();
            Assert.Equal("[[[\"a\"],\"&\",[\"b\"]],\"&\",[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesOrCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.Equal("[[\"c\"],\"|\",[\"d\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleOrCriteriaProperly()
        {
            var actual = (new Criteria("a") | new Criteria("b") | new Criteria("c")).ToJson();
            Assert.Equal("[[[\"a\"],\"|\",[\"b\"]],\"|\",[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesXorCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.Equal("[[\"c\"],\"|\",[\"d\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesMultipleXorCriteriaProperly()
        {
            var actual = (new Criteria("a") ^ new Criteria("b") ^ new Criteria("c")).ToJson();
            Assert.Equal("[[[\"a\"],\"^\",[\"b\"]],\"^\",[\"c\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesEQCriteriaProperly()
        {
            var actual = (new Criteria("a") == new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\"=\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNECriteriaProperly()
        {
            var actual = (new Criteria("a") != new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\"!=\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesGTCriteriaProperly()
        {
            var actual = (new Criteria("a") > new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\">\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesGECriteriaProperly()
        {
            var actual = (new Criteria("a") >= new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\">=\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLTCriteriaProperly()
        {
            var actual = (new Criteria("a") < new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\"<\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLECriteriaProperly()
        {
            var actual = (new Criteria("a") <= new Criteria("b")).ToJson();
            Assert.Equal("[[\"a\"],\"<=\",[\"b\"]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesINCriteriaProperly()
        {
            var actual = (new Criteria("a").In("b", "c", "d")).ToJson();
            Assert.Equal("[[\"a\"],\"(in)\",[[\"b\",\"c\",\"d\"]]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNotINCriteriaProperly()
        {
            var actual = (new Criteria("a").NotIn("b")).ToJson();
            Assert.Equal("[[\"a\"],\"(!in)\",[[\"b\"]]]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").StartsWith("b")).ToJson();
            Assert.Equal("[[\"a\"],\"~\",\"b%\"]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesNotLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").NotLike("%b")).ToJson();
            Assert.Equal("[[\"a\"],\"!~\",\"%b\"]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesValueCriteriaProperly()
        {
            var actual = (new Criteria("a") >= 5).ToJson();
            Assert.Equal("[[\"a\"],\">=\",5]", actual);
        }

        [Fact]
        public void JsonCriteriaConverter_SerializesComplexCriteriaProperly()
        {
            var actual = (
                (~(new Criteria("a") >= 5 & new Criteria("c").IsNotNull())) | 
                  (new Criteria("x").In(4, 5, 6)) != 7).ToJson();
            Assert.Equal("[[\"()\",[[[\"a\"],\">=\",5],\"&\",[\"(!null)\",[\"c\"]]]],\"|\",[[[\"x\"],\"(in)\",[[4,5,6]]],\"!=\",7]]", actual);
        }
    }
}