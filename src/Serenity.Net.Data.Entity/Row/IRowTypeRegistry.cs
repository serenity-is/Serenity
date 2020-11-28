using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IRowTypeRegistry
    {
        IEnumerable<Type> All { get; }
        IEnumerable<Type> ByConnectionKey(string connectionKey);
    }
}