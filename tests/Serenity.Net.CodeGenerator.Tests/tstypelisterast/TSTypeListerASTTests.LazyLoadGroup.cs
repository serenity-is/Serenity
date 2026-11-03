using Serenity.Reflection;

namespace Serenity.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Reads_The_Lazy_Load_Group_Without_Changing_The_Interface_Name()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        var file = root + "Modules/Test/MyEditors.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(file));
        fileSystem.WriteAllText(file, /*lang=typescript*/ """
            import { LazyLoadInclude } from "@serenity-is/corelib";

            export class GroupedEditor implements LazyLoadInclude<"Viewer"> {
            }

            export class UngroupedEditor implements LazyLoadInclude {
            }

            export class PlainEditor {
            }
            """);

        var typeLister = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
            CompilerOptions = new TSConfig.CompilerConfig { Module = "ESNext" }
        });
        typeLister.AddInputFile(file);

        var types = typeLister.ExtractTypes();

        var grouped = Assert.Single(types, x => x.Name == "GroupedEditor");
        Assert.Equal("Viewer", grouped.LazyLoadGroup);
        Assert.EndsWith("LazyLoadInclude", Assert.Single(grouped.Interfaces!), StringComparison.Ordinal);
        Assert.DoesNotContain("Viewer", grouped.Interfaces![0], StringComparison.Ordinal);

        Assert.Null(Assert.Single(types, x => x.Name == "UngroupedEditor").LazyLoadGroup);
        Assert.Null(Assert.Single(types, x => x.Name == "PlainEditor").LazyLoadGroup);
    }
}
