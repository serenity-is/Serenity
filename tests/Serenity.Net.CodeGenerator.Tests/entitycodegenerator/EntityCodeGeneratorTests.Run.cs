namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    [Fact]
    public void Ctor_Throws_For_Null_Project()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityCodeGenerator(null!,
            new EntityModel(), new GeneratorConfig(), new MockGeneratedFileWriter(new MockFileSystem())));
    }

    [Fact]
    public void Ctor_Throws_For_Null_Writer()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityCodeGenerator(
            new MockProjectFileInfo(new MockFileSystem()),
            new EntityModel(), new GeneratorConfig(), null!));
    }

    [Fact]
    public void Ctor_Throws_For_Null_Model()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityCodeGenerator(
            new MockProjectFileInfo(new MockFileSystem()),
            null!, new GeneratorConfig(), new MockGeneratedFileWriter(new MockFileSystem())));
    }

    [Fact]
    public void Run_Generates_Nothing_When_All_Options_Are_Disabled()
    {
        var generator = Create(out _, out var writer, out _, out _);

        generator.Run();

        Assert.Empty(writer.Files);
    }

    [Fact]
    public void Run_GenerateRow_Writes_Row_And_Row_Typing()
    {
        var generator = Create(out var fs, out var writer, out var model, out var config);
        config.GenerateRow = true;

        generator.Run();

        Assert.Equal(2, writer.Files.Count);
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerRow.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/ServerTypes/TestModule/CustomerRow.ts"));
        Assert.Contains("class " + model.RowClassName + " : ",
            fs.ReadAllText(writer.Files.First(x => x.EndsWith("CustomerRow.cs"))));
        Assert.NotEmpty(fs.ReadAllText(writer.Files.First(x => x.EndsWith("CustomerRow.ts"))));
    }

    [Fact]
    public void Run_GenerateService_Writes_Handlers_Endpoint_And_Service_Typing()
    {
        var generator = Create(out _, out var writer, out _, out var config);
        config.GenerateService = true;

        generator.Run();

        Assert.Equal(6, writer.Files.Count);
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/RequestHandlers/CustomerDeleteHandler.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/RequestHandlers/CustomerListHandler.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/RequestHandlers/CustomerRetrieveHandler.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/RequestHandlers/CustomerSaveHandler.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerEndpoint.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/ServerTypes/TestModule/CustomerService.ts"));
    }

    [Fact]
    public void Run_GenerateUI_Writes_UI_Files_And_Navigation()
    {
        var generator = Create(out _, out var writer, out _, out var config);
        config.GenerateUI = true;

        generator.Run();

        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerColumns.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerForm.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerPage.cs"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerDialog.tsx"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerGrid.tsx"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/TestModule/Customer/CustomerPage.tsx"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/ServerTypes/TestModule/CustomerForm.ts"));
        Assert.Contains(writer.Files, x => x.EndsWith("/Modules/ServerTypes/TestModule/CustomerColumns.ts"));

        var navigationFile = "/app/Modules/TestModule/TestModuleNavigation.cs";
        Assert.True(writer.FileSystem.FileExists(navigationFile));

        var bytes = writer.FileSystem.ReadAllBytes(navigationFile);
        var preamble = Encoding.UTF8.GetPreamble();
        Assert.True(bytes.AsSpan(0, preamble.Length).SequenceEqual(preamble));
        var navigationCode = Encoding.UTF8.GetString(bytes[preamble.Length..]);
        Assert.Contains("MyPages.CustomerPage", navigationCode, StringComparison.Ordinal);
    }
}
