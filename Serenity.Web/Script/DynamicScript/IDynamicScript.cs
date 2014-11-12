using System;

namespace Serenity.Web
{
    public interface IDynamicScript
    {
        string GetScript();
        void CheckRights();
        void Changed();
        string GroupKey { get; }
        TimeSpan Expiration { get; }
        event EventHandler ScriptChanged;
    }
}