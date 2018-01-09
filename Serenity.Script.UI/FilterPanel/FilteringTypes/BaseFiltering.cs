using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public abstract class BaseFiltering : IFiltering, IQuickFiltering
    {
        public PropertyItem Field { get; set; }
        public jQueryObject Container { get; set; }
        public FilterOperator Operator { get; set; }

        public abstract List<FilterOperator> GetOperators();

        protected List<FilterOperator> AppendNullableOperators(List<FilterOperator> list)
        {
            return null;
        }

        protected List<FilterOperator> AppendComparisonOperators(List<FilterOperator> list)
        {
            return null;
        }

        protected virtual bool IsNullable()
        {
            return false;
        }

        public virtual void CreateEditor()
        {
        }

        public virtual string OperatorFormat(FilterOperator op)
        {
            return null;
        }

        protected virtual string GetTitle(PropertyItem field)
        {
            return null;
        }

        protected string DisplayText(FilterOperator op, params object[] values)
        {
            return null;
        }

        protected virtual string GetCriteriaField()
        {
            return null;
        }

        public virtual CriteriaWithText GetCriteria()
        {
            return null;
        }

        public virtual void LoadState(object state)
        {
        }

        public virtual object SaveState()
        {
            return null;
        }

        protected ArgumentNullException ArgumentNull()
        {
            return null;
        }

        protected virtual object ValidateEditorValue(string value)
        {
            return null;
        }

        protected virtual object GetEditorValue()
        {
            return null;
        }

        protected virtual string GetEditorText()
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        public virtual void InitQuickFilter(QuickFilter<Widget, object> filter)
        {
        }
    }
}