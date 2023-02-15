using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Serenity.Reporting;

/// <summary>
/// Implementation for IWKHtmlToPdfConverter
/// </summary>
public class WKHtmlToPdfConverter : IWKHtmlToPdfConverter
{
    private readonly IOptions<WKHtmlToPdfSettings> options;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IFileSystem fileSystem;
    private string utilityPath;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="webHostEnvironment">Host environment</param>
    /// <param name="fileSystem">File system</param>
    public WKHtmlToPdfConverter(IOptions<WKHtmlToPdfSettings> options = null,
        IWebHostEnvironment webHostEnvironment = null, IFileSystem fileSystem = null)
    {
        this.options = options;
        this.webHostEnvironment = webHostEnvironment;
        this.fileSystem = fileSystem ?? new PhysicalFileSystem();
    }

    /// <inheritdoc/>
    public string GetUtilityExePath()
    {
        if (!string.IsNullOrEmpty(utilityPath) && fileSystem.FileExists(utilityPath))
            return utilityPath;

        if (!string.IsNullOrEmpty(options?.Value.UtilityExePath) &&
            fileSystem.FileExists(options.Value.UtilityExePath))
        {
            utilityPath = options.Value.UtilityExePath;
            return utilityPath;
        }

        var assemblyPath = fileSystem.GetDirectoryName(typeof(WKHtmlToPdfConverter).Assembly.Location);

        string[] wkhtmlFileNames =
            Environment.OSVersion.Platform == PlatformID.Win32NT ?
                new[] { "wkhtmltopdf.exe", "wkhtmltopdf.cmd", "wkhtmltopdf.bat" } :
                new[] { "wkhtmltopdf", "wkhtmltopdf.sh" };

        IEnumerable<string> paths = new[] { assemblyPath };
        string contentRootPath = webHostEnvironment?.ContentRootPath;
        if (!string.IsNullOrEmpty(contentRootPath))
            paths = paths.Concat(new[]
            {
                fileSystem.Combine(contentRootPath),
                fileSystem.Combine(contentRootPath, "App_Data", "Reporting"),
                fileSystem.Combine(contentRootPath, "App_Data", "reporting"),
                fileSystem.Combine(contentRootPath, "bin")
            });

        paths = paths.Concat((Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'));

        utilityPath = paths.SelectMany(path =>
            wkhtmlFileNames.Select(f => fileSystem.Combine(path, f)))
                .FirstOrDefault(fileSystem.FileExists);

        return utilityPath;
    }

    /// <inheritdoc/>
    public byte[] Convert(IHtmlToPdfOptions options)
    {
        var converter = new HtmlToPdfConverter(options)
        {
            UtilityExePath = GetUtilityExePath()
        };

        if (string.IsNullOrEmpty(converter.UtilityExePath))
            throw new ValidationError("Can't locate wkhtmltopdf.exe (or wkhtmltopdf in Linux) " +
                "that is required for report generation in PATH or folder " +
                fileSystem.GetDirectoryName(typeof(WKHtmlToPdfConverter).Assembly.Location) +
                ". Please download and install the version suitable for your system from " +
                "https://wkhtmltopdf.org/downloads.html");

        return converter.Execute();
    }
}