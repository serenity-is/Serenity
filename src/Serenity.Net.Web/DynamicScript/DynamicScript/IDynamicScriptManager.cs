using System;
using System.Collections.Generic;

namespace Serenity.Web
{
    public interface IDynamicScriptManager
    {
        event Action<string> ScriptChanged;

        void Changed(string name);
        void CheckScriptRights(string name);
        Dictionary<string, string> GetRegisteredScripts();
        IEnumerable<string> GetRegisteredScriptNames();
        string GetScriptInclude(string name, string extension = ".js");
        string GetScriptText(string name);
        IScriptContent ReadScriptContent(string name);
        void IfNotRegistered(string name, Func<IDynamicScript> callback);
        bool IsRegistered(string name);
        void Register(INamedDynamicScript script);
        void Register(string name, IDynamicScript script);
        void Reset();
    }
}