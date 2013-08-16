using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public interface ISqlQuery
    {
        IDictionary<string, object> Params { get; }
        string Text { get; }
    }
}
