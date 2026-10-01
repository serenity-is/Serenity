using System.Text.Json;
using System.Globalization;
using System.Numerics;

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
        return JsonSerializer.Deserialize<BaseCriteria>(json, options);
    }

    // Write

    [Fact]
    public void Write_EmptyCriteria_WritesEmptyArray()
    {
        var options = GetOptions();

        Assert.Equal("[]", JsonSerializer.Serialize(Criteria.Empty, options));
    }

    [Fact]
    public void Write_NullCriteria_WritesNull()
    {
        var options = GetOptions();

        Assert.Equal("null", JsonSerializer.Serialize<BaseCriteria>(null, options));
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

        Assert.Equal("[\"is null\",[\"Name\"]]", JsonSerializer.Serialize(
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
    public void Write_FunctionCallCriteria_WritesFunctionNode()
    {
        var json = JsonSerializer.Serialize<BaseCriteria>(
            new UpperFunctionCriteria(new Criteria("Name")), GetOptions());

        Assert.Equal("[\"function\",[\"UPPER\",[\"Name\"]]]", json);
    }

    [Fact]
    public void Read_FunctionNode_Parses_Whitelisted_Function_And_Arguments()
    {
        var result = Assert.IsAssignableFrom<FunctionCallCriteria>(JsonSerializer.Deserialize<BaseCriteria>(
            "[\"function\",[\"upper\",[\"Name\"]]]", GetOptions()));

        var arguments = result.Arguments;
        Assert.Single(arguments);
        Assert.Equal("Name", Assert.IsType<Criteria>(arguments[0]).Expression);
    }

    [Fact]
    public void Read_FunctionNode_Rejects_Unregistered_Name()
    {
        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BaseCriteria>(
            "[\"function\",[\"DELETE_ALL\"]]", GetOptions()));

        Assert.Contains("isn't supported in criteria JSON", ex.Message);
    }

    [Fact]
    public void FunctionFactory_Can_Be_Extended()
    {
        const string functionName = "TEST_MULTI";
        var source = new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>
        {
            [functionName] = arguments => new TestFunctionCriteria(arguments)
        };
        var oldFactories = FunctionCallCriteriaFactory.SetLocalFactories(
            source);
        try
        {
            source.Clear();
            var result = JsonSerializer.Deserialize<BaseCriteria>(
                "[\"function\",[\"TEST_MULTI\",[\"Name\"],[\"A\",\"=\",5]]]", GetOptions());

            var function = Assert.IsType<TestFunctionCriteria>(result);
            Assert.Equal(2, function.Arguments.Length);
            Assert.Equal("Name", Assert.IsType<Criteria>(function.Arguments[0]).Expression);
            Assert.IsType<BinaryCriteria>(function.Arguments[1]);
        }
        finally
        {
            FunctionCallCriteriaFactory.SetLocalFactories(oldFactories);
        }
    }

    [Fact]
    public void LocalFactory_IsReadOnly_AndCopied()
    {
        const string functionName = "TEST_LOCAL";
        var source = new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>
        {
            [functionName] = arguments => new TestFunctionCriteria(arguments)
        };
        var oldFactories = FunctionCallCriteriaFactory.SetLocalFactories(source);
        try
        {
            source.Clear();
            Assert.IsType<TestFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                functionName, [new Criteria("Name")]));

            var local = Assert.IsAssignableFrom<IDictionary<string,
                Func<BaseCriteria[], FunctionCallCriteria?>>>(
                    FunctionCallCriteriaFactory.SetLocalFactories(null));
            Assert.Throws<NotSupportedException>(() =>
                local.Add("OTHER", arguments => new TestFunctionCriteria(arguments)));
        }
        finally
        {
            FunctionCallCriteriaFactory.SetLocalFactories(oldFactories);
        }
    }

    [Fact]
    public void LocalFactories_AreExclusive_And_Null_Restores_Global_Factories()
    {
        const string functionName = "TEST_LOCAL_ONLY";
        var oldFactories = FunctionCallCriteriaFactory.SetLocalFactories(
            new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>
            {
                [functionName] = arguments => new TestFunctionCriteria(arguments)
            }, merge: false);
        try
        {
            Assert.IsType<TestFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                functionName, [new Criteria("Name")]));
            Assert.Null(FunctionCallCriteriaFactory.Create("UPPER", [new Criteria("Name")]));

            FunctionCallCriteriaFactory.SetLocalFactories(null, merge: false);

            Assert.IsType<UpperFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                "UPPER", [new Criteria("Name")]));
            Assert.Null(FunctionCallCriteriaFactory.Create(functionName, [new Criteria("Name")]));
        }
        finally
        {
            FunctionCallCriteriaFactory.SetLocalFactories(oldFactories, merge: false);
        }
    }

    [Fact]
    public void SetFactories_Merges_ByDefault_And_Can_Replace_And_Restore_Snapshot()
    {
        const string mergedName = "TEST_MERGED";
        const string replacedName = "TEST_REPLACED";
        var original = FunctionCallCriteriaFactory.SetFactories(null);
        var incoming = new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>
        {
            [mergedName] = arguments => new TestFunctionCriteria(arguments)
        };

        try
        {
            var previous = FunctionCallCriteriaFactory.SetFactories(incoming);
            Assert.Contains("UPPER", previous.Keys);
            Assert.DoesNotContain(mergedName, previous.Keys);
            incoming.Clear();

            Assert.IsType<UpperFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                "UPPER", [new Criteria("Name")]));
            Assert.IsType<TestFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                mergedName, [new Criteria("Name")]));

            var replacement = new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>
            {
                ["UPPER"] = args => args.Length == 1 ? new UpperFunctionCriteria(args[0]) : null,
                [replacedName] = args => new TestFunctionCriteria(args)
            };

            var mergedSnapshot = FunctionCallCriteriaFactory.SetFactories(replacement, merge: false);
            Assert.Contains(mergedName, mergedSnapshot.Keys);
            Assert.Null(FunctionCallCriteriaFactory.Create(mergedName, [new Criteria("Name")]));
            Assert.IsType<UpperFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                "upper", [new Criteria("Name")]));
            Assert.IsType<TestFunctionCriteria>(FunctionCallCriteriaFactory.Create(
                replacedName, [new Criteria("Name")]));

            var replacedSnapshot = FunctionCallCriteriaFactory.SetFactories(mergedSnapshot, merge: false);
            Assert.Contains(replacedName, replacedSnapshot.Keys);
            Assert.Null(FunctionCallCriteriaFactory.Create(replacedName, [new Criteria("Name")]));
        }
        finally
        {
            FunctionCallCriteriaFactory.SetFactories(original, merge: false);
        }
    }

    [Fact]
    public void Read_BinaryCriteria_With_FunctionStringLeftOperand_IsNotAmbiguous()
    {
        var result = Assert.IsType<BinaryCriteria>(JsonSerializer.Deserialize<BaseCriteria>(
            "[\"function\",\"=\",[\"something\"]]", GetOptions()));

        Assert.Equal(CriteriaOperator.EQ, result.Operator);
        Assert.Equal("function", Assert.IsType<ValueCriteria>(result.LeftOperand).Value);
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

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"some text\"]", options);

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

        var result = JsonSerializer.Deserialize<BaseCriteria>("[[\"a\",\"b\",true,false,3]]", options);

        Assert.IsType<ValueCriteria>(result);
        var value = Assert.IsType<object[]>(((ValueCriteria)result).Value);
        Assert.Equal(["a", "b", true, false, 3L], value);
    }

    [Fact]
    public void Read_Integer_Preserves_Int64_Above_Double_Precision()
    {
        var result = Assert.IsType<ValueCriteria>(JsonSerializer.Deserialize<BaseCriteria>(
            "9007199254740993", GetOptions()));

        Assert.Equal(9007199254740993L, Assert.IsType<long>(result.Value));
    }

    [Fact]
    public void Read_Binary_IntegerOperand_Preserves_Int64_Above_Double_Precision()
    {
        var result = Assert.IsType<BinaryCriteria>(JsonSerializer.Deserialize<BaseCriteria>(
            "[\"A\",\"=\",9007199254740993]", GetOptions()));

        Assert.Equal(9007199254740993L,
            Assert.IsType<long>(Assert.IsType<ValueCriteria>(result.RightOperand).Value));
    }

    [Fact]
    public void Read_Integer_Larger_Than_Int64_Preserves_BigInteger()
    {
        var result = Assert.IsType<ValueCriteria>(JsonSerializer.Deserialize<BaseCriteria>(
            "18446744073709551616", GetOptions()));

        Assert.Equal(BigInteger.Parse("18446744073709551616", CultureInfo.InvariantCulture),
            Assert.IsType<BigInteger>(result.Value));
    }

    [Theory]
    [InlineData("1.25", 1.25)]
    [InlineData("1e2", 100.0)]
    public void Read_Fractional_And_Exponent_Numbers_Are_Double(string json, double expected)
    {
        var result = Assert.IsType<ValueCriteria>(JsonSerializer.Deserialize<BaseCriteria>(json, GetOptions()));

        Assert.Equal(expected, Assert.IsType<double>(result.Value));
    }

    [Fact]
    public void Read_DateTime_String_Preserves_DateTime_ValueType()
    {
        var expected = new DateTime(2023, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var json = JsonSerializer.Serialize<BaseCriteria>(new ValueCriteria(expected), GetOptions());

        var result = Assert.IsType<ValueCriteria>(JsonSerializer.Deserialize<BaseCriteria>(json, GetOptions()));

        Assert.Equal(expected, Assert.IsType<DateTime>(result.Value));
    }

    [Fact]
    public void Read_Binary_DateTimeOperand_Preserves_DateTime_ValueType()
    {
        const string json = "[\"A\",\"=\",\"2023-01-15T10:30:00Z\"]";

        var result = Assert.IsType<BinaryCriteria>(JsonSerializer.Deserialize<BaseCriteria>(json, GetOptions()));

        Assert.IsType<DateTime>(Assert.IsType<ValueCriteria>(result.RightOperand).Value);
    }

    [Fact]
    public void Read_Guid_String_Remains_String_Without_Type_Metadata()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var json = JsonSerializer.Serialize<BaseCriteria>(new ValueCriteria(guid), GetOptions());

        var result = Assert.IsType<ValueCriteria>(JsonSerializer.Deserialize<BaseCriteria>(json, GetOptions()));

        // A Guid and a string containing the same text have identical JSON
        // representations, so the criteria wire format has no type information
        // with which to distinguish them during deserialization.
        Assert.Equal(guid.ToString("D"), Assert.IsType<string>(result.Value));
    }

    [Fact]
    public void Read_NestedArrayWithNullValueItem_AddsNullToValues()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[[\"a\",null]]", options);

        var value = Assert.IsType<object?[]>(((ValueCriteria)result!).Value);
        Assert.Equal(2, value.Length);
        Assert.Equal("a", value[0]);
        Assert.Null(value[1]);
    }

    [Fact]
    public void Read_EmptyArray_ReturnsEmptyCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[]", options);

        Assert.True(result is not null && result.IsEmpty);
    }

    [Fact]
    public void Read_NullToken_DeserializesAsNull()
    {
        var options = GetOptions();

        Assert.Null(JsonSerializer.Deserialize<BaseCriteria>("null", options));
    }

    [Fact]
    public void Read_NestedEmptyArray_ThrowsJsonException()
    {
        var options = GetOptions();

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"=\",[]]", options));
    }

    [Theory]
    [InlineData("\"string criteria\"", "string criteria")]
    [InlineData("5", 5L)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Read_ScalarRoot_ParsesAsValueCriteria(string json, object expected)
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>(json, options);

        Assert.Equal(expected, Assert.IsType<ValueCriteria>(result!).Value);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[\"A\",\"=\",{\"x\":1}]")]
    public void Read_ObjectRootOrOperand_ThrowsJsonException(string json)
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

        var result = JsonSerializer.Deserialize<BaseCriteria>($"[{opKey},\"Name\"]", options);

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

        var result = JsonSerializer.Deserialize<BaseCriteria>($"[\"A\",{opKey},\"B\"]", options);

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

    [Theory]
    [InlineData("[5,\"Name\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",5,\"B\"]", "Couldn't deserialize unary criteria")]
    [InlineData("[\"A\",\"xyz\",\"B\"]", "Unknown Criteria operator")]
    [InlineData("[\"A\",\"=\",\"B\",\"C\"]", "Invalid Criteria format")]
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

        var result = JsonSerializer.Deserialize<BaseCriteria>(json, options);

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.IsType<bool>(Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void Read_EmptyStringExpression_DeserializesButValidatorRejects()
    {
        // Converter itself still parses; SafeCriteriaValidator rejects empty nested criteria.
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"\"]", options);

        Assert.Equal("", ((Criteria)result!).Expression);
    }

    [Fact]
    public void Read_NullToken_DirectConverterCall_ReturnsEmpty()
    {
        // Note: reader is not advanced here (TokenType.None), so this exercises
        // the Deserialize-then-Parse path, which maps null input to empty criteria.
        // The framework path with a positioned Null token returns C# null instead.
        var bytes = Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        var converter = new CriteriaJsonConverter();

        var result = converter.Read(ref reader, typeof(BaseCriteria), GetOptions());

        Assert.False(result is null || !result.IsEmpty);
    }

    [Fact]
    public void Read_BinaryOperatorValueNullOperand_ParsesAsNullValueCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"=\",null]", options);

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Null(Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void RoundTrip_EmptyCriteria_PreservesEmpty()
    {
        var result = RoundTrip(Criteria.Empty);

        Assert.True(result is not null && result.IsEmpty);
    }

    [Fact]
    public void RoundTrip_ScalarValueCriteria_PreservesValue()
    {
        var result = Assert.IsType<ValueCriteria>(RoundTrip(new ValueCriteria("test")));

        Assert.Equal("test", result.Value);
    }

    // Round trips

    [Fact]
    public void RoundTrip_ScalarCriteriaExpression_PreservesCriteria()
    {
        var result = Assert.IsType<Criteria>(RoundTrip(new Criteria("Name")));

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
        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class TestFunctionCriteria(params BaseCriteria[] arguments)
        : FunctionCallCriteria(arguments)
    {
        public override string GetFunctionName(ISqlDialect dialect) => "TEST_MULTI";
    }
}
