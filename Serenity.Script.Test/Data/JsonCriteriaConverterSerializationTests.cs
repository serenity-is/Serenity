using QUnit;
using Serenity.Data;

namespace Serenity.Test
{
    [TestFixture]
    public class JsonCriteriaConverterSerializationTests
    {
        [Test]
        public void JsonCriteriaConverter_SerializesStringCriteriaAsIs()
        {
            var actual = (new Criteria("a")).ToJson();
            Assert.AreEqual(actual, "[\"a\"]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesParenCriteriaProperly()
        {
            var actual = (~(new Criteria("a"))).ToJson();
            Assert.AreEqual(actual, "[\"()\",[\"a\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesIsNullCriteriaProperly()
        {
            var actual = (new Criteria("x")).IsNull().ToJson();
            Assert.AreEqual(actual, "[\"is null\",[\"x\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesIsNotNullCriteriaProperly()
        {
            var actual = (new Criteria("b")).IsNotNull().ToJson();
            Assert.AreEqual(actual, "[\"is not null\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesExistsCriteriaProperly()
        {
            var actual = Criteria.Exists("some expression").ToJson();
            Assert.AreEqual(actual, "[\"exists\",[\"some expression\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"and\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesMultipleAndCriteriaProperly()
        {
            var actual = (new Criteria("a") & new Criteria("b") & new Criteria("c")).ToJson();
            Assert.AreEqual(actual, "[[[\"a\"],\"and\",[\"b\"]],\"and\",[\"c\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesOrCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.AreEqual(actual, "[[\"c\"],\"or\",[\"d\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesMultipleOrCriteriaProperly()
        {
            var actual = (new Criteria("a") | new Criteria("b") | new Criteria("c")).ToJson();
            Assert.AreEqual(actual, "[[[\"a\"],\"or\",[\"b\"]],\"or\",[\"c\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesXorCriteriaProperly()
        {
            var actual = (new Criteria("c") | new Criteria("d")).ToJson();
            Assert.AreEqual(actual, "[[\"c\"],\"or\",[\"d\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesMultipleXorCriteriaProperly()
        {
            var actual = (new Criteria("a") ^ new Criteria("b") ^ new Criteria("c")).ToJson();
            Assert.AreEqual(actual, "[[[\"a\"],\"xor\",[\"b\"]],\"xor\",[\"c\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesEQCriteriaProperly()
        {
            var actual = (new Criteria("a") == new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"=\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesNECriteriaProperly()
        {
            var actual = (new Criteria("a") != new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"!=\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesGTCriteriaProperly()
        {
            var actual = (new Criteria("a") > new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\">\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesGECriteriaProperly()
        {
            var actual = (new Criteria("a") >= new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\">=\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesLTCriteriaProperly()
        {
            var actual = (new Criteria("a") < new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"<\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesLECriteriaProperly()
        {
            var actual = (new Criteria("a") <= new Criteria("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"<=\",[\"b\"]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesINCriteriaProperly()
        {
            var actual = (new Criteria("a").In("b", "c", "d")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"in\",[[\"b\",\"c\",\"d\"]]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesNotINCriteriaProperly()
        {
            var actual = (new Criteria("a").NotIn("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"not in\",[[\"b\"]]]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").StartsWith("b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"like\",\"b%\"]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesNotLikeCriteriaProperly()
        {
            var actual = (new Criteria("a").NotLike("%b")).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\"not like\",\"%b\"]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesValueCriteriaProperly()
        {
            var actual = (new Criteria("a") >= 5).ToJson();
            Assert.AreEqual(actual, "[[\"a\"],\">=\",5]");
        }

        [Test]
        public void JsonCriteriaConverter_SerializesComplexCriteriaProperly()
        {
            var actual = (
                (~(new Criteria("a") >= 5 & new Criteria("c").IsNotNull())) | 
                  (new Criteria("x").In(4, 5, 6)) != 7).ToJson();
            Assert.AreEqual(actual, "[[\"()\",[[[\"a\"],\">=\",5],\"and\",[\"is not null\",[\"c\"]]]],\"or\",[[[\"x\"],\"in\",[[4,5,6]]],\"!=\",7]]");
        }
    }
}