namespace Serenity.Reporting;

/// <summary>
/// The attribute used to determine the location of the report design,
/// which is usually a CSHTML file.
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="design">The location of the design file.</param>
public class ReportDesignAttribute(string design) : Attribute
{

    /// <summary>
    /// Gets the location of the design file which is passed via the constructor.
    /// </summary>
    public string Design { get; private set; } = design ?? throw new ArgumentNullException(nameof(design));
}