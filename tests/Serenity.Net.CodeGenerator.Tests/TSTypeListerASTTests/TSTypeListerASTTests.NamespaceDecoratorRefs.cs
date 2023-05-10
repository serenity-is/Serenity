using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Resolves_Namespace_DecoratorRefs_InSerenityNamespace()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.WriteAllText("a.ts", @"
declare namespace Serenity {
    export class Widget {
    }

    export namespace Decorators {
        export function registerEditor();
    }
}


namespace Serenity.Sub {

    @Decorators.registerEditor()
    export class B extends Serenity.Widget {
    }
}");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: null);
        tl.AddInputFile("a.ts");

        var types = tl.ExtractTypes();
        var b = Assert.Single(types, x => x.FullName == "Serenity.Sub.B");
        Assert.Single(b.Attributes, x => x.Type == "Serenity.Decorators.registerEditor");
    }

}
