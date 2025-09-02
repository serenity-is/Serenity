namespace Serenity.Reporting;

/// <summary>
/// Set of options for HTML to PDF converter (WKHTMLToPdf)
/// </summary>
public interface IHtmlToPdfOptions
{
    /// <summary>
    /// The URL that is being converted
    /// </summary>
    string Url { get; set; }

    /// <summary>
    /// Optional list of URLs that should be converted and appended to the document
    /// </summary>
    List<string> AdditionalUrls { get; }

    /// <summary>
    /// Contains cookies that will be sent to target URL
    /// </summary>
    Dictionary<string, string> Cookies { get; }

    /// <summary>
    /// Timeout for converter in seconds (default 300 seconds / 5 min)
    /// </summary>
    int TimeoutSeconds { get; set; }

    /// <summary>
    /// Use Print media type to render document (default true)
    /// </summary>
    bool UsePrintMediaType { get; set; }

    /// <summary>
    /// Print background image if any (default true)
    /// </summary>
    bool PrintBackground { get; set; }

    /// <summary>
    /// Page size constant like "A4", "Letter" etc. Default value is A4
    /// </summary>
    string PageSize { get; set; }

    /// <summary>
    /// Page width like 21cm. Default value is unspecified.
    /// </summary>
    string PageWidth { get; set; }

    /// <summary>
    /// Page height like 15cm. Default value is unspecified.
    /// </summary>
    string PageHeight { get; set; }

    /// <summary>
    /// Use smart shrinking. Default is false.
    /// </summary>
    bool SmartShrinking { get; set; }

    /// <summary>
    /// Document DPI. Default is unspecified.
    /// </summary>
    int? Dpi { get; set; }

    /// <summary>
    /// Landscape page layout. Default is false.
    /// </summary>
    bool Landscape { get; set; }

    /// <summary>
    /// Zoom value like "0.33". Default is unspecified.
    /// </summary>
    string Zoom { get; set; }

    /// <summary>
    /// Use this to set all margins (left, right, bottom, top) at once
    /// </summary>
    string MarginsAll { set; }

    /// <summary>
    /// Page left margin, default is unspecified.
    /// </summary>
    string MarginLeft { get; set; }

    /// <summary>
    /// Page right margin, default is unspecified.
    /// </summary>
    string MarginRight { get; set; }

    /// <summary>
    /// Page bottom margin, default is unspecified.
    /// </summary>
    string MarginBottom { get; set; }

    /// <summary>
    /// Page top margin, default is unspecified.
    /// </summary>
    string MarginTop { get; set; }

    /// <summary>
    /// Url of header HTML page
    /// </summary>
    string HeaderHtmlUrl { get; set; }

    /// <summary>
    /// Url of footer HTML page
    /// </summary>
    string FooterHtmlUrl { get; set; }

    /// <summary>
    /// A set of key value pairs that should be search replaced in footer and header
    /// </summary>
    Dictionary<string, string> FooterHeaderReplace { get; }

    /// <summary>
    /// Disables local file access, default is true
    /// </summary>
    bool DisableLocalFileAccess { get; set; }

    /// <summary>
    /// List of allowed local folder (or file) paths
    /// </summary>
    List<string> AllowedLocalPaths { get; }

    /// <summary>
    /// A list of custom arguments to pass to HTML to PDF converter
    /// </summary>
    List<string> CustomArgs { get; }
}