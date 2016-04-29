using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Html;
using System;

namespace Serenity
{
    [Imported, Serializable, ScriptNamespace("jsPDF")]
    public class AutoTableStyles
    {
        public double? CellPadding { get; set; }
        public double? FontSize { get; set; }
        public string Font { get; set; }
        public TypeOption<double, double[]> LineColor { get; set; }
        public double? LineWidth { get; set; }
        public double? LineHeight { get; set; }
        public string FontStyle { get; set; }
        public TypeOption<double?, double[]> FillColor { get; set; }
        public TypeOption<double?, double[]> TextColor { get; set; }
        public string Halign { get; set; }
        public string Valign { get; set; }
        public string FillStyle { get; set; }
        public double RowHeight { get; set; }
        public TypeOption<string, double> ColumnWidth { get; set; }
        public string Overflow { get; set; }
    }

    [Imported, Serializable, ScriptNamespace("jsPDF")]
    public class AutoTableOptions
    {
        public string TableWidth { get; set; }
        public string Theme { get; set; }
        public double? StartY { get; set; }
        public AutoTableStyles Styles { get; set; }
        public AutoTableStyles HeaderStyles { get; set; }
        public AutoTableStyles BodyStyles { get; set; }
        public JsDictionary<string, AutoTableStyles> ColumnStyles { get; set; }
        public AutoTableMargin Margin { get; set; }
        public Action<object[]> BeforePageContent { get; set; }
        public Action<object[]> AfterPageContent { get; set; }
    }


    [Imported, Serializable, ScriptNamespace("jsPDF")]
    public class AutoTableColumn
    {
        public string Title { get; set; }
        public string DataKey { get; set; }
    }

    [Imported, Serializable, ScriptNamespace("jsPDF")]
    public class AutoTableMargin
    {
        public double? Horizontal { get; set; }
        public double? Top { get; set; }
        public double? Left { get; set; }
        public double? Right { get; set; }
        public double? Bottom { get; set; }
    }

    [IgnoreNamespace, Imported]
    public class jsPDF
    {
        public static double autoTableEndPosY;

        public static object[] AutoTableHtmlToJson(HtmlElement table)
        {
            return null;
        }

        public static void AutoTable(TypeOption<List<string>, List<AutoTableColumn[]>> columns,
            List<object> data, AutoTableOptions options)
        {
        }

        public static void AutoTableText(string text, double x, double y, AutoTableStyles styles = null)
        {
        }
    }
}