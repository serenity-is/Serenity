using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class RetrieveResponse<TEntity> : ServiceResponse
    {
        public TEntity Entity { get; set; }
    }
}