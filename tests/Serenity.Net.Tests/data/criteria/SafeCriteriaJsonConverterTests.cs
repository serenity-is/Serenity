using System.Text.Json;

namespace Serenity.JsonConverters;

public class SafeCriteriaJsonConverterTests
{
    private static JsonSerializerOptions GetOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SafeCriteriaJsonConverter());
        return options;
    }

    [Fact]
    public void CanInheritFromCriteriaJsonConverter()
    {
        Assert.IsAssignableFrom<CriteriaJsonConverter>(new SafeCriteriaJsonConverter());
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var options = GetOptions();

        Assert.Null(JsonSerializer.Deserialize<BaseCriteria>("null", options));
    }

    [Fact]
    public void Read_SafeCriteria_ReturnsCriteria()
    {
        var options = GetOptions();

        var result = JsonSerializer.Deserialize<BaseCriteria>("[\"Name\",\"=\",\"@p1\"]", options);

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal("@p1", Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void Read_UnsafeExpression_ThrowsValidationError()
    {
        var options = GetOptions();

        Assert.Throws<ValidationError>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"Name; DROP TABLE\"]", options));
    }

    [Fact]
    public void Read_ParamCriteria_ThrowsValidationError()
    {
        var options = GetOptions();

        Assert.Throws<ValidationError>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"@p1\"]", options));
    }

    [Fact]
    public void Write_WritesSameAsBaseConverter()
    {
        var options = GetOptions();

        var json = JsonSerializer.Serialize(new Criteria("Name").IsNull(), options);

        Assert.Equal("[\"is null\",[\"Name\"]]", json);
    }
}
