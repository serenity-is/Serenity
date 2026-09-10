using Serenity.JsonConverters;
using System.Collections;

namespace Serenity.Data;

public class JsonCriteriaConverterTests
{
    private static Newtonsoft.Json.JsonSerializerSettings GetSettings()
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings();
        settings.Converters.Add(new JsonCriteriaConverter());
        return settings;
    }

    private static BaseCriteria? RoundTrip(BaseCriteria criteria)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>(
            Newtonsoft.Json.JsonConvert.SerializeObject(criteria, GetSettings()), GetSettings());
    }

    // CanConvert / CanRead / CanWrite

    [Theory]
    [InlineData(typeof(Criteria), true)]
    [InlineData(typeof(ValueCriteria), true)]
    [InlineData(typeof(UnaryCriteria), true)]
    [InlineData(typeof(BaseCriteria), false)]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(object), false)]
    public void CanConvert_ChecksForBaseCriteriaSubclass(Type type, bool expected)
    {
        var converter = new JsonCriteriaConverter();

        Assert.Equal(expected, converter.CanConvert(type));
    }

    [Fact]
    public void CanRead_ReturnsTrue() => Assert.True(new JsonCriteriaConverter().CanRead);

    [Fact]
    public void CanWrite_ReturnsTrue() => Assert.True(new JsonCriteriaConverter().CanWrite);

    // Write

    [Fact]
    public void Write_NullCriteria_WritesNull()
    {
        Assert.Equal("null", Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria?)null, GetSettings()));
    }

    [Fact]
    public void Write_EmptyCriteria_WritesNull()
    {
        Assert.Equal("null", Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria)Criteria.Empty, GetSettings()));
    }

    [Fact]
    public void Write_ValueCriteriaString_WritesString()
    {
        Assert.Equal("\"test\"", Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria)new ValueCriteria("test"), GetSettings()));
    }

    [Fact]
    public void Write_ValueCriteriaArray_IsWrappedInOuterArray()
    {
        Assert.Equal("[[\"a\",\"b\"]]", Newtonsoft.Json.JsonConvert.SerializeObject(
            (BaseCriteria)new ValueCriteria(new object[] { "a", "b" }), GetSettings()));
    }

    [Fact]
    public void Write_ValueCriteriaOperatorFirstArray_IsWrapped()
    {
        Assert.Equal("[[\">\",\"a\",\"b\"]]", Newtonsoft.Json.JsonConvert.SerializeObject(
            (BaseCriteria)new ValueCriteria(new object[] { ">", "a", "b" }), GetSettings()));
    }

    [Fact]
    public void Write_ParamCriteria_WritesNameAsArray()
    {
        Assert.Equal("[\"@p1\"]", Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria)new ParamCriteria("@p1"), GetSettings()));
    }

    [Fact]
    public void Write_Criteria_WritesExpressionAsArray()
    {
        Assert.Equal("[\"Name\"]", Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria)new Criteria("Name"), GetSettings()));
    }

    [Fact]
    public void Write_UnaryCriteria_WritesOperatorKeyAndOperand()
    {
        Assert.Equal("[\"is null\",[\"Name\"]]", Newtonsoft.Json.JsonConvert.SerializeObject(
            (BaseCriteria)new Criteria("Name").IsNull(), GetSettings()));
    }

    [Fact]
    public void Write_BinaryCriteria_WritesOperandsAndOperatorKey()
    {
        var binary = new BinaryCriteria(new Criteria("A"), CriteriaOperator.AND, new Criteria("B"));

        Assert.Equal("[[\"A\"],\"and\",[\"B\"]]", Newtonsoft.Json.JsonConvert.SerializeObject(
            binary, GetSettings()));
    }

    [Fact]
    public void Write_UnsupportedCriteriaType_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.SerializeObject((BaseCriteria)new CustomCriteria(), GetSettings()));

        Assert.Contains("Can't serialize criteria of type", ex.Message);
    }

    [Fact]
    public void Write_GenericListValue_SerializedByNestedSerializer()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(
            (BaseCriteria)new ValueCriteria(new List<int> { 1, 2, 3 }), GetSettings());

        Assert.Equal("[[1,2,3]]", json);
    }

    // Read

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        Assert.Null(Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("null", GetSettings()));
    }

    [Fact]
    public void Read_StringExpression_ParsesExpressionCriteria()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"Name\"]", GetSettings())!;

        Assert.IsType<Criteria>(result);
        Assert.Equal("Name", ((Criteria)result).Expression);
    }

    [Fact]
    public void Read_ParamExpression_ParsesParamCriteria()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"@MyParam\"]", GetSettings())!;

        Assert.Equal("@MyParam", Assert.IsType<ParamCriteria>(result).Name);
    }

    [Fact]
    public void Read_NestedArray_ParsesAsValueCriteriaList()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[[\"a\",\"b\",true,3]]", GetSettings())!;

        var value = Assert.IsType<object[]>(((ValueCriteria)result).Value);
        Assert.Equal(new object[] { "a", "b", true, 3L }, value);
    }

    [Fact]
    public void Read_NestedArrayWithNullJValueItem_AddsNullToValues()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[[\"a\",null]]", GetSettings())!;

        var value = Assert.IsType<object[]>(((ValueCriteria)result).Value);
        Assert.Equal(2, value.Length);
        Assert.Equal("a", value[0]);
        Assert.Null(value[1]);
    }

    [Fact]
    public void Read_NestedArrayWithNonValueItem_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[[{\"x\":1}]]", GetSettings()));

        Assert.Contains("as Criteria value", ex.Message);
    }

    [Fact]
    public void Read_EmptyArray_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[]", GetSettings()));

        Assert.Equal("Can't deserialize empty array as Criteria", ex.Message);
    }

    [Fact]
    public void Read_SingleNonStringJValue_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[5]", GetSettings()));

        Assert.Contains("Couldn't deserialize string criteria", ex.Message);
    }

    [Fact]
    public void Read_SingleNullJValue_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[null]", GetSettings()));

        Assert.Contains("Couldn't deserialize string criteria", ex.Message);
    }

    [Theory]
    [InlineData("\"is null\"")]
    [InlineData("\"is not null\"")]
    [InlineData("\"exists\"")]
    [InlineData("\"not\"")]
    [InlineData("\"()\"")]
    public void Read_UnaryCriteria_ParsesOperatorAndOperand(string opKey)
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>($"[{opKey},\"Name\"]", GetSettings())!;

        Assert.IsType<UnaryCriteria>(result);
    }

    [Fact]
    public void Read_UnknownOperator_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"xyz\",\"Name\"]", GetSettings()));

        Assert.Contains("Unknown Criteria operator", ex.Message);
    }

    [Fact]
    public void Read_BinaryOperatorInUnaryPosition_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"and\",\"Name\"]", GetSettings()));

        Assert.Contains("Invalid Unary Criteria format", ex.Message);
    }

    [Theory]
    [InlineData("\"and\"")]
    [InlineData("\"or\"")]
    [InlineData("\"xor\"")]
    [InlineData("\"=\"")]
    [InlineData("\"not like\"")]
    public void Read_BinaryCriteria_ParsesOperandsAndOperator(string opKey)
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>($"[\"A\",{opKey},\"B\"]", GetSettings())!;

        Assert.IsType<BinaryCriteria>(result);
    }

    [Fact]
    public void Read_UnaryOperatorInBinaryPosition_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"A\",\"is null\",\"B\"]", GetSettings()));

        Assert.Contains("Invalid Criteria format", ex.Message);
    }

    [Fact]
    public void Read_ParamCriteriaInsideBinary_ParseSubValue()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"A\",\"=\",[\"@p3\"]]", GetSettings())!;

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal("@p3", Assert.IsType<ParamCriteria>(binary.RightOperand).Name);
    }

    [Theory]
    [InlineData("[5,\"Name\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",5,\"B\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",\"xyz\",\"B\"]", "Unknown Criteria operator")]
    [InlineData("[\"A\",\"=\",\"B\",\"C\"]", "Can't deserialize")]
    public void Read_MalformedArrayShapes_ThrowJsonSerializationException(string json, string messagePart)
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>(json, GetSettings()));

        Assert.Contains(messagePart, ex.Message);
    }

    [Fact]
    public void Read_ObjectElementInBinary_ThrowsJsonSerializationException()
    {
        var ex = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"A\",\"=\",{\"x\":1}]", GetSettings()));

        Assert.Contains("as Criteria value", ex.Message);
    }

    [Fact]
    public void Read_BoolOperand_ParsesAsValueCriteria()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>("[\"Name\",\"=\",true]", GetSettings())!;

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.IsType<bool>(Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    // Round trips

    [Fact]
    public void RoundTrip_ScalarCriteriaExpression_PreservesCriteria()
    {
        var result = Assert.IsType<Criteria>(RoundTrip(new Criteria("Name"))!);

        Assert.Equal("Name", result.Expression);
    }

    [Fact]
    public void RoundTrip_OperatorFirstArrayValue_DoesNotTurnValueIntoOperator()
    {
        var result = Assert.IsType<ValueCriteria>(RoundTrip(
            new ValueCriteria(new object[] { ">", "a", "b" }))!);

        var value = Assert.IsType<object[]>(result.Value);
        Assert.Equal(new object[] { ">", "a", "b" }, value);
    }

    [Fact]
    public void RoundTrip_NestedBinaryCriteria_PreservesStructure()
    {
        var criteria = new Criteria("Name").StartsWith("a") &
            new Criteria("Age") >= 18;

        var result = Assert.IsType<BinaryCriteria>(RoundTrip(criteria)!);

        Assert.Equal(CriteriaOperator.AND, result.Operator);
        Assert.IsType<BinaryCriteria>(result.LeftOperand);
        Assert.IsType<BinaryCriteria>(result.RightOperand);
    }

    [Fact]
    public void RoundTrip_UnaryCriteria_PreservesOperator()
    {
        var result = Assert.IsType<UnaryCriteria>(RoundTrip(new Criteria("Name").IsNull())!);

        Assert.Equal(CriteriaOperator.IsNull, result.Operator);
    }

    // Custom (unsupported) criteria type for serializer error branches

    private sealed class CustomCriteria : BaseCriteria
    {
        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            throw new NotImplementedException();
        }
    }
}

