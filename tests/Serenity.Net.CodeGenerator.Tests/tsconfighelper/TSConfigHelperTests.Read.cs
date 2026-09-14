namespace Serenity.CodeGenerator;

public partial class TSConfigHelperTests
{
    [Fact]
    public void Read_ReturnsNull_WhenFileDoesNotExist()
    {
        var fileSystem = new MockFileSystem();

        Assert.Null(TSConfigHelper.Read(fileSystem, "/root/tsconfig.json"));
    }

    [Fact]
    public void Read_ReturnsNull_WhenFileIsInvalidJson()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", "this is not json");

        Assert.Null(TSConfigHelper.Read(fileSystem, "/root/tsconfig.json"));
    }

    [Fact]
    public void Read_Parses_CompilerOptionsAndArrays()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "module": "ESNext"
                },
                "include": ["Modules"],
                "exclude": ["node_modules"],
                "files": ["a.ts"]
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal("ESNext", config.CompilerOptions!.Module);
        Assert.Equal(["Modules"], config.Include);
        Assert.Equal(["node_modules"], config.Exclude);
        Assert.Equal(["a.ts"], config.Files);
    }

    [Fact]
    public void Read_InheritsFromRelativeExtends()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "./base/base.json"
            }
            """);
        AddFile(fileSystem, "/root/base/base.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "module": "ESNext",
                    "paths": { "@/*": ["src/*"] },
                    "rootDir": "src",
                    "types": ["node"]
                },
                "exclude": ["excluded"],
                "files": ["file.ts"],
                "include": ["Included"]
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal("ESNext", config.CompilerOptions!.Module);
        Assert.Equal(["src/*"], config.CompilerOptions.Paths!["@/*"]);
        Assert.Equal("src", config.CompilerOptions.RootDir);
        Assert.Equal(["node"], config.CompilerOptions.Types);
        Assert.Equal(["excluded"], config.Exclude);
        Assert.Equal(["file.ts"], config.Files);
        Assert.Equal(["Included"], config.Include);
    }

    [Fact]
    public void Read_InheritsFromRelativeExtends_With_NoCompilerOptions()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "./base.json"
            }
            """);
        AddFile(fileSystem, "/root/base.json", /*lang=json*/ """
            {
                "include": ["FromBase"]
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal(["FromBase"], config.Include);
        Assert.Null(config.CompilerOptions);
    }

    [Fact]
    public void Read_Finds_NonRelative_Extends_InNodeModules_WalkingUp()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/Modules/tsconfig.json", /*lang=json*/ """
            {
                "extends": "basepkg/tsconfig.json"
            }
            """);
        AddFile(fileSystem, "/root/node_modules/basepkg/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "ESNext" }
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/Modules/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal("ESNext", config.CompilerOptions!.Module);
    }

    [Fact]
    public void Read_Handles_AbsoluteExtends()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "/shared/base.json"
            }
            """);
        AddFile(fileSystem, "/shared/base.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "CommonJS" }
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal("CommonJS", config.CompilerOptions!.Module);
    }

    [Fact]
    public void Read_KeepsOwnConfig_WhenExtendsCannotBeResolved()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "missingpkg",
                "include": ["Own"]
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal(["Own"], config.Include);
        Assert.Null(config.CompilerOptions);
    }

    [Fact]
    public void Read_DoesNotSearchNodeModules_ForRootedExtends()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "/missing/base.json",
                "include": ["Own"]
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal(["Own"], config.Include);
        Assert.Null(config.CompilerOptions);
    }

    [Fact]
    public void Read_Stops_After_TenLevelsOfExtends()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/ts0.json", /*lang=json*/ """
            {
                "extends": "./ts1.json",
                "include": ["Top"]
            }
            """);

        for (var i = 1; i < 11; i++)
        {
            AddFile(fileSystem, "/root/ts" + i + ".json", $$"""
                {
                    "extends": "./ts{{i + 1}}.json"
                }
                """);
        }

        AddFile(fileSystem, "/root/ts11.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "ESNext" }
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/ts0.json");

        Assert.NotNull(config);
        Assert.Equal(["Top"], config.Include);
    }

    [Fact]
    public void Read_Resolves_TypeRootsAndConfigDir_RelativeToBaseConfig()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "extends": "base/base.json"
            }
            """);
        AddFile(fileSystem, "/root/base/base.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "rootDir": "src",
                    "typeRoots": ["${configDir}/types", "node_modules/@types", ""]
                }
            }
            """);

        var config = TSConfigHelper.Read(fileSystem, "/root/tsconfig.json");

        Assert.NotNull(config);
        Assert.Equal("src", config.CompilerOptions!.RootDir);
        Assert.Equal(["./types", "./node_modules/@types", ""], config.CompilerOptions.TypeRoots);
    }
}
