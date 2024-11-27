using Serenity.Reporting;

namespace Serenity.Demo.Northwind.Reports;

/// <summary>
/// This is an external report sample that opens a Google search page on execute.
/// You could open any other page, internal or external, e.g. an SSRS report web page
/// </summary>
[Report]
[Category("Northwind/Orders"), DisplayName("Google Search - External Report Sample")]
public class GoogleSearchReport : IReport, IExternalReport
{
    [DisplayName("Search Query"), Required(true)]
    public string Query { get; set; }

    public object GetData()
    {
        return "https://www.google.com.tr/search?q=" + Uri.EscapeDataString(Query);
    }
}