using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class UpdateRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
    }
}