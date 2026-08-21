namespace Serenity.Reporting;

/// <summary>
/// An interface reports can implement to customize HTML to PDF
/// converter settings.
/// </summary>
public interface ICustomizeHtmlToPdf
{
    /// <summary>
    /// Method that is called to customize HTML to PDF options.
    /// </summary>
    /// <param name="options">The HTML to PDF options to customize.</param>
    void Customize(IHtmlToPdfOptions options);
}