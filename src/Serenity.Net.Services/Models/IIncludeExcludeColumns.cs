using System.Collections.Generic;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Services
{
    public interface IIncludeExcludeColumns
    {
        HashSet<string> IncludeColumns { get; set; }
        HashSet<string> ExcludeColumns { get; set; }
    }
}