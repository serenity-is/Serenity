using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IFilterField
    {
        string Name { get; set; }
        LocalText Title { get; set; }
        string Handler { get; set; }
        bool IsRequired { get; set; }
        string Textual { get; set; }
        Dictionary<string, object> Params { get; set; }
    }
}
