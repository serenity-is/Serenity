
namespace Serenity.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Extract_TypeName_From_TypeInfo()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"

export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;

export class Widget<TOptions = any> {
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
    static typeInfo: ClassTypeInfo<""Serenity.Widget"">;
}
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { Widget } from '@serenity-is/corelib';

export class Type1 extends Widget {
    static override typeInfo = this.registerClass(""MyNamespace.MyType1"");
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: fileSystem.Directory.GetCurrentDirectory(),
            tsConfig: new TSConfig
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
                Assert.Equal("@serenity-is/corelib:Widget", x.BaseType);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("MyNamespace.MyType1", f.Value);
                Assert.True(f.IsStatic);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
                Assert.True(x.IsDeclaration);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("Serenity.Widget", f.Value);
                Assert.True(f.IsStatic);
            });
    }

    [Fact]
    public void Extract_TypeName_From_Via_Import_Statement_TypeInfo()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"

export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;

export class Widget<TOptions = any> {
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName> {
    }
    static typeInfo: ClassTypeInfo<""Serenity.Widget"">;
}
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { Widget } from '@serenity-is/corelib';

export class Type1 extends Widget {
    static override typeInfo: import(""@serenity-is/corelib"").ClassTypeInfo<""MyNamespace.MyType1"">
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: fileSystem.Directory.GetCurrentDirectory(),
            tsConfig: new TSConfig
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
                Assert.Equal("@serenity-is/corelib:Widget", x.BaseType);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("MyNamespace.MyType1", f.Value);
                Assert.True(f.IsStatic);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
                Assert.True(x.IsDeclaration);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("Serenity.Widget", f.Value);
                Assert.True(f.IsStatic);
            });
    }

    [Fact]
    public void Extract_TypeName_From_Without_This_TypeInfo()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("node_modules/@serenity-is/corelib/dist/index.d.ts", @"

export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;

export class Widget<TOptions = any> {
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
    static typeInfo: ClassTypeInfo<""Serenity.Widget"">;
}
");
        fileSystem.AddFile("node_modules/@serenity-is/corelib/package.json", @"{
    ""name"": ""@serenity-is/corelib"",
    ""types"": ""dist/index.d.ts""
}");

        var testTS = "test.ts";
        fileSystem.AddFile(testTS, @"
import { Widget } from '@serenity-is/corelib';

export class Type1 extends Widget {
    static override typeInfo = classTypeInfo(""MyNamespace.MyType1""); static { registerType(this); }
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: fileSystem.Directory.GetCurrentDirectory(),
            tsConfig: new TSConfig
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
                Assert.Equal("@serenity-is/corelib:Widget", x.BaseType);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("MyNamespace.MyType1", f.Value);
                Assert.True(f.IsStatic);
            },
            x =>
            {
                Assert.Equal("@serenity-is/corelib:Widget", x.FullName);
                Assert.True(x.IsDeclaration);
                var f = Assert.Single(x.Fields);
                Assert.Equal("typeInfo", f.Name);
                Assert.Equal("ClassTypeInfo", f.Type);
                Assert.Equal("Serenity.Widget", f.Value);
                Assert.True(f.IsStatic);
            });
    }
}
