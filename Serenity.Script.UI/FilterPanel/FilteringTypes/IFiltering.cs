using jQueryApi;
using Serenity.Data;
using System.Collections.Generic;

namespace Serenity
{
    public interface IFiltering
    {
        PropertyItem Field { get; set; }
        jQueryObject Container { get; set; }
        FilterOperator Operator { get; set; }
        void CreateEditor();
        BaseCriteria GetCriteria(out string displayText, ref string errorMessage);
        List<FilterOperator> GetOperators();
        void LoadState(object state);
        object SaveState();
    }
}
