namespace Serenity.Reporting;

/// <summary>
/// The attribute used to determine the location of the report design
/// which is usually a CSHTML file.
/// </summary>
public class ReportDesignAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="design"></param>
    public ReportDesignAttribute(string design)
    {
        Design = design ?? throw new ArgumentNullException(nameof(design));
    }

    /// <summary>
    /// Returns the location of the design file which is passed via the constructor.
    /// </summary>
    public string Design { get; private set; }
}