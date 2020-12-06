using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IRowTypeRegistry
    {
        IEnumerable<Type> AllRowTypes { get; }
        IEnumerable<Type> ByConnectionKey(string connectionKey);
    }
}