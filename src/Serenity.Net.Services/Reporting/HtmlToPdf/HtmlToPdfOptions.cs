namespace Serenity.Reporting;

/// <summary>
/// HTML to PDF options
/// </summary>
public class HtmlToPdfOptions : IHtmlToPdfOptions
{       
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public HtmlToPdfOptions()
    {
        AdditionalUrls = new List<string>();
        Cookies = new Dictionary<string, string>();
        CustomArgs = new List<string>();
        FooterHeaderReplace = new Dictionary<string, string>();
        TimeoutSeconds = 300;
        UsePrintMediaType = true;
        PrintBackground = true;
        PageSize = "A4";
    }

    /// <inheritdoc/>
    public string Url { get; set; }

    /// <inheritdoc/>
    public List<string> AdditionalUrls { get; set; }

    /// <inheritdoc/>
    public Dictionary<string, string> Cookies { get; set; }

    /// <inheritdoc/>
    public int TimeoutSeconds { get; set; }

    /// <inheritdoc/>
    public bool UsePrintMediaType { get; set; }

    /// <inheritdoc/>
    public bool PrintBackground { get; set; }

    /// <inheritdoc/>
    public string PageHeight { get; set; }
    
    /// <inheritdoc/>
    public string PageSize { get; set; }

    /// <inheritdoc/>
    public string PageWidth { get; set; }

    /// <inheritdoc/>
    public bool SmartShrinking { get; set; }

    /// <inheritdoc/>
    public int? Dpi { get; set; }

    /// <inheritdoc/>
    public bool Landscape { get; set; }

    /// <inheritdoc/>
    public string Zoom { get; set; }

    /// <inheritdoc/>
    public string MarginLeft { get; set; }

    /// <inheritdoc/>
    public string MarginRight { get; set; }
    
    /// <inheritdoc/>
    public string MarginBottom { get; set; }

    /// <inheritdoc/>
    public string MarginTop { get; set; }

    /// <inheritdoc/>
    public string HeaderHtmlUrl { get; set; }

    /// <inheritdoc/>
    public string FooterHtmlUrl { get; set; }


    /// <inheritdoc/>
    public Dictionary<string, string> FooterHeaderReplace { get; private set; }

    /// <inheritdoc/>
    public List<string> CustomArgs { get; private set; }

    /// <inheritdoc/>
    public string MarginsAll
    {
        set
        {
            MarginLeft = value;
            MarginTop = value;
            MarginRight = value;
            MarginBottom = value;
        }
    }
}