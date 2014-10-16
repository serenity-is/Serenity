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
            private IFilterableSource source;

            public FieldSelect(jQueryObject hidden, IFilterableSource source)
                : base(hidden, null)
            {
                this.source = source;
            }

            protected override string GetItemKey(IFilterField item)
            {
                return item.Name;
            }

            protected override string GetItemText(IFilterField item)
            {
                return item.Title ?? item.Name;
            }

            protected override string GetService()
            {
                return "Dummy";
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

            protected override void ExecuteQuery(ServiceCallOptions<ListResponse<IFilterField>> options)
            {
                IEnumerable<IFilterField> filtered = source.GetFields();
                var request = options.Request.As<ListRequest>();

                if (!request.ContainsText.IsEmptyOrNull())
                {
                    var contains = Q.Externals.StripDiacritics(request.ContainsText.ToLower());
                    filtered = filtered.Where(x => Q.Externals.StripDiacritics(x.Title ?? x.Name ?? "")
                        .ToLower().IndexOf(contains) >= 0);
                }

                if (Script.IsValue(request.Skip) && request.Skip != 0)
                    filtered = filtered.Skip(request.Skip);

                if (Script.IsValue(request.Take) && request.Take != 0)
                    filtered = filtered.Take(request.Take);

                if (options.OnSuccess != null)
                    options.OnSuccess(new ListResponse<IFilterField>
                    {
                        Entities = filtered.ToList()
                    });
            }
        }
    }
}