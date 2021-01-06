using System.Collections.Generic;

namespace Serenity.Web
{
    public interface ICssBundleManager
    {
        bool IsEnabled { get; }

        void CssChanged();
        IEnumerable<string> GetBundleIncludes(string bundleKey);
        string GetCssBundle(string cssUrl);
        void Reset();
    }
}