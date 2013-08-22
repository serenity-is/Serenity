using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public interface ISqlQuery
    {
        Dictionary<string, object> Params { get; set; }
        string Text { get; }
    }
}
