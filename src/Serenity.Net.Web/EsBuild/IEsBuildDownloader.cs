using System.Threading.Tasks;

namespace Serenity.Web.EsBuild;

internal interface IEsBuildDownloader
{
    Task<string> DownloadAsync(string version, string targetDirectory);
    string Download(string version, string targetDirectory);
    Task<string> GetLatestVersionAsync(string binString);
    string GetLatestVersionSync(string binString);
}
