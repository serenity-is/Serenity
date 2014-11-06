using jQueryApi;
using Serenity.Data;
using System.Collections.Generic;

namespace Serenity
{
    public interface IFilterHandler
    {
        IFilterField Field { get; set; }
        jQueryObject Container { get; set; }
        void CreateEditor(FilterOperator op);
        BaseCriteria GetCriteria(FilterOperator op, out string displayText);
        List<FilterOperator> GetOperators();
        void LoadState(object state);
        object SaveState();
    }
}
