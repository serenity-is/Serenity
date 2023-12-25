namespace Serenity.Reporting;

/// <summary>
/// The attribute used to determine the location of the report design
/// which is usually a CSHTML file.
/// </summary>
/// <remarks>
/// Creates an instance of the attribute
/// </remarks>
/// <param name="design"></param>
public class ReportDesignAttribute(string design) : Attribute
{

    /// <summary>
    /// Returns the location of the design file which is passed via the constructor.
    /// </summary>
    public string Design { get; private set; } = design ?? throw new ArgumentNullException(nameof(design));
}