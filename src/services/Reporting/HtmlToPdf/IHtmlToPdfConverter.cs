namespace Serenity.Reporting;

/// <summary>
/// Abstraction for service that converts HTML to PDF
/// </summary>
public interface IHtmlToPdfConverter
{
    /// <summary>
    /// Converts HTML to PDF
    /// </summary>
    /// <param name="options">Conversion options</param>
    byte[] Convert(IHtmlToPdfOptions options);
}