using System;

namespace Serenity.Web
{
    public interface IDynamicScript
    {
        string GetScript();
        void CheckRights();
        void Changed();
        bool NonCached { get; }
        event EventHandler ScriptChanged;
    }
}