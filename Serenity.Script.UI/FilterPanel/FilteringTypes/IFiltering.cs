using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IFiltering
    {
        PropertyItem Field { get; set; }
        jQueryObject Container { get; set; }
        FilterOperator Operator { get; set; }
        void CreateEditor();
        CriteriaWithText GetCriteria();
        List<FilterOperator> GetOperators();
        void LoadState(object state);
        object SaveState();
    }

    [Imported, Serializable]
    public class CriteriaWithText
    {
        public BaseCriteria Criteria { get; set; }
        public string DisplayText { get; set; }
    }
}
