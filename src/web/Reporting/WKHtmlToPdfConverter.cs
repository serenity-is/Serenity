using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Serenity.Reporting;

/// <summary>
/// Implementation for IWKHtmlToPdfConverter
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="options">Options</param>
/// <param name="webHostEnvironment">Host environment</param>
/// <param name="fileSystem">File system</param>
public class WKHtmlToPdfConverter(IOptions<WKHtmlToPdfSettings> options = null,
    IWebHostEnvironment webHostEnvironment = null, IFileSystem fileSystem = null) : IWKHtmlToPdfConverter
{
    private readonly IOptions<WKHtmlToPdfSettings> options = options;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
    private readonly IFileSystem fileSystem = fileSystem ?? new PhysicalFileSystem();
    private string executablePath;

    /// <summary>
    /// Gets wkhtmltopdf executable path
    /// </summary>
    /// <returns></returns>
    public virtual string GetExecutablePath()
    {
        if (!string.IsNullOrEmpty(executablePath) && 
            fileSystem.FileExists(executablePath))
            return executablePath;

        if (!string.IsNullOrEmpty(options?.Value.ExecutablePath) &&
            fileSystem.FileExists(options.Value.ExecutablePath))
        {
            executablePath = options.Value.ExecutablePath;
            return executablePath;
        }

        var assemblyPath = fileSystem.GetDirectoryName(typeof(WKHtmlToPdfConverter).Assembly.Location);

        string[] wkhtmlFileNames =
            Environment.OSVersion.Platform == PlatformID.Win32NT ?
                ["wkhtmltopdf.exe", "wkhtmltopdf.cmd", "wkhtmltopdf.bat"] :
                ["wkhtmltopdf", "wkhtmltopdf.sh"];

        IEnumerable<string> paths = [assemblyPath];
        string contentRootPath = webHostEnvironment?.ContentRootPath;
        if (!string.IsNullOrEmpty(contentRootPath))
            paths = paths.Concat(
            [
                fileSystem.Combine(contentRootPath),
                fileSystem.Combine(contentRootPath, "App_Data", "Reporting"),
                fileSystem.Combine(contentRootPath, "App_Data", "reporting"),
                fileSystem.Combine(contentRootPath, "bin")
            ]);

        paths = paths.Concat((Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'));

        executablePath = paths.SelectMany(path =>
            wkhtmlFileNames.Select(f => fileSystem.Combine(path, f)))
                .FirstOrDefault(fileSystem.FileExists);

        return executablePath;
    }

    /// <inheritdoc/>
    public byte[] Convert(IHtmlToPdfOptions options)
    {
        var converter = new WKHtmlToPdf(options)
        {
            ExecutablePath = GetExecutablePath()
        };

        if (string.IsNullOrEmpty(converter.ExecutablePath))
            throw new ValidationError("Can't locate wkhtmltopdf executable " +
                "that is required for report generation in the system PATH, or in the web application " + 
                "directory. Please download and install the version suitable for your system from " +
                "https://wkhtmltopdf.org/downloads.html");

        return converter.Execute();
    }
}