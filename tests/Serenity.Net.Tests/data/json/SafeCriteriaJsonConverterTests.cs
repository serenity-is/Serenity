using System.Text.Json;

namespace Serenity.JsonConverters;

public class SafeCriteriaJsonConverterMoreTests
{
    private static JsonSerializerOptions NewOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SafeCriteriaJsonConverter());
        return options;
    }

    [Fact]
    public void Read_Null_Returns_Null()
    {
        Assert.Null(JsonSerializer.Deserialize<BaseCriteria>("null", NewOptions()));
    }

    [Fact]
    public void Read_Safe_Criteria_Validates_And_Returns()
    {
        var criteria = JsonSerializer.Deserialize<BaseCriteria>(
            """["T0.[A]", "=", 5]""", NewOptions());

        Assert.NotNull(criteria);
    }
}
