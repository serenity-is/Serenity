using System;

namespace Serenity.Data
{
    public static class AliasExtensions
    {
        public static Alias WithNoLock(this IAlias alias)
        {
            if (String.IsNullOrEmpty(alias.Table))
                return new Alias(alias.Name + " WITH(NOLOCK)");

            return new Alias(alias.Table, alias.Name + " WITH(NOLOCK)");
        }
    }
}