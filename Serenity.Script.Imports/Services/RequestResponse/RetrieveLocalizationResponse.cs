using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveLocalizationResponse<TEntity> : ServiceResponse
        where TEntity: new()
    {
        public JsDictionary<string, TEntity> Entities { get; set; }
    }
}