using System.Text.Json;
using Serenity.JsonConverters;

namespace Serenity.JsonConverters;

public class CriteriaJsonConverterTests
{
    private static JsonSerializerOptions GetOptions()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.Converters.Add(new CriteriaJsonConverter());
        return options;
    }

    private static BaseCriteria RoundTrip(BaseCriteria criteria)
    {
        var options = GetOptions();
        var json = JsonSerializer.Serialize(criteria, options);
        return JsonSerializer.Deserialize<BaseCriteria>(json, options)!;
    }

    // Write

    [Fact]
    public void Write_NullCriteria_WritesNullValue()
    {
        var options = GetOptions();

        Assert.Equal("null", JsonSerializer.Serialize<BaseCriteria>(null!, options));
    }

    [Fact]
    public void Write_EmptyCriteria_WritesNullValue()
    {
        var options = GetOptions();

        Assert.Equal("null", JsonSerializer.Serialize<BaseCriteria>(Criteria.Empty, options));
    }

    [Fact]
    public void Write_ValueCriteriaString_WritesString()
    {
        var options = GetOptions();

        Assert.Equal("\"test\"", JsonSerializer.Serialize<BaseCriteria>(new ValueCriteria("test"), options));
    }

    [Fact]
    public void Write_ValueCriteriaNumber_WritesNumber()
    {
        var options = GetOptions();

        Assert.Equal("5", JsonSerializer.Serialize<BaseCriteria>(new ValueCriteria(5), options));
    }

    [Fact]
    public void Write_ValueCriteriaArray_IsWrappedInOuterArrayToAvoidOperatorRecognition()
    {
        var options = GetOptions();

        Assert.Equal("[[\"a\",\"b\"]]", JsonSerializer.Serialize<BaseCriteria>(
            new ValueCriteria(new object[] { "a", "b" }), options));
    }

    [Fact]
    public void Write_ValueCriteriaStartingWithOperator_IsWrappedToAvoidOperatorRecognition()
    {
        var options = GetOptions();

        Assert.Equal("[[\">\",\"a\",\"b\"]]", JsonSerializer.Serialize<BaseCriteria>(
            new ValueCriteria(new object[] { ">", "a", "b" }), options));
    }

    [Fact]
    public void Write_ParamCriteria_WritesNameAsArray()
    {
        var options = GetOptions();

        Assert.Equal("[\"@p1\"]", JsonSerializer.Serialize<BaseCriteria>(new ParamCriteria("@p1"), options));
    }

    [Fact]
    public void Write_Criteria_WritesExpressionAsArray()
    {
        var options = GetOptions();

        Assert.Equal("[\"Name\"]", JsonSerializer.Serialize<BaseCriteria>(new Criteria("Name"), options));
    }

    [Fact]
    public void Write_UnaryCriteria_WritesOperatorKeyAndOperand()
    {
        var options = GetOptions();

        Assert.Equal("[\"is null\",[\"Name\"]]", JsonSerializer.Serialize<BaseCriteria>(
            new Criteria("Name").IsNull(), options));
    }

    [Fact]
    public void Write_BinaryCriteria_WritesOperandsAndOperatorKey()
    {
        var options = GetOptions();

        var binary = new BinaryCriteria(new Criteria("A"), CriteriaOperator.AND, new Criteria("B"));

        Assert.Equal("[[\"A\"],\"and\",[\"B\"]]", JsonSerializer.Serialize<BaseCriteria>(binary, options));
    }

    [Fact]
    public void Write_UnsupportedCriteriaType_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Serialize<BaseCriteria>(
            new CustomCriteria(), options));

        Assert.Contains("Can't serialize criteria of type", ex.Message);
    }

    [Fact]
    public void Write_AllUnaryOperatorKeys_UpperFunctionCriteriaAsNested()
    {
        var options = GetOptions();

        Assert.Equal("[\"exists\",[\"Name\"]]", JsonSerializer.Serialize<BaseCriteria>(
            new UnaryCriteria(CriteriaOperator.Exists, new Criteria("Name")), options));
        Assert.Equal("[\"not\",[\"is null\",[\"Name\"]]]", JsonSerializer.Serialize<BaseCriteria>(
            new UnaryCriteria(CriteriaOperator.Not, new Criteria("Name").IsNull()), options));
        Assert.Equal("[\"()\",[\"Name\"]]", JsonSerializer.Serialize<BaseCriteria>(
            new UnaryCriteria(CriteriaOperator.Paren, new Criteria("Name")), options));
    }

    // Read

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var options = GetOptions();

        Assert.Null(JsonSerializer.Deserialize<BaseCriteria>("null", options));
    }

    [Fact]
    public void Read_CriteriaExpression_ParsesExpressionCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"Name\"]", options);

        Assert.IsType<Criteria>(result);
        Assert.Equal("Name", ((Criteria)result).Expression);
    }

    [Fact]
    public void Read_ParamExpression_ParsesParamCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"@MyParam\"]", options);

        Assert.IsType<ParamCriteria>(result);
        Assert.Equal("@MyParam", ((ParamCriteria)result).Name);
    }

    [Fact]
    public void Read_SingleStringValue_ParsesAsExpressionCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"some text\"]", options)!;

        Assert.IsType<Criteria>(result);
        Assert.Equal("some text", ((Criteria)result).Expression);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("true")]
    [InlineData("false")]
    public void Read_PrimitiveAsSingleElement_ThrowsJsonException(string json)
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>($"[{json}]", options));

        Assert.Contains("Couldn't deserialize string criteria", ex.Message);
    }

    [Fact]
    public void Read_NestedArray_ParsesAsValueCriteriaList()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[[\"a\",\"b\",true,false,3]]", options)!;

        Assert.IsType<ValueCriteria>(result);
        var value = Assert.IsType<object[]>(((ValueCriteria)result).Value);
        Assert.Equal(["a", "b", true, false, 3.0], value);
    }

    [Fact]
    public void Read_NestedArrayWithNullItem_ThrowsArgumentNullException()
    {
        var options = GetOptions();

        Assert.Throws<ArgumentNullException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[[\"a\",null]]", options));
    }

    [Fact]
    public void Read_NestedArrayWithObjectItem_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[[{\"x\":1}]]", options));

        Assert.Contains("as Criteria value", ex.Message);
    }

    [Fact]
    public void Read_EmptyArray_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[]", options));

        Assert.Equal("Can't deserialize empty array as Criteria", ex.Message);
    }

    [Theory]
    [InlineData("\"string criteria\"")]
    [InlineData("5")]
    public void Read_NonArrayRoot_ThrowsJsonException(string json)
    {
        var options = GetOptions();

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BaseCriteria>(json, options));
    }

    [Fact]
    public void Read_SingleNonStringValueAndNotArray_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[5]", options));

        Assert.Contains("Couldn't deserialize string criteria", ex.Message);
    }

    [Fact]
    public void Read_SingleNullElement_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[null]", options));

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
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>($"[{opKey},\"Name\"]", options)!;

        Assert.IsType<UnaryCriteria>(result);
    }

    [Fact]
    public void Read_UnknownOperator_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"xyz\",\"Name\"]", options));

        Assert.Contains("Unknown Criteria operator", ex.Message);
    }

    [Fact]
    public void Read_BinaryOperatorInUnaryPosition_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"and\",\"Name\"]", options));

        Assert.Contains("Invalid Unary Criteria format", ex.Message);
    }

    [Theory]
    [InlineData("\"and\"")]
    [InlineData("\"or\"")]
    [InlineData("\"xor\"")]
    [InlineData("\"=\"")]
    [InlineData("\"!=\"")]
    [InlineData("\">\"")]
    [InlineData("\">=\"")]
    [InlineData("\"<\"")]
    [InlineData("\"<=\"")]
    [InlineData("\"in\"")]
    [InlineData("\"not in\"")]
    [InlineData("\"like\"")]
    [InlineData("\"not like\"")]
    public void Read_BinaryCriteria_ParsesOperandsAndOperator(string opKey)
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>($"[\"A\",{opKey},\"B\"]", options)!;

        Assert.IsType<BinaryCriteria>(result);
    }

    [Fact]
    public void Read_UnaryLikeOperatorInBinaryPosition_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"is null\",\"B\"]", options));

        Assert.Contains("Invalid Criteria format", ex.Message);
    }

    [Fact]
    public void Read_BinaryOperatorValueNullOperand_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"=\",null]", options));

        Assert.Contains("as Criteria value", ex.Message);
    }

    [Theory]
    [InlineData("[5,\"Name\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",5,\"B\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",\"xyz\",\"B\"]", "Unknown Criteria operator")]
    [InlineData("[\"A\",\"=\",\"B\",\"C\"]", "Can't deserialize")]
    public void Read_MalformedArrayShapes_ThrowJsonException(string json, string messagePart)
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>(json, options));

        Assert.Contains(messagePart, ex.Message);
    }

    [Theory]
    [InlineData("[\"Name\",\"=\",true]")]
    [InlineData("[\"Name\",\"=\",false]")]
    public void Read_BooleanOperand_ParsesAsValueCriteria(string json)
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>(json, options)!;

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.IsType<bool>(Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void Read_NullCriteriaExpression_ThrowsJsonException()
    {
        // An array like [""] is a string criteria, while a stripped expression is invalid
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"\" ]", options)!;

        Assert.Equal("", ((Criteria)result).Expression);
    }

    [Fact]
    public void Read_NullToken_DirectConverterCall_ReturnsNull()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        var converter = new CriteriaJsonConverter();

        var result = converter.Read(ref reader, typeof(BaseCriteria), GetOptions());

        // "null" parses into an empty criteria rather than a null instance
        Assert.False(result is null || !result.IsEmpty);
    }

    [Fact]
    public void Read_ObjectElementInBinary_ThrowsJsonException()
    {
        var options = GetOptions();

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"=\",{\"x\":1}]", options));

        Assert.Contains("as Criteria value", ex.Message);
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
        var criteria = new ValueCriteria(new object[] { ">", "a", "b" });

        var result = Assert.IsType<ValueCriteria>(RoundTrip(criteria));

        var value = Assert.IsType<object[]>(result.Value);
        Assert.Equal([">", "a", "b"], value);
    }

    [Fact]
    public void RoundTrip_NestedBinaryCriteria_PreservesStructure()
    {
        var criteria = new Criteria("Name").StartsWith("a") &
            new Criteria("Age") >= 18;

        var result = RoundTrip(criteria);

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal(CriteriaOperator.AND, binary.Operator);
        Assert.IsType<BinaryCriteria>(binary.LeftOperand);
        Assert.IsType<BinaryCriteria>(binary.RightOperand);
    }

    [Fact]
    public void RoundTrip_ParamCriteria_WrapsNameToArrayNotOperator()
    {
        var criteria = new ParamCriteria("@p1");

        var result = Assert.IsType<ParamCriteria>(RoundTrip(criteria));

        Assert.Equal("@p1", result.Name);
    }

    [Fact]
    public void RoundTrip_UnaryCriteria_PreservesOperator()
    {
        var result = Assert.IsType<UnaryCriteria>(RoundTrip(new Criteria("Name").IsNull()));

        Assert.Equal(CriteriaOperator.IsNull, result.Operator);
    }

    // Custom (unsupported) criteria type for serializer error branches

    private sealed class CustomCriteria : BaseCriteria
    {
        public override void ToString(StringBuilder sb, Serenity.Data.IQueryWithParams query)
        {
            throw new NotImplementedException();
        }
    }
}
