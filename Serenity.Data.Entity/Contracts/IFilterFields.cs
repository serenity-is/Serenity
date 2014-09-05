using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IFilterFields : IEnumerable<IFilterField>
    {
        IFilterField ByNameOrTextual(string name);
    }
}
