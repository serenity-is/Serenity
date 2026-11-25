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

    /// <summary>
    /// Converts HTML to PDF asynchronously. The default implementation calls <see cref="Convert"/>, a converter that
    /// does its work asynchronously, e.g. by driving a browser, should override it.
    /// </summary>
    /// <param name="options">Conversion options</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<byte[]> ConvertAsync(IHtmlToPdfOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Convert(options));
    }
}