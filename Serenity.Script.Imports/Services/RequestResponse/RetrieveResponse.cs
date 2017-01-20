using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class RetrieveResponse<TEntity> : ServiceResponse
    {
        public TEntity Entity { get; set; }

        public JsDictionary<string, TEntity> Localizations { get; set; }
    }
}