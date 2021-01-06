using Microsoft.AspNetCore.Http;

namespace Serenity.Web
{
    public interface IContentHashCache
    {
        string ResolvePath(PathString pathBase, string contentPath);
        string ResolveWithHash(PathString pathBase, string contentUrl);
        void ScriptsChanged();
    }
}