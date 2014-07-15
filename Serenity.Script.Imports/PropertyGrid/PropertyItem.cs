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
    }
}