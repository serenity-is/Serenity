using Serenity.Localization;

namespace Serenity.Tests.Localization;

public class JsonLocalTextRegistrationTests
{
    [Fact]
    public void AddFromNestedDictionary_ThrowsArgumentNull_If_Nested_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            JsonLocalTextRegistration.AddFromNestedDictionary(null, "x", "en", registry: new MockLocalTextRegistry());
        });
    }

    [Fact]
    public void AddFromNestedDictionary_ThrowsKeyNotFound_If_Registry_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            JsonLocalTextRegistration.AddFromNestedDictionary(nested: new Dictionary<string, object>(), 
                "pre", "en", registry: null);
        });
    }

    [Fact]
    public void AddFromNestedDictionary_Handles_SimpleDictionary()
    {
        var registry = new MockLocalTextRegistry();
        var dict = JSON.Parse<Dictionary<string, object>>(@"{""x"":""5"", ""y.z"": ""a.b.c""}");
        JsonLocalTextRegistration.AddFromNestedDictionary(dict, "pre.", "es", registry);
        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("es", "pre.x", "5"), x),
            x => Assert.Equal(("es", "pre.y.z", "a.b.c"), x));
    }

    [Fact]
    public void AddFromNestedDictionary_Handles_HierarchicalDictionary()
    {
        var registry = new MockLocalTextRegistry();
        var dict = JSON.Parse<Dictionary<string, object>>(@"{x:""x"",y:{z:{u:{l:""l"",m:""m""},t:""t""}}}");
        JsonLocalTextRegistration.AddFromNestedDictionary(dict, "Db.", "jp", registry);
        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("jp", "Db.x", "x"), x),
            x => Assert.Equal(("jp", "Db.y.z.t", "t"), x),
            x => Assert.Equal(("jp", "Db.y.z.u.l", "l"), x),
            x => Assert.Equal(("jp", "Db.y.z.u.m", "m"), x));
    }

    [Fact]
    public void AddFromNestedDictionary_Skips_NullValues()
    {
        var registry = new MockLocalTextRegistry();
        var dict = JSON.Parse<Dictionary<string, object>>(@"{x:""x"",y:null}");
        JsonLocalTextRegistration.AddFromNestedDictionary(dict, "Db.", "jp", registry);
        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("jp", "Db.x", "x"), x));
    }

    [Fact]
    public void ProcessNestedDictionary_ThrowsArgumentNull_If_Nested_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            JsonLocalTextRegistration.ProcessNestedDictionary<object>(nested: null, "x", new Dictionary<string, string>());
        });
    }

    [Fact]
    public void JsonLocalTextRegistration_ProcessNestedDictionary_HandlesSimpleDictionary()
    {
        var dict = JSON.Parse<Dictionary<string, object>>(@"{x:""5"", ""y.z"": ""a.b.c""}");
        var target = new Dictionary<string, string>();
        JsonLocalTextRegistration.ProcessNestedDictionary(dict, "pre.", target);

        Assert.Equal(2, target.Count);

        Assert.True(target.ContainsKey("pre.x"));
        Assert.Equal("5", target["pre.x"]);

        Assert.True(target.ContainsKey("pre.y.z"));
        Assert.Equal("a.b.c", target["pre.y.z"]);
    }

    [Fact]
    public void JsonLocalTextRegistration_ProcessNestedDictionary_HandlesHierarchicalDictionary()
    {
        var dict = JSON.Parse<Dictionary<string, object>>(@"{x:""x"",y:{z:{u:{l:""l"",m:""m""},t:""t""}}}");
        var target = new Dictionary<string, string>();
        JsonLocalTextRegistration.ProcessNestedDictionary(dict, "Db.", target);

        Assert.Equal(4, target.Count);

        Assert.True(target.ContainsKey("Db.x"));
        Assert.Equal("x", target["Db.x"]);

        Assert.True(target.ContainsKey("Db.y.z.u.l"));
        Assert.Equal("l", target["Db.y.z.u.l"]);

        Assert.True(target.ContainsKey("Db.y.z.u.m"));
        Assert.Equal("m", target["Db.y.z.u.m"]);

        Assert.True(target.ContainsKey("Db.y.z.t"));
        Assert.Equal("t", target["Db.y.z.t"]);
    }

    [Fact]
    public void ProcessNestedDictionary_Skips_NullValues()
    {
        var dict = JSON.Parse<Dictionary<string, object>>(@"{x:""x"",y:null}");
        var target = new Dictionary<string, string>();
        JsonLocalTextRegistration.ProcessNestedDictionary(dict, "Db.", target);

        Assert.StrictEqual(1, target.Count);

        Assert.True(target.ContainsKey("Db.x"));
        Assert.Equal("x", target["Db.x"]);
    }

    [Fact]
    public void AddJsonTexts_ThrowsArgumentNull_If_Path_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => JsonLocalTextRegistration.AddJsonTexts(
            registry: new MockLocalTextRegistry(), 
            path: null, 
            fileSystem: new MockFileSystem()));
    }

    [Fact]
    public void AddJsonTexts_ThrowsArgumentNull_If_Registry_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => JsonLocalTextRegistration.AddJsonTexts(
            registry: null,
            path: null,
            fileSystem: new MockFileSystem()));
    }

    [Fact]
    public void AddJsonTexts_Ignores_IfDirectoryDoesntExist()
    {
        JsonLocalTextRegistration.AddJsonTexts(
            registry: new MockLocalTextRegistry(), 
            path: @"C:/s_o_m_e_f_o_l_d_e_r",
            fileSystem: new MockFileSystem());
    }

    [Fact]
    public void AddJsonTexts_Handles_SimpleDictionary()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(@"C:/My/my.es.json", @"{x:""5"", ""y.z"": ""a.b.c""}");
        
        var registry = new MockLocalTextRegistry();
        JsonLocalTextRegistration.AddJsonTexts(registry, @"C:/My/", fileSystem);

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("es", "x", "5"), x),
            x => Assert.Equal(("es", "y.z", "a.b.c"), x));
    }

    [Fact]
    public void AddJsonTexts_Handles_HierarchicalDictionary()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(@"C:/My/jp.json", @"{x:""x"",y:{z:{u:{l:""l"",m:""m""},t:""t""}}}");

        var registry = new MockLocalTextRegistry();
        JsonLocalTextRegistration.AddJsonTexts(registry, @"C:/My/", fileSystem);

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("jp", "x", "x"), x),
            x => Assert.Equal(("jp", "y.z.t", "t"), x),
            x => Assert.Equal(("jp", "y.z.u.l", "l"), x),
            x => Assert.Equal(("jp", "y.z.u.m", "m"), x));
    }

    [Fact]
    public void AddJsonTexts_Skips_NullValues()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(@"C:/My/jp.json", @"{x:""x"",y:null}");

        var registry = new MockLocalTextRegistry();
        JsonLocalTextRegistration.AddJsonTexts(registry, @"C:/My/", fileSystem);

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("jp", "x", "x"), x));
    }

    [Fact]
    public void AddJsonTexts_Determines_LanguageIDs_Properly()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(@"C:/My/jp.json", @"{x:""1""}");
        fileSystem.AddFile(@"C:/My/en-US.json", @"{x:""2""}");
        fileSystem.AddFile(@"C:/My/texts.en-GB.json", @"{x:""3""}");
        fileSystem.AddFile(@"C:/My/texts.en-US.en.json", @"{x:""4""}");
        fileSystem.AddFile(@"C:/My/my.some.long.prefix.tr-TR.json", @"{x:""5""}");

        var registry = new MockLocalTextRegistry();
        JsonLocalTextRegistration.AddJsonTexts(registry, @"C:/My/", fileSystem);

        Assert.Collection(registry.AddedList.OrderBy(x => x.languageID),
            x => Assert.Equal(("en", "x", "4"), x),
            x => Assert.Equal(("en-GB", "x", "3"), x),
            x => Assert.Equal(("en-US", "x", "2"), x),
            x => Assert.Equal(("jp", "x", "1"), x),
            x => Assert.Equal(("tr-TR", "x", "5"), x));
    }

    [Fact]
    public void AddJsonTexts_Sorts_Files_ByFilename()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(@"C:/My/zz.json", @"{x:""1""}");
        fileSystem.AddFile(@"C:/My/bb.json", @"{x:""2""}");
        fileSystem.AddFile(@"C:/My/tt.json", @"{x:""3""}");
        fileSystem.AddFile(@"C:/My/cc.json", @"{x:""4""}");
        fileSystem.AddFile(@"C:/My/aa.json", @"{x:""5""}");

        var registry = new MockLocalTextRegistry();
        JsonLocalTextRegistration.AddJsonTexts(registry, @"C:/My/", fileSystem);

        Assert.Collection(registry.AddedList,
            x => Assert.Equal("5", x.text),
            x => Assert.Equal("2", x.text),
            x => Assert.Equal("4", x.text),
            x => Assert.Equal("3", x.text),
            x => Assert.Equal("1", x.text));
    }
}