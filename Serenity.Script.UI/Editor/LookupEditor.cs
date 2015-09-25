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
            if (options.InplaceAdd)
                AddInplaceCreate(Texts.Controls.SelectEditor.InplaceAdd);
        }

        protected override string GetLookupKey()
        {
            return options.LookupKey ?? base.GetLookupKey();
        }

        protected override string GetDialogTypeKey()
        {
            return options.DialogType ?? base.GetDialogTypeKey();
        }

        protected override Select2Options GetSelect2Options()
        {
            var opt = base.GetSelect2Options();

            if (options.MinimumResultsForSearch != null)
                opt.MinimumResultsForSearch = options.MinimumResultsForSearch.Value;

            if (options.InplaceAdd)
                opt.CreateSearchChoice = GetCreateSearchChoice();

            return opt;
        }
    }

    [Serializable, Reflectable]
    public class LookupEditorOptions
    {
        public string LookupKey { get; set; }
        public int? MinimumResultsForSearch { get; set; }
        public bool InplaceAdd { get; set; }
        public string DialogType { get; set; }
    }
}