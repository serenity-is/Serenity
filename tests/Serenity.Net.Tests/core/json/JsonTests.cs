namespace Serenity;

public class JsonTests
{
    private class TestDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void Parse_Generic_Deserializes()
    {
        var dto = JSON.Parse<TestDto>("{\"Name\":\"test\",\"Value\":42}");
        Assert.Equal("test", dto.Name);
        Assert.Equal(42, dto.Value);
    }

    [Fact]
    public void Parse_WithType_Deserializes()
    {
        var dto = (TestDto)JSON.Parse("{\"Name\":\"test\",\"Value\":42}", typeof(TestDto));
        Assert.Equal("test", dto.Name);
    }

    [Fact]
    public void Parse_WithOptions_UsesOptions()
    {
        var dto = JSON.Parse<TestDto>("{\"Name\":\"test\"}", JSON.Defaults.Tolerant);
        Assert.Equal("test", dto.Name);
    }

    [Fact]
    public void ParseTolerant_Generic_Deserializes()
    {
        var dto = JSON.ParseTolerant<TestDto>("{\"Name\":\"test\",\"Value\":42}");
        Assert.Equal("test", dto.Name);
    }

    [Fact]
    public void ParseTolerant_WithType_Deserializes()
    {
        var dto = (TestDto)JSON.ParseTolerant("{\"Name\":\"test\",\"Value\":42}", typeof(TestDto));
        Assert.Equal("test", dto.Name);
    }

    [Fact]
    public void Stringify_Serializes()
    {
        var json = JSON.Stringify(new TestDto { Name = "test", Value = 42 });
        Assert.Contains("test", json);
        Assert.Contains("42", json);
    }

    [Fact]
    public void Stringify_WithWriteNulls_IncludesNulls()
    {
        var json = JSON.Stringify(new TestDto { Name = null, Value = 42 }, writeNulls: true);
        Assert.Contains("\"Name\":null", json);
    }

    [Fact]
    public void Stringify_WithoutWriteNulls_SkipsNulls()
    {
        var json = JSON.Stringify(new TestDto { Name = null, Value = 42 }, writeNulls: false);
        Assert.DoesNotContain("\"Name\":null", json);
    }

    [Fact]
    public void Stringify_WithOptions_Serializes()
    {
        var json = JSON.Stringify(new TestDto { Name = "test" }, JSON.Defaults.Strict);
        Assert.Contains("test", json);
    }

    [Fact]
    public void Stringify_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => JSON.Stringify("test", null));
    }

    [Fact]
    public void ToJson_Serializes()
    {
        var json = new TestDto { Name = "test" }.ToJson();
        Assert.Contains("test", json);
    }

    [Fact]
    public void ToJson_WithWriteNulls_IncludesNulls()
    {
        var json = new TestDto { Name = null }.ToJson(writeNulls: true);
        Assert.Contains("\"Name\":null", json);
    }

    [Fact]
    public void PopulateObject_PopulatesExistingObject()
    {
        var dto = new TestDto { Name = "old", Value = 1 };
        JSON.PopulateObject(dto, "{\"Name\":\"new\",\"Value\":42}", JSON.Defaults.Tolerant);
        Assert.Equal("new", dto.Name);
        Assert.Equal(42, dto.Value);
    }

    [Fact]
    public void Defaults_Populate_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => JSON.Defaults.Populate(null));
    }
}