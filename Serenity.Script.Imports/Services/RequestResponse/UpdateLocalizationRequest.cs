using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class UpdateLocalizationRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
        public string CultureId { get; set; }
    }
}