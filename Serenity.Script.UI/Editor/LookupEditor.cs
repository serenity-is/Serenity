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
    }

    [Imported, Serializable, Reflectable]
    public class LookupEditorOptions
    {
        public string LookupKey { get; set; }
        public int? MinimumResultsForSearch { get; set; }
        public bool InplaceAdd { get; set; }
        public string DialogType { get; set; }
        /// <summary>
        /// ID (can be relative) of the editor that this editor will cascade from, e.g. Country
        /// </summary>
        public string CascadeFrom { get; set; }
        /// <summary>
        /// Cascade filtering field (items will be filtered on this key, e.g. CountryID).
        /// If null, CascadeFrom is used as cascade field name too.
        /// </summary>
        public string CascadeField { get; set; }
        /// <summary>
        /// Cascade filtering value, usually set by CascadeFrom editor, e.g. the integer value of CountryID
        /// If null or empty, and CascadeField/CascadeFrom is set, all items are filtered
        /// </summary>
        public object CascadeValue { get; set; }
        /// <summary>
        /// Optional filtering field (items will be filtered on this key, e.g. GroupID)
        /// </summary>
        public string FilterField { get; set; }
        /// <summary>
        /// Optional filtering value, usually set by CascadeFrom editor, e.g. the integer value of CountryID
        /// If null or empty, and CascadeField is set, all items are filtered
        /// </summary>
        public object FilterValue { get; set; }
        /// <summary>
        /// Allow multiple selection
        /// </summary>
        public bool Multiple { get; set; }
        /// <summary>
        /// In multiple mode, use a comma separated string, instead of an array when serializing value
        /// </summary>
        public bool Delimited { get; set; }
        /// <summary>
        /// If this property is false, the Select2Item is not clearable
        /// </summary>
        public bool AllowClear { get; set; }
    }
}
