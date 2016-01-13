
namespace Serenity.Reporting
{
    using System.Collections.Generic;

    public interface IHtmlToPdfOptions
    {
        string Url { get; set; }
        List<string> AdditionalUrls { get; }
        Dictionary<string, string> Cookies { get; }
        int TimeoutSeconds { get; set; }
        bool UsePrintMediaType { get; set; }
        bool PrintBackground { get; set; }
        string UtilityExePath { get; set; }
        string PageSize { get; set; }
        bool SmartShrinking { get; set; }
        int? Dpi { get; set; }
        bool Landscape { get; set; }
        string Zoom { get; set; }
        string MarginsAll { set; }
        string MarginLeft { get; set; }
        string MarginRight { get; set; }
        string MarginBottom { get; set; }
        string MarginTop { get; set; }
        string FooterHtmlUrl { get; set; }
        Dictionary<string, string> FooterHeaderReplace { get; }
        List<string> CustomArgs { get; }
    }
}