#if IsTemplateBuild
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;

namespace Build;

public static partial class Shared
{
    public static string GetLatestNpmPackageVersion(string packageName)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://registry.npmjs.org/{packageName}");
        using var registryContent = httpClient.Send(request).Content.ReadAsStream();
        using var sr = new StreamReader(registryContent);
        return JObject.Parse(sr.ReadToEnd())["dist-tags"]?["latest"]?.Value<string>();
    }
}
#endif