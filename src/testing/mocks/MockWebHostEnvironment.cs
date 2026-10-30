namespace Serenity.Testing;

public class MockWebHostEnvironment(string applicationName = null, IFileProvider contentRootFileProvider = null,
    string contentRootPath = null, string environmentName = null, string webRootPath = null,
    IFileProvider webRootFileProvider = null) : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = applicationName;
    public IFileProvider ContentRootFileProvider { get; set; } = contentRootFileProvider;
    public string ContentRootPath { get; set; } = contentRootPath;
    public string EnvironmentName { get; set; } = environmentName;
    public string WebRootPath { get; set; } = webRootPath;
    public IFileProvider WebRootFileProvider { get; set; } = webRootFileProvider;
}