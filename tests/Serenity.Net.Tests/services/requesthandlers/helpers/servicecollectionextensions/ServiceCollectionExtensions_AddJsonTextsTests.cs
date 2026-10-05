namespace Serenity.Extensions.DependencyInjection;

public class ServiceCollectionExtensions_AddJsonTextsTests
{
    [Fact]
    public void AddJsonTexts_Throws_For_Nulls()
    {
        var provider = new MockFileProvider();

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(null, provider, "texts"));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(new MockLocalTextRegistry(), null, "texts"));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(new MockLocalTextRegistry(), provider, null));
    }

    [Fact]
    public void AddJsonTexts_Returns_Registry_When_Directory_Does_Not_Exist()
    {
        var registry = new MockLocalTextRegistry();
        var provider = new MockFileProvider();

        Assert.Same(registry, ServiceCollectionExtensions.AddJsonTexts(registry, provider, "missing"));
        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddJsonTexts_Handles_Simple_Dictionary()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "{\"x\":\"5\", \"y.z\": \"a.b.c\"}");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("es", "x", "5"), x),
            x => Assert.Equal(("es", "y.z", "a.b.c"), x));
    }

    [Fact]
    public void AddJsonTexts_Handles_Recursive_Subdirectories()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "{\"a\":\"1\"}");
        fileSystem.AddFile("/Testing/Test/texts/sub/my.de.json", "{\"b\":\"2\"}");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Collection(registry.AddedList.OrderBy(x => x.languageID),
            x => Assert.Equal(("de", "b", "2"), x),
            x => Assert.Equal(("es", "a", "1"), x));
    }

    [Fact]
    public void AddJsonTexts_Ignores_Files_Without_Language_Id()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/random.json", "{\"x\":\"5\"}");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddJsonTexts_Ignores_Non_Json_Files()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.txt", "{\"x\":\"5\"}");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddJsonTexts_Ignores_Files_With_Empty_Content()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "   ");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddJsonTexts_Ignores_Files_With_Null_Json()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "null");

        var registry = new MockLocalTextRegistry();
        ServiceCollectionExtensions.AddJsonTexts(registry, new MockFileProvider(fileSystem: fileSystem), "texts");

        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddJsonTexts_Throws_With_File_Name_For_Malformed_Json()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "{ \"x\": }");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(new MockLocalTextRegistry(),
                new MockFileProvider(fileSystem: fileSystem), "texts"));

        Assert.Contains("my.es.json", ex.Message);
    }

    [Fact]
    public void AddJsonTexts_Throws_With_File_Name_For_Non_Dictionary_Json()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/Testing/Test/texts/my.es.json", "[]");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(new MockLocalTextRegistry(),
                new MockFileProvider(fileSystem: fileSystem), "texts"));

        Assert.Contains("my.es.json", ex.Message);
    }
}
