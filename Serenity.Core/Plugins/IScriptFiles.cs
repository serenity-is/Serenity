using System.Collections.Generic;

namespace Serenity.Plugins
{
    public interface IScriptFiles
    {
        IEnumerable<ScriptFile> GetScriptFiles();
    }
}