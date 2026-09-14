namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    [Fact]
    public void Run_GenerateCustom_Renders_Template_Overridden_File()
    {
        var generator = Create(out var fs, out var writer, out _, out var config);
        config.GenerateCustom = true;
        AddFile(fs, "/custom/Cmd/MyTemplate.scriban", "CUSTOM-OUTPUT");
        config.CustomGenerate = new Dictionary<string, string>()
        {
            ["Cmd/MyTemplate"] = "{4}/Output/{0}_{1}.txt"
        };

        try
        {
            Templates.TemplatePath = "/custom";
            generator.Run();
        }
        finally
        {
            Templates.TemplatePath = null;
        }

        var output = "/app/Output/Customer_TestModule.txt";
        Assert.Contains(writer.Files, x => x.EndsWith("/Output/Customer_TestModule.txt"));
        Assert.Contains("CUSTOM-OUTPUT", fs.ReadAllText(output));
    }

    [Fact]
    public void Run_Skips_CustomGenerate_Entries_With_Empty_Value()
    {
        var generator = Create(out var fs, out var writer, out _, out var config);
        config.GenerateCustom = true;
        config.CustomGenerate = new Dictionary<string, string>()
        {
            ["Row"] = null!,
            ["Row"] = ""
        };

        generator.Run();

        Assert.Empty(writer.Files);
    }

    [Fact]
    public void Run_Skips_CustomGenerate_When_GenerateCustom_Is_False()
    {
        var generator = Create(out var fs, out var writer, out _, out var config);
        config.GenerateCustom = false;
        config.CustomGenerate = new Dictionary<string, string>()
        {
            ["Row"] = "Output/Row.cs"
        };

        generator.Run();

        Assert.Empty(writer.Files);
    }

    [Fact]
    public void Run_Throws_For_Insecure_Custom_Template_Key()
    {
        var generator = Create(out _, out var writer, out _, out var config);
        config.GenerateCustom = true;
        config.CustomGenerate = new Dictionary<string, string>()
        {
            ["../evil"] = "Output/Row.cs"
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => generator.Run());
        Assert.Empty(writer.Files);
    }

    [Fact]
    public void Run_Throws_For_Insecure_Custom_Output_File()
    {
        var generator = Create(out _, out var writer, out _, out var config);
        config.GenerateCustom = true;
        config.CustomGenerate = new Dictionary<string, string>()
        {
            ["Row"] = "Output/../Row.cs"
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => generator.Run());
        Assert.Empty(writer.Files);
    }
}
