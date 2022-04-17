using Serenity.CodeGenerator;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public class TSTypeListerTests
    {
        [Fact]
        public void Resolves_Type_Refs_In_Same_Namespace_Same_File()
        {
            var tl = new TSTypeListerAST();
            tl.AddInputFile("a.d.ts", @"
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

            var types = tl.ExtractTypes();
            var type = Assert.Single(types, x => x.FullName == "jsPDF.AutoTableOptions");
            var styles = Assert.Single(type.Fields, x => x.Name == "styles");
            Assert.Equal("jsPDF.AutoTableStyles", styles.Type);
            var columnStyles = Assert.Single(type.Fields, x => x.Name == "columnStyles");
            Assert.Null(columnStyles.Type);
            var margin = Assert.Single(type.Fields, x => x.Name == "margin");
            Assert.Equal("jsPDF.AutoTableMargin", margin.Type);
        }
    }
}
