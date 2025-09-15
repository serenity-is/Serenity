
namespace Serenity.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Extract_TypeName_From_Decorator()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"

export class Widget<TOptions = any> {
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
    static [Symbol.typeInfo]: ClassTypeInfo<""Serenity.Widget"">;
}
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { Widget, Decorators } from '@serenity-is/corelib';

@Decorators.registerClass(""MyNamespace."")
export class Type1 extends Widget {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: fileSystem.Directory.GetCurrentDirectory(),
            tsConfig: new TSConfig
            {
                CompilerOptions = new()
                {
                    Module = "bundler"
                }
            });
        tl.AddInputFile(testTS);

        var types = tl.ExtractTypes();
        Assert.Collection(types.OrderBy(x => x.FullName, StringComparer.Ordinal),
            x =>
            {
                Assert.Equal("/test:Type1", x.FullName);
                Assert.Equal("@serenity-is/corelib:Widget", x.BaseType);
                var f = Assert.Single(x.Attributes);
                Assert.Equal("@serenity-is/corelib:Decorators.registerClass", f.Type);
                Assert.Equal("MyNamespace.", Assert.Single(f.Arguments)?.Value);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
            });
    }

    [Fact]
    public void Extract_TypeName_Imported_Constant_From_Decorator()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"

export class Widget<TOptions = any> {
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
    static [Symbol.typeInfo]: ClassTypeInfo<""Serenity.Widget"">;
}
export const nsMyNamespace: ""MyNamespace."" = ""MyNamespace."";
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { Widget, Decorators, nsMyNamespace } from '@serenity-is/corelib';

@Decorators.registerClass(nsMyNamespace)
export class Type1 extends Widget {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: fileSystem.Directory.GetCurrentDirectory(),
            tsConfig: new TSConfig
            {
                CompilerOptions = new()
                {
                    Module = "bundler"
                }
            });
        tl.AddInputFile(testTS);

        var types = tl.ExtractTypes();
        Assert.Collection(types.OrderBy(x => x.FullName, StringComparer.Ordinal),
            x =>
            {
                Assert.Equal("/test:Type1", x.FullName);
                Assert.Equal("@serenity-is/corelib:Widget", x.BaseType);
                var f = Assert.Single(x.Attributes);
                Assert.Equal("@serenity-is/corelib:Decorators.registerClass", f.Type);
                Assert.Equal("MyNamespace.", Assert.Single(f.Arguments)?.Value);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
            });
    }
}
