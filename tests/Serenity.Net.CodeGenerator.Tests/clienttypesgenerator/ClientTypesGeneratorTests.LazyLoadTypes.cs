namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    const string MarkedEditor = /*lang=typescript*/ """
        import { Widget, LazyLoadInclude } from "@serenity-is/corelib";
        export class MarkedEditor extends Widget<any> implements LazyLoadInclude {
            static [Symbol.typeInfo] = this.registerEditor("MyProject.MarkedEditor");
        }
        export class PlainEditor extends Widget<any> {
            static [Symbol.typeInfo] = this.registerEditor("MyProject.PlainEditor");
        }
        export class DerivedEditor extends MarkedEditor {
            static [Symbol.typeInfo] = this.registerEditor("MyProject.DerivedEditor");
        }
        export class AnotherEditor extends Widget<any> implements LazyLoadInclude {
            static [Symbol.typeInfo] = this.registerEditor("MyProject.AnotherEditor");
        }
        """;

    [Fact]
    public void Declares_Only_The_Types_Directly_Implementing_The_Marker()
    {
        var code = Read(Generate(null, true, ("Modules/MarkedEditor.ts", MarkedEditor)), "LazyLoadTypes.cs");

        Assert.Contains("[assembly: global::Serenity.Web.LazyLoadTypes(\"~/esm/Modules/ServerTypes/LazyTypeLoader.js\", " +
            "\"MyProject.AnotherEditor\", \"MyProject.MarkedEditor\")]", code);
        Assert.DoesNotContain("PlainEditor", code);
        Assert.DoesNotContain("DerivedEditor", code);
    }

    [Fact]
    public void Declares_The_Types_Of_A_Lazy_Group()
    {
        var code = Read(Generate(null, true, ("Modules/GroupedEditor.ts", /*lang=typescript*/ """
            import { Widget, LazyLoadInclude } from "@serenity-is/corelib";
            export class GroupedEditor extends Widget<any> implements LazyLoadInclude<"Viewer"> {
                static [Symbol.typeInfo] = this.registerEditor("MyProject.GroupedEditor");
            }
            """)), "LazyLoadTypes.cs");

        Assert.Contains("\"MyProject.GroupedEditor\"", code);
    }

    [Fact]
    public void Does_Not_Generate_LazyLoadTypes_Without_A_Marked_Type()
    {
        var files = Generate(null, true, ("Modules/PlainEditor.ts", /*lang=typescript*/ """
            import { Widget } from "@serenity-is/corelib";
            export class PlainEditor extends Widget<any> {
                static [Symbol.typeInfo] = this.registerEditor("MyProject.PlainEditor");
            }
            """));

        Assert.DoesNotContain(files, x => x.Filename == "LazyLoadTypes.cs");
    }
}
