using System;
using System.Collections.Generic;
using System.Reflection;
using Serenity.Data;
using System.ComponentModel;
using Newtonsoft.Json;
using Serenity.ComponentModel;

namespace Serenity.ComponentModel
{
    public class PropertyItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("hint")]
        public string Hint { get; set; }
        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }
        [JsonProperty("editorType")]
        public string EditorType { get; set; }
        [JsonProperty("editorParams")]
        public Dictionary<string, object> EditorParams { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("cssClass")]
        public string CssClass { get; set; }
        [JsonProperty("maxLength")]
        public int? MaxLength { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }
        [JsonProperty("insertable")]
        public bool Insertable { get; set; }
        [JsonProperty("updatable")]
        public bool Updatable { get; set; }
        [JsonProperty("oneWay")]
        public bool OneWay { get; set; }
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }
        [JsonProperty("localizable")]
        public bool Localizable { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("minWidth")]
        public int MinWidth { get; set; }
        [JsonProperty("maxWidth")]
        public int MaxWidth { get; set; }
        [JsonProperty("resizable")]
        public bool Resizable { get; set; }
        
        [JsonProperty("editLink")]
        public bool EditLink { get; set; }
        [JsonProperty("editLinkItemType")]
        public string EditLinkItemType { get; set; }
        [JsonProperty("editLinkIdField")]
        public string EditLinkIdField { get; set; }
        [JsonProperty("editLinkCssClass")]
        public string EditLinkCssClass { get; set; }

        public PropertyItem()
        {
            EditorParams = new Dictionary<string, object>();
        }
    }
}