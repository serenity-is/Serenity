using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class GeneratorConfigTests
{
    [Fact]
    public void Extends_WorksProperly_With_Nested_Objects()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(@"/a/b/c");

        fileSystem.WriteAllText(@"/a/sergen.a.json",
            """""
            {
                "RootNamespace": "A",
                "DeclareJoinConstants": false,
                "IncludeGlobalUsings": ["IA"],
                "OmitDefaultSchema": true,
                "ServerTypings": {
                    "LocalTexts": true,
                    "OutDir": "OA"
                }
            }
            """"");

        fileSystem.WriteAllText(@"/a/b/sergen.b.json",
            """""
            {
                "Extends": "../sergen.a.json",
                "RootNamespace": "B",
                "DeclareJoinConstants": true,
                "IncludeGlobalUsings": ["IB"],
                "ExcludeGlobalUsings": ["XB"],
                "SaveGeneratedTables": false,
                "ServerTypings": {
                    "ModuleTypings": false
                }
            }
            """"");

        fileSystem.WriteAllText(@"/a/b/c/sergen.json",
            """""
            {
                "Extends": "../sergen.b.json",
                "RootNamespace": "C",
                "DeclareJoinConstants": null,
                "IncludeGlobalUsings": ["IC"],
                "ServerTypings": {
                    "LocalTexts": false,
                    "ModuleTypings": null
                }
            }
            """"");

        var config = fileSystem.LoadGeneratorConfig(@"/a/b/c");

        Assert.Equal("../sergen.b.json", config.Extends);
        Assert.Equal("C", config.RootNamespace);
        Assert.Null(config.DeclareJoinConstants);
        Assert.Equal(new[] { "IC" }, config.IncludeGlobalUsings);
        Assert.Equal(new[] { "XB" }, config.ExcludeGlobalUsings);
        Assert.False(config.SaveGeneratedTables);
        Assert.True(config.OmitDefaultSchema);
        Assert.NotNull(config.ServerTypings);
        Assert.False(config.ServerTypings.LocalTexts);
        Assert.Null(config.ServerTypings.ModuleTypings);
        Assert.Equal("OA", config.ServerTypings.OutDir);
    }

    [Fact]
    public void Extends_Can_Load_Defaults()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(@"/a/b/c");

        fileSystem.WriteAllText(@"/a/b/sergen.b.json",
            """""
            {
                "Extends": "defaults@6.6.0",
                "RootNamespace": "B",
                "IncludeGlobalUsings": ["IB"],
                "ExcludeGlobalUsings": ["XB"],
                "ServerTypings": {
                    "ModuleTypings": false
                }
            }
            """"");

        fileSystem.WriteAllText(@"/a/b/c/sergen.json",
            """""
            {
                "Extends": "../sergen.b.json",
                "RootNamespace": "C",
                "IncludeGlobalUsings": ["IC"],
                "ServerTypings": {
                    "ModuleTypings": null
                }
            }
            """"");

        var config = fileSystem.LoadGeneratorConfig(@"/a/b/c");

        Assert.Equal("../sergen.b.json", config.Extends);
        Assert.Equal("C", config.RootNamespace);
        Assert.True(config.DeclareJoinConstants);
        Assert.True(config.FileScopedNamespaces);
        Assert.False(config.SaveGeneratedTables);
        Assert.True(config.OmitDefaultSchema);
        Assert.True(config.ServerTypings.LocalTexts);

        Assert.Equal(GeneratorConfig.FieldSelection.NameOnly, config.ForeignFieldSelection);
        Assert.Equal(new[] { "IC" }, config.IncludeGlobalUsings);
        Assert.Equal(new[] { "XB" }, config.ExcludeGlobalUsings);
        Assert.NotNull(config.ServerTypings);
        Assert.Null(config.ServerTypings.ModuleTypings);
    }

}
