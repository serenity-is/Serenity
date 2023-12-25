using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void NamespaceDTS_Extracts_StringEditor_Without_Decorators()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("Serenity.CoreLib.d.ts", @"
declare namespace Serenity {
    class Widget<TOptions = any> {
    }

    interface Widget<TOptions> {
        change(handler: (e: Event) => void): void;
        changeSelect2(handler: (e: Event) => void): void;
    }

    class StringEditor extends Widget<any> {
        constructor(props?: any);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
}");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: null);
        tl.AddInputFile("Serenity.CoreLib.d.ts");

        var types = tl.ExtractTypes();
        var b = Assert.Single(types, x => x.FullName == "Serenity.StringEditor");
    }
}
