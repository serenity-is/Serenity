using Serenity.Abstractions;
using System;

namespace Serenity.Web
{
    public interface IDynamicScript
    {
        string GetScript();
        void CheckRights(IPermissionService permissions, ITextLocalizer localizer);
        void Changed();
        string GroupKey { get; }
        TimeSpan Expiration { get; }
        event EventHandler ScriptChanged;
    }
}