using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class SaveWithLocalizationRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
    }
}