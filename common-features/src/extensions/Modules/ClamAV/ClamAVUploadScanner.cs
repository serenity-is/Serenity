using Microsoft.Extensions.Logging;
using nClam;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Implementation of <see cref="IUploadAVScanner"/> which connects to ClamAV service
/// (https://www.clamav.net/ - Windows: https://oss.netfarm.it/clamav/), 
/// using the nClam library (https://github.com/tekmaven/nClam)
/// </summary>
namespace Serenity.Extensions;

/// <summary>
/// Implementation of <see cref="IUploadAVScanner"/> which connects to ClamAV service
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="options">Options</param>
/// <param name="localizer">Text localizer</param>
/// <param name="logger">Logger</param>
/// <exception cref="ArgumentNullException">One of arguments is null</exception>
public class ClamAVUploadScanner(IOptionsMonitor<ClamAVSettings> options,
    ITextLocalizer localizer = null,
    ILogger<ClamAVUploadScanner> logger = null) : IUploadAVScanner
{
    private readonly IOptionsMonitor<ClamAVSettings> options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Processes a temporary upload stream, usually from the HTTP request files and
    /// returns false 
    /// </summary>
    /// <param name="stream">File content stream (usually from HTTP request files)</param>
    /// <param name="filename">The filename of the uploaded file (original name)</param>
    public void Scan(Stream stream, string filename)
    {
        try
        {
            var settings = options.CurrentValue;
            if (!settings.Enabled)
                return;

            // please install ClamAV on your server, otherwise all uploads will fail
            // it is assumed to be running at localhost:3310 by default
            // to disable AV scan, set ClamAV:Enabled to false in appsettings.json

            var host = settings.Host;
            if (string.IsNullOrEmpty(host))
                host = "localhost";
            var port = settings.Port;

            var clam = new ClamClient(host, port);

            var scanResult = Task.Run(() => clam.SendAndScanFileAsync(stream)).Result;

            switch (scanResult.Result)
            {
                case ClamScanResults.VirusDetected:
                    logger?.LogError(InformationalException.EventId,
                        "Virus {virus} found in file getting uploaded: {filename}",
                        scanResult.InfectedFiles.First().VirusName,
                        filename);

                    throw new ValidationError("InfectedFile",
                        UploadTexts.Controls.ImageUpload.InfectedFile.ToString(localizer));

                case ClamScanResults.Unknown:
                case ClamScanResults.Error:
                    // reaching here does not mean the uploaded file is virus free
                    // another antivirus might have deleted the temporary file before scan
                    logger?.LogError(InformationalException.EventId,
                        "Error occured during AV scan: {error}",
                        scanResult.RawResult);

                    throw new ValidationError("InfectedFileOrError",
                        UploadTexts.Controls.ImageUpload.InfectedFileOrError.ToString(localizer));
            }
        }
        catch (Exception ex) when (ex is not ValidationError)
        {
            logger?.LogError(ex, "Error occured during AV scan (is ClamAV installed?)");

            throw new ValidationError("FailedScan",
                UploadTexts.Controls.ImageUpload.FailedScan.ToString(localizer));
        }
    }
}