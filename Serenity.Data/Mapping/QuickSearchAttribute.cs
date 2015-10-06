using System;

namespace Serenity.Data.Mapping
{
    public class QuickSearchAttribute : Attribute
    {
        public QuickSearchAttribute(SearchType searchType = SearchType.Auto, int numericOnly = -1, bool isExplicit = false)
        {
            this.SearchType = searchType;
            this.NumericOnly = numericOnly < 0 ? (bool?)null : (numericOnly > 0 ? true : false);
            this.IsExplicit = isExplicit;
        }

        public SearchType SearchType { get; private set; }
        public bool? NumericOnly { get; private set; }
        public bool IsExplicit { get; private set; }
    }
}