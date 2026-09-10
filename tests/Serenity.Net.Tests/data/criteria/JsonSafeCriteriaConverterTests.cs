namespace Serenity.Data;

public class JsonSafeCriteriaConverterTests
{
    [Fact]
    public void CanInheritFromJsonCriteriaConverter()
    {
        Assert.IsAssignableFrom<JsonCriteriaConverter>(new JsonSafeCriteriaConverter());
    }

    [Fact]
    public void CanConvert_ChecksForBaseCriteriaSubclass()
    {
        var converter = new JsonSafeCriteriaConverter();

        Assert.True(converter.CanConvert(typeof(Criteria)));
        Assert.False(converter.CanConvert(typeof(string)));
    }

    [Fact]
    public void Read_SafeCriteria_ReturnsCriteria()
    {
        var container = Newtonsoft.Json.JsonConvert.DeserializeObject<CriteriaContainer>(
            "{\"Criteria\": [\"Name\",\"=\",3]}", GetSettings());

        Assert.IsType<BinaryCriteria>(container!.Criteria);
    }

    [Fact]
    public void Read_UnsafeExpression_ThrowsValidationError()
    {
        Assert.Throws<ValidationError>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<CriteriaContainer>(
                "{\"Criteria\": [\"Name; DROP TABLE\"]}", GetSettings()));
    }

    [Fact]
    public void Read_ParamCriteria_ThrowsValidationError()
    {
        Assert.Throws<ValidationError>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<CriteriaContainer>(
                "{\"Criteria\": [\"@p1\"]}", GetSettings()));
    }

    [Fact]
    public void Write_WritesSameAsBaseConverter()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new CriteriaContainer
        {
            Criteria = new Criteria("Name").IsNull()
        }, GetSettings());

        Assert.Equal("{\"Criteria\":[\"is null\",[\"Name\"]]}", json);
    }

    [Fact]
    public void Write_NullCriteriaProperty_WritesNull()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(
            new CriteriaContainer(), GetSettings());

        Assert.Equal("{\"Criteria\":null}", json);
    }

    [Fact]
    public void Read_NullCriteriaProperty_ReturnsNull()
    {
        var container = Newtonsoft.Json.JsonConvert.DeserializeObject<CriteriaContainer>(
            "{\"Criteria\": null}", GetSettings());

        Assert.Null(container!.Criteria);
    }

    private static Newtonsoft.Json.JsonSerializerSettings GetSettings() => null!;

    private sealed class CriteriaContainer
    {
        [Newtonsoft.Json.JsonConverter(typeof(JsonSafeCriteriaConverter))]
        public BaseCriteria? Criteria { get; set; }
    }
}
