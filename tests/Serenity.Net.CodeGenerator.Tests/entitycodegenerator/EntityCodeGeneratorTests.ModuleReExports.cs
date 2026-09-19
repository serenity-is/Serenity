namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    [Fact]
    public void Run_GenerateRow_Creates_ModuleReExport_Index()
    {
        var generator = Create(out var fileSystem, out _, out _, out _, out var config);
        config.GenerateRow = true;

        generator.Run();

        var indexFile = "/app/Modules/ServerTypes/TestModule.ts";
        var content = fileSystem.ReadAllText(indexFile).Replace("\r", "", StringComparison.Ordinal);
        Assert.Contains("export * from \"./TestModule/CustomerRow\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_Does_Not_Duplicate_ModuleReExport_Entries()
    {
        var generator = Create(out var fileSystem, out _, out _, out _, out var config);
        config.GenerateRow = true;
        config.GenerateService = true;

        generator.Run();
        var indexFile = "/app/Modules/ServerTypes/TestModule.ts";
        var afterFirst = fileSystem.ReadAllText(indexFile);
        generator.Run();
        var afterSecond = fileSystem.ReadAllText(indexFile);

        Assert.Equal(afterFirst, afterSecond);
        Assert.Contains("export * from \"./TestModule/CustomerService\"", afterFirst, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_Skips_ModuleReExport_Index_When_Disabled()
    {
        var generator = Create(out var fileSystem, out _, out _, out _, out var config);
        config.GenerateRow = true;
        config.ServerTypings = new GeneratorConfig.ServerTypingsConfig()
        {
            ModuleReExports = false
        };

        generator.Run();

        Assert.False(fileSystem.FileExists("/app/Modules/ServerTypes/TestModule.ts"));
    }

    [Fact]
    public void Run_GenerateUI_With_Empty_Module_Uses_Common_Navigation_Path()
    {
        var generator = Create(out var fileSystem, out _, out _, out var model, out var config);
        config.GenerateUI = true;
        model.Module = null;

        generator.Run();

        var navigationFile = "/app/Modules/Common/Navigation/NavigationItems.cs";
        Assert.True(fileSystem.FileExists(navigationFile));
        var text = fileSystem.ReadAllText(navigationFile);
        Assert.Contains("NavigationLink", text, StringComparison.Ordinal);
    }
}
