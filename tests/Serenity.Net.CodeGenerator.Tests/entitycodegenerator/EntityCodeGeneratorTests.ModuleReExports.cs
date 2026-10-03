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
    public void Run_Generate_Dotted_Module_Uses_Consistent_Typescript_Paths()
    {
        Create(out var fileSystem, out var templates, out var writer, out var model, out var config);
        model.Module = "Some.Module";
        config.GenerateRow = true;
        config.GenerateService = true;
        config.GenerateUI = true;
        var generator = new EntityCodeGenerator(new MockProjectFileInfo(fileSystem), model,
            config, templates, writer);

        generator.Run();

        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/Some/Module/Customer/CustomerDialog.tsx"));
        var dialog = fileSystem.ReadAllText("/app/Modules/Some/Module/Customer/CustomerDialog.tsx");
        Assert.Contains("from '../../../ServerTypes/Some/Module'", dialog, StringComparison.Ordinal);
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/ServerTypes/Some/Module/CustomerRow.ts"));

        var moduleIndex = fileSystem.ReadAllText("/app/Modules/ServerTypes/Some/Module.ts");
        Assert.Contains("export * from \"./Module/CustomerRow\"", moduleIndex, StringComparison.Ordinal);
        var service = fileSystem.ReadAllText("/app/Modules/ServerTypes/Some/Module/CustomerService.ts");
        Assert.Contains("baseUrl = 'Some/Module/Customer'", service, StringComparison.Ordinal);
        Assert.Contains("Some/Module/Customer/Create", service, StringComparison.Ordinal);
        var navigation = "/app/Modules/Some/Module/Some.ModuleNavigation.cs";
        Assert.Contains("\"Some/Module/Customer\"", fileSystem.ReadAllText(navigation), StringComparison.Ordinal);
        Assert.Equal("Some/Module/Customer", model.ServiceBaseUrl);
        Assert.Equal("Some/Module/Customer", model.ViewPageRoute);
        Assert.Equal("@/Some/Module/Customer/CustomerPage", model.ViewPageModulePath);
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
