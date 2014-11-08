using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class PropertyItem
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Hint { get; set; }
        public string Placeholder { get; set; }
        public string EditorType { get; set; }
        public JsDictionary EditorParams { get; set; }
        public string Category { get; set; }
        public string CssClass { get; set; }
        public int? MaxLength { get; set; }
        public bool Required { get; set; }
        public bool? Insertable { get; set; }
        public bool? Updatable { get; set; }
        public bool ReadOnly { get; set; }
        public bool OneWay { get; set; }
        public object DefaultValue { get; set; }
        public bool Localizable { get; set; }

        public string FormatterType { get; set; }
        public JsDictionary FormatterParams { get; set; }
        public string DisplayFormat { get; set; }
        public string Alignment { get; set; }
        public int Width { get; set; }
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }
        public bool Resizable { get; set; }

        public bool EditLink { get; set; }
        public string EditLinkItemType { get; set; }
        public string EditLinkIdField { get; set; }
        public string EditLinkCssClass { get; set; }

        public string FilteringType { get; set; }
        public JsDictionary FilteringParams { get; set; }
        public string FilteringIdField { get; set; }
        public bool NotFilterable { get; set; }
        public bool FilterOnly { get; set; }
    }
}