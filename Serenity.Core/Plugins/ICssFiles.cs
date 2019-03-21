using System.Collections.Generic;

namespace Serenity.Plugins
{
    public interface ICssFiles
    {
        IEnumerable<CssFile> GetCssFiles();
    }
}