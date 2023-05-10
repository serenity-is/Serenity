using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void ModuleDTS_Extracts_StringEditor_Without_Decorators()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"
declare class Widget<TOptions = any> {
}

declare class StringEditor extends Widget<any> {
}

export { Widget, StringEditor }
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { StringEditor } from '@serenity-is/corelib';

export class Type1 extends StringEditor {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: new TSConfig
        {
            CompilerOptions = new()
            {
                Module = "ESNext"
            }
        });
        tl.AddInputFile(testTS);

        var types = tl.ExtractTypes();
        Assert.Collection(types.OrderBy(x => x.FullName, StringComparer.Ordinal),
            x =>
            {
                Assert.Equal("/test:Type1", x.FullName);
                Assert.Equal("@serenity-is/corelib:StringEditor", x.BaseType);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:StringEditor", x.FullName);
                // can't resolve type refs in same module yet
                Assert.Equal("Widget", x.BaseType);
                Assert.True(x.IsDeclaration);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
                Assert.True(x.IsDeclaration);
            });
    }

    [Fact]
    public void ModuleDTS_Extracts_StringEditor_Without_Decorators_But_Static_TypeName()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"
declare class Widget<TOptions = any> {
}

declare class StringEditor extends Widget<any> {
    static readonly __typeName = ""Serenity.StringEditor"";
    static readonly __bool = false;
}

export { Widget, StringEditor }
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { StringEditor } from '@serenity-is/corelib';

export class Type1 extends StringEditor {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: new TSConfig
        {
            CompilerOptions = new()
            {
                Module = "ESNext"
            }
        });
        tl.AddInputFile(testTS);

        var types = tl.ExtractTypes();
        Assert.Collection(types.OrderBy(x => x.FullName, StringComparer.Ordinal),
            x =>
            {
                Assert.Equal("/test:Type1", x.FullName);
                Assert.Equal("@serenity-is/corelib:StringEditor", x.BaseType);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:StringEditor", x.FullName);
                // can't resolve type refs in same module yet
                Assert.Equal("Widget", x.BaseType);
                Assert.True(x.IsDeclaration);
                Assert.Collection(x.Fields.OrderBy(x => x.Name, StringComparer.Ordinal), 
                    x =>
                    {
                        Assert.True(x.IsStatic);
                        Assert.Equal("__bool", x.Name);
                        Assert.False(x.Value as bool?);
                    },
                    x =>
                    {
                        Assert.True(x.IsStatic);
                        Assert.Equal("__typeName", x.Name);
                        Assert.Equal("Serenity.StringEditor", x.Value);
                    });
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
                Assert.True(x.IsDeclaration);
            });
    }
}
