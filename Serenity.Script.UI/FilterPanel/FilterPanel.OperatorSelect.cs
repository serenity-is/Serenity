using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public partial class FilterPanel : FilterWidgetBase<object>
    {
        private class OperatorSelect : Select2Editor<object, FilterOperator>
        {
            public OperatorSelect(jQueryObject hidden, IEnumerable<FilterOperator> source)
                : base(hidden, null)
            {
                foreach (var op in source)
                {
                    var title = op.Title ?? Q.TryGetText("Controls.FilterPanel.OperatorNames." + op.Key) ?? op.Key;
                    AddItem(op.Key, title, op, false);
                }

                var first = source.FirstOrDefault();
                if (first != null)
                    this.Value = first.Key;
            }

            protected override string EmptyItemText()
            {
                return null;
            }

            protected override Select2Options GetSelect2Options()
            {
                var opt = base.GetSelect2Options();
                opt.AllowClear = false;
                return opt;
            }
        }
    }
}