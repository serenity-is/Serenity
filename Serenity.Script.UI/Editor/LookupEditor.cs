using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, OptionsType(typeof(LookupEditorOptions))]
    public class LookupEditor : LookupEditorBase<LookupEditorOptions, object>
    {
        public LookupEditor(jQueryObject hidden, LookupEditorOptions opt)
            : base(hidden, opt)
        {
        }

        protected override string GetLookupKey()
        {
            return options.LookupKey ?? base.GetLookupKey();
        }

        protected override Select2Options GetSelect2Options()
        {
            var opt = base.GetSelect2Options();

            if (options.MinimumResultsForSearch != null)
                opt.MinimumResultsForSearch = options.MinimumResultsForSearch.Value;

            return opt;
        }
    }

    [Serializable, Reflectable]
    public class LookupEditorOptions
    {
        public string LookupKey { get; set; }
        public int? MinimumResultsForSearch { get; set; }
    }
}