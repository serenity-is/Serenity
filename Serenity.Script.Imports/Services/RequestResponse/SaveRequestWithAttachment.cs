using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class SaveRequestWithAttachment<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
        public object[] Attachments { get; set; }
    }
}