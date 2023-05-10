using System.Net;

namespace Serenity.Reporting;

/// <summary>
/// Response type for IHtmlReportRenderUrlBuilder.GetRenderUrl method
/// </summary>
public class HtmlReportRenderUrl : IDisposable
{
    private bool disposed;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public HtmlReportRenderUrl()
    {
        CookiesToForward = new List<Cookie>();
    }

    /// <summary>
    /// The generated URL for main page
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The generated URL for footer content
    /// </summary>
    public string FooterUrl { get; set; }

    /// <summary>
    /// The generated URL for header content
    /// </summary>
    public string HeaderUrl { get; set; }

    /// <summary>
    /// List of cookies to forward
    /// </summary>
    public IList<Cookie> CookiesToForward { get; private set; }

    /// <summary>
    /// Gets the list of temporary folders if any. This can be
    /// used to pass them to WKHTMLTOPDF's --allow argument.
    /// </summary>
    public virtual IEnumerable<string> GetTemporaryFolders()
    {
        return Array.Empty<string>();
    }

    /// <summary>
    /// Cleanup method that can be overridden by derived classes.
    /// </summary>
    protected virtual void Cleanup()
    {
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~HtmlReportRenderUrl()
    {
        if (!disposed)
        {
            Cleanup();
            disposed = true;
        }
    }

    /// <summary>
    /// Disposes resources
    /// </summary>
    public void Dispose()
    {
        if (!disposed)
        {
            Cleanup();
            disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}