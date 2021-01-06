using System.Collections.Generic;

namespace Serenity.Services
{
    public interface IIncludeExcludeColumns
    {
        HashSet<string> IncludeColumns { get; set; }
        HashSet<string> ExcludeColumns { get; set; }
    }
}