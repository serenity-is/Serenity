using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
        [JsonProperty("collapsible")]
        public bool? Collapsible { get; set; }
        [JsonProperty("collapsed")]
        public bool? Collapsed { get; set; }
        [JsonProperty("tab")]
        public string Tab { get; set; }
        [JsonProperty("cssClass")]
        public string CssClass { get; set; }
        [JsonProperty("headerCssClass")]
        public string HeaderCssClass { get; set; }
        [JsonProperty("formCssClass")]
        public string FormCssClass { get; set; }
        [JsonProperty("maxLength")]
        public int? MaxLength { get; set; }
        [JsonProperty("required")]
        public bool? Required { get; set; }
        [JsonProperty("readOnly")]
        public bool? ReadOnly { get; set; }
        [JsonProperty("readPermission")]
        public string ReadPermission { get; set; }
        [JsonProperty("insertable")]
        public bool? Insertable { get; set; }
        [JsonProperty("insertPermission")]
        public string InsertPermission { get; set; }
        [JsonProperty("hideOnInsert")]
        public bool? HideOnInsert { get; set; }
        [JsonProperty("updatable")]
        public bool? Updatable { get; set; }
        [JsonProperty("updatePermission")]
        public string UpdatePermission { get; set; }
        [JsonProperty("hideOnUpdate")]
        public bool? HideOnUpdate { get; set; }
        [JsonProperty("oneWay")]
        public bool? OneWay { get; set; }
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }
        [JsonProperty("localizable")]
        public bool? Localizable { get; set; }
        [JsonProperty("visible")]
        public bool? Visible { get; set; }
        [JsonProperty("allowHide")]
        public bool? AllowHide { get; set; }

        [JsonProperty("formatterType")]
        public string FormatterType { get; set; }
        [JsonProperty("formatterParams")]
        public Dictionary<string, object> FormatterParams { get; set; }
        [JsonProperty("displayFormat")]
        public string DisplayFormat { get; set; }

        [JsonProperty("alignment")]
        public string Alignment { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }
        [JsonProperty("minWidth")]
        public int? MinWidth { get; set; }
        [JsonProperty("maxWidth")]
        public int? MaxWidth { get; set; }
        [JsonProperty("labelWidth")]
        public string LabelWidth { get; set; }
        [JsonProperty("resizable")]
        public bool? Resizable { get; set; }
        [JsonProperty("sortable")]
        public bool? Sortable { get; set; }
        [JsonProperty("sortOrder")]
        public int? SortOrder { get; set; }
        [JsonProperty("groupOrder")]
        public int? GroupOrder { get; set; }
        [JsonProperty("summaryType")]
        public SummaryType? SummaryType { get; set; }

        [JsonProperty("editLink")]
        public bool? EditLink { get; set; }
        [JsonProperty("editLinkItemType")]
        public string EditLinkItemType { get; set; }
        [JsonProperty("editLinkIdField")]
        public string EditLinkIdField { get; set; }
        [JsonProperty("editLinkCssClass")]
        public string EditLinkCssClass { get; set; }

        [JsonProperty("filteringType")]
        public string FilteringType { get; set; }
        [JsonProperty("filteringParams")]
        public Dictionary<string, object> FilteringParams { get; set; }
        [JsonProperty("filteringIdField")]
        public string FilteringIdField { get; set; }
        [JsonProperty("filterOnly")]
        public bool? FilterOnly { get; set; }
        [JsonProperty("notFilterable")]
        public bool? NotFilterable { get; set; }

        [JsonProperty("quickFilter")]
        public bool? QuickFilter { get; set; }
        [JsonProperty("quickFilterParams")]
        public Dictionary<string, object> QuickFilterParams { get; set; }
        [JsonProperty("quickFilterSeparator")]
        public bool? QuickFilterSeparator { get; set; }
        [JsonProperty("quickFilterCssClass")]
        public string QuickFilterCssClass { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }

        public bool ShouldSerializeEditorType()
        {
            return EditorType != null && EditorType != "String";
        }

        public bool ShouldSerializeFilteringType()
        {
            return FilteringType != null && FilteringType != "String";
        }

        public bool ShouldSerializeEditorParams()
        {
            return EditorParams != null && EditorParams.Count > 0;
        }

        public bool ShouldSerializeFormatterParams()
        {
            return EditorParams != null && FormatterParams.Count > 0;
        }

        public bool ShouldSerializeFilteringParams()
        {
            return FilteringParams != null && FilteringParams.Count > 0;
        }

        public bool ShouldSerializeQuickFilterParams()
        {
            return QuickFilterParams != null && QuickFilterParams.Count > 0;
        }

        public PropertyItem()
        {
            EditorParams = new Dictionary<string, object>();
            FormatterParams = new Dictionary<string, object>();
            FilteringParams = new Dictionary<string, object>();
            QuickFilterParams = new Dictionary<string, object>();
            ExtensionData = new Dictionary<string, JToken>();
        }
    }
}