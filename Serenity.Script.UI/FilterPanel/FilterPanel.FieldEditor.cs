using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public partial class FilterPanel : FilterWidgetBase<object>
    {
        private class FieldSelect : Select2Editor<object, IFilterField>
        {
            public FieldSelect(jQueryObject hidden, IFilterableSource source)
                : base(hidden, null)
            {
                foreach (var field in source.GetFields())
                    AddItem(field.Name, field.Title ?? field.Name, field, false);
            }

            protected override string EmptyItemText()
            {
                if (this.Value.IsEmptyOrNull())
                    return Q.Text("Controls.FilterPanel.SelectField");

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