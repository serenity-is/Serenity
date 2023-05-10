using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Resolves_Type_Refs_In_Same_Namespace_Same_File()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.WriteAllText("a.d.ts", @"
declare namespace jsPDF {
    interface AutoTableOptions {
        styles?: AutoTableStyles;
        columnStyles?: { [dataKey: string]: AutoTableStyles; };
        margin?: AutoTableMargin;
    }

    interface AutoTableMargin {
        bottom?: number;
    }

    interface AutoTableStyles {
        cellPadding?: number;
    }
}");
        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", null);
        tl.AddInputFile("a.d.ts");

        var types = tl.ExtractTypes();
        var type = Assert.Single(types, x => x.FullName == "jsPDF.AutoTableOptions");
        var styles = Assert.Single(type.Fields, x => x.Name == "styles");
        Assert.Equal("jsPDF.AutoTableStyles", styles.Type);
        var columnStyles = Assert.Single(type.Fields, x => x.Name == "columnStyles");
        Assert.Null(columnStyles.Type);
        var margin = Assert.Single(type.Fields, x => x.Name == "margin");
        Assert.Equal("jsPDF.AutoTableMargin", margin.Type);
    }

    [Fact]
    public void Resolve_Same_Namespace_In_One_File_Multiple()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.WriteAllText("a.d.ts", @"
declare namespace Serenity.Extensions {
    interface ExcelImportRequest extends Serenity.ServiceRequest {
        FileName?: string;
    }
}
declare namespace Serenity.Extensions {
    interface ExcelImportResponse extends Serenity.ServiceResponse {
        Inserted?: number;
        Updated?: number;
        ErrorList?: string[];
    }
}");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: null);
        tl.AddInputFile("a.d.ts");

        var types = tl.ExtractTypes();
        var excelImportRequest = Assert.Single(types, x => x.FullName == "Serenity.Extensions.ExcelImportRequest");
        var excelImportResponse = Assert.Single(types, x => x.FullName == "Serenity.Extensions.ExcelImportResponse");
    }

    [Fact]
    public void BodySkipTest()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.WriteAllText("a.ts", @"namespace A {

    @Serenity.Decorators.registerEditor()
    export class B extends C {

        public getSelect2Options() {
            var selec2Options = super.getSelect2Options();

            var oldFormatResult = selec2Options.formatResult;
            var oldFormatSelection = selec2Options.formatSelection;

            var isDeletedFormat = (item: Serenity.Select2Item, html: string): string => {
                if (item?.source.IsActive == -1)
                    return `<div class='deleted-record'>${html}</div>`;
                return html;
            };

            selec2Options.formatResult = (item: Serenity.Select2Item, p2, p3, p4) => {
                var formatted = oldFormatResult ? oldFormatResult(item, p2, p3, p4) : item.text;
                return isDeletedFormat(item, formatted);
            };

            selec2Options.formatSelection = (item: Serenity.Select2Item, p2, p3) => {
                var formatted = oldFormatSelection ? oldFormatSelection(item, p2, p3) : item?.text;
                return isDeletedFormat(item, formatted);
            };

            return selec2Options;
        }
    }
}");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: "/", tsConfig: null);
        tl.AddInputFile("a.ts");

        var types = tl.ExtractTypes();
        var b = Assert.Single(types, x => x.FullName == "A.B");
        Assert.Single(b.Methods, x => x.Name == "getSelect2Options");
    }
}
